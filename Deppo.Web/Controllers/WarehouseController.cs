using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	public class WarehouseController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IWarehouseService _warehouseService;
		private readonly IAuthenticationService _authenticationService;

		public WarehouseController(
			IHttpClientFactory httpClientFactory,
			IWarehouseService warehouseService,
			IAuthenticationService authenticationService)
		{
			_httpClientFactory = httpClientFactory;
			_warehouseService = warehouseService;
			_authenticationService = authenticationService;
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetObjectsJsonResult([FromForm] int draw, [FromForm] int start, [FromForm] int length, [FromForm] string searchText)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");
				var token = await _authenticationService.Authenticate(httpClient, "Admin", "");
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var result = await _warehouseService.GetObjects(httpClient, searchText, null, start, length, 1);

				if (result.IsSuccess && result.Data != null)
				{
					return Json(new { draw, data = result.Data });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ambarlar yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
			}
		}
	}
}
