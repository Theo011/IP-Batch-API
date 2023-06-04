using IPInfoProvider;
using Microsoft.AspNetCore.Mvc;

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
                var ipDetails = await _iPInfoProvider.GetDetails(ip);

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