using IPInfoProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using IP_Batch_API.Services;
using IP_Batch_API.Models;
using IP_Batch_API.Entities;

namespace IP_Batch_API.Controllers
{
    [ApiController]
    [Route("api/ipdetails")]
    public class IPDetailsController : ControllerBase
    {
        private readonly IIPInfoProvider _iPInfoProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly IIPBatchRepository _iPBatchRepository;
        private static Dictionary<Guid, Job> jobs = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IPDetailsController(IIPInfoProvider iPInfoProvider, IMemoryCache memoryCache, IMapper mapper, IIPBatchRepository iPBatchRepository, IServiceScopeFactory serviceScopeFactory)
        {
            _iPInfoProvider = iPInfoProvider ?? throw new ArgumentNullException(nameof(iPInfoProvider));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _iPBatchRepository = iPBatchRepository ?? throw new ArgumentNullException(nameof(iPBatchRepository));
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet("{ip}")]
        public async Task<ActionResult<IPDetailDto>> GetDetails(string ip)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ip))
                    return BadRequest();

                if (_memoryCache.TryGetValue(ip, out IPDetail ipDetails))
                {
                    var ipDetailsDto = _mapper.Map<IPDetailDto>(ipDetails);

                    return Ok(ipDetailsDto);
                }

                if (await _iPBatchRepository.GetIPDetailsAsync(ip) is IPDetail ipDetailsFromDb)
                {
                    var ipDetailsDto = _mapper.Map<IPDetailDto>(ipDetailsFromDb);

                    _memoryCache.Set(ip, ipDetailsFromDb, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));

                    return Ok(ipDetailsDto);
                }

                if (await _iPInfoProvider.GetDetails(ip) is not IPDetails ipDetailsFromProvider)
                    return StatusCode(500);

                ipDetails = _mapper.Map<IPDetail>(ipDetailsFromProvider);
                ipDetails.Ip = ip;
                await _iPBatchRepository.AddIPDetails(ipDetails);

                if (!await _iPBatchRepository.SaveChangesAsync())
                    return StatusCode(500);

                _memoryCache.Set(ip, ipDetails, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));

                var ipDetailsDtoToReturn = _mapper.Map<IPDetailDto>(ipDetails);

                /*#region debug
                using (StreamWriter fileStream = new("LogFile.txt", true))
                {
                    await fileStream.WriteLineAsync(ipDetails.ToString());
                }
                #endregion*/

                return Ok(ipDetailsDtoToReturn);
            }
            catch (IPServiceNotAvailableException)
            {
                return StatusCode(503);
            }
        }

        [HttpPost("update")]
        public IActionResult Update(List<IPDetailDto> ipDetailsDto)
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                Ips = ipDetailsDto.Select(x => x.Ip).ToList(),
                TotalItems = ipDetailsDto.Count,
                ProcessedItems = 0
            };

            // Console.WriteLine($"Created job with ID {job.Id}");

            jobs.Add(job.Id, job);
            Task.Run(() => ProcessJob(job));

            return Ok(job.Id);
        }

        private async void ProcessJob(Job job)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var iPBatchRepository = scope.ServiceProvider.GetRequiredService<IIPBatchRepository>();
                var batchSize = 10;

                while (job.Ips.Any())
                {
                    var batch = job.Ips.Take(batchSize).ToList();

                    foreach (var ip in batch)
                    {
                        IPDetail ipDetails;

                        if (_memoryCache.TryGetValue(ip, out IPDetail cachedIPDetails))
                        {
                            ipDetails = cachedIPDetails;

                            var updatedDetails = await _iPInfoProvider.GetDetails(ip);

                            if (updatedDetails is not null)
                            {
                                ipDetails = _mapper.Map(updatedDetails, ipDetails);
                                iPBatchRepository.UpdateIPDetails(ipDetails);
                                await iPBatchRepository.SaveChangesAsync();
                            }
                        }
                        else if (await iPBatchRepository.GetIPDetailsAsync(ip) is IPDetail ipDetailsFromDb)
                        {
                            ipDetails = ipDetailsFromDb;

                            var updatedDetails = await _iPInfoProvider.GetDetails(ip);

                            if (updatedDetails is not null)
                            {
                                ipDetails = _mapper.Map(updatedDetails, ipDetails);
                                iPBatchRepository.UpdateIPDetails(ipDetails);
                                await iPBatchRepository.SaveChangesAsync();
                            }
                        }
                        else if (await _iPInfoProvider.GetDetails(ip) is IPDetails ipDetailsFromProvider)
                        {
                            ipDetails = _mapper.Map<IPDetail>(ipDetailsFromProvider);
                            ipDetails.Ip = ip;
                            await iPBatchRepository.AddIPDetails(ipDetails);
                            await iPBatchRepository.SaveChangesAsync();
                        }
                        else
                        {
                            continue;
                        }

                        _memoryCache.Set(ip, ipDetails, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
                        job.ProcessedItems++;
                    }

                    job.Ips = job.Ips.Skip(batchSize).ToList();
                }
            }
        }

        [HttpGet("getProgress/{id}")]
        public IActionResult GetProgress(Guid id)
        {
            // Console.WriteLine($"Getting progress for job with ID {id}");

            if (jobs.ContainsKey(id))
            {
                var job = jobs[id];

                // Console.WriteLine($"Job with ID {id} has {job.ProcessedItems}/{job.TotalItems} processed items");

                return Ok($"{job.ProcessedItems}/{job.TotalItems}");
            }
            /*else
            {
                Console.WriteLine($"Job with ID {id} not found");
            }*/

            return NotFound();
        }
    }
}