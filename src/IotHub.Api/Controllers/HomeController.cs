using Microsoft.AspNetCore.Mvc;

namespace IotHub.Api.Controllers
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
			return Ok("IoT hub service");
		}
	}
}