using Deppo.Sys.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	public class UserController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IApplicationUserService _applicationUserService;
		private readonly IAuthenticateSysService _authenticationSysService;

		public UserController(IHttpClientFactory httpClientFactory, IAuthenticateSysService authenticateSysService, IApplicationUserService applicationUserService)
		{
			_httpClientFactory = httpClientFactory;
			_authenticationSysService = authenticateSysService;
			_applicationUserService = applicationUserService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetObjectsJsonResult()
		{
			try
			{
				var httpClientSys = _httpClientFactory.CreateClient("sys");
				var token = await _authenticationSysService.AuthenticateAsync(httpClientSys, "Admin", "");

				httpClientSys.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var query = $"$expand=Image";
				var result = await _applicationUserService.GetAllAsync(httpClientSys, query);

				if (result.Any())
				{
					return Json(new { data = result });
				}
				else
				{
					return Json(new { success = false, message = "Kullanıcılar yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
			}
		}
	}
}
