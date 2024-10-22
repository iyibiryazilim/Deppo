using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	public class NegativeProductController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly INegativeProductService _negativeProductService;
		private readonly IAuthenticationService _authenticationService;

		public NegativeProductController(
			IHttpClientFactory httpClientFactory,
			INegativeProductService negativeProductService,
			IAuthenticationService authenticationService)
		{
			_httpClientFactory = httpClientFactory;
			_negativeProductService = negativeProductService;
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

				var result = await _negativeProductService.GetObjects(httpClient, 1, 2, searchText, start, length);

				if (result.IsSuccess && result.Data != null)
				{
					var mappedProducts = Mapping.Mapper.Map<IEnumerable<NegativeProduct>>(result.Data);
					return Json(new { draw, data = mappedProducts });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
			}
		}
	}
}
