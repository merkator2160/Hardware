using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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
			return Ok($"IoT hub v{Assembly.GetExecutingAssembly().GetName().Version}");
		}
	}
}