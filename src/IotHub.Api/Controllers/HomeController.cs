using Microsoft.AspNetCore.Mvc;

namespace DenverTraffic.FingerprintBuilder.Api.Controllers
{
	[ApiController]
#if !DEBUG
	[ApiExplorerSettings(IgnoreApi = true)]
#endif
	public class HomeController : ControllerBase
	{
		[HttpGet("/")]
		public IActionResult Index()
		{
			return Ok("Fingerprint builder service");
		}
	}
}