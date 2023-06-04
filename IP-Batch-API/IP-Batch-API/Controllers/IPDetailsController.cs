using IPInfoProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IP_Batch_API.Controllers
{
    [ApiController]
    [Route("api/ipdetails")]
    public class IPDetailsController : ControllerBase
    {
        private readonly IIPInfoProvider _iPInfoProvider;

        public IPDetailsController(IIPInfoProvider iPInfoProvider)
        {
            _iPInfoProvider = iPInfoProvider ?? throw new ArgumentNullException(nameof(iPInfoProvider));
        }

        [HttpGet("{ip}")]
        public async Task<IActionResult> GetDetails(string ip)
        {
            try
            {
                var cache = HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

                if (!cache.TryGetValue(ip, out var ipDetails))
                {
                    ipDetails = await _iPInfoProvider.GetDetails(ip);

                    var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

                    cache.Set(ip, ipDetails, cacheOptions);
                }

                /*#region debug
                using (StreamWriter fileStream = new("LogFile.txt", true))
                {
                    await fileStream.WriteLineAsync(ipDetails.ToString());
                }
                #endregion*/

                return Ok(ipDetails);
            }
            catch (IPServiceNotAvailableException)
            {
                return StatusCode(503);
            }
        }
    }
}