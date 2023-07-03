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

        public IPDetailsController(IIPInfoProvider iPInfoProvider, IMemoryCache memoryCache, IMapper mapper, IIPBatchRepository iPBatchRepository)
        {
            _iPInfoProvider = iPInfoProvider ?? throw new ArgumentNullException(nameof(iPInfoProvider));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _iPBatchRepository = iPBatchRepository ?? throw new ArgumentNullException(nameof(iPBatchRepository));
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
    }
}