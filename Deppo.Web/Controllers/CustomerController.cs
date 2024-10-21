using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	public class CustomerController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ICustomerService _customerService;
		private readonly IAuthenticationService _authenticationService;

		public CustomerController(IHttpClientFactory httpClientFactory, ICustomerService customerService, IAuthenticationService authenticationService)
		{
			_httpClientFactory = httpClientFactory;
			_customerService = customerService;
			_authenticationService = authenticationService;
		}

		public IActionResult Index()
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

				var result = await _customerService.GetObjects(httpClient, 1, 2, searchText, start, length);

				if (result.IsSuccess && result.Data != null)
				{
					var mappedProducts = Mapping.Mapper.Map<IEnumerable<Customer>>(result.Data);
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
