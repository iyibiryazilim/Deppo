using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Sys.Service.Services;
using Deppo.Web.Helpers.MappingHelper;
using Deppo.Web.Services;
using Deppo.Web.ViewModels.SalesViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public class CustomerController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ICustomerService _customerService;

		public CustomerController(IHttpClientFactory httpClientFactory, ICustomerService customerService)
		{
			_httpClientFactory = httpClientFactory;
			_customerService = customerService;
			
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Detail(int referenceId)
		{
			var customer = await GetObjectByIdAsync(referenceId);
			CustomerDetailViewModel viewModel = new();

			if (customer is not null)
			{
				viewModel.Customer = customer;
			}

			return View(viewModel);
		}

		[HttpPost]
		public async Task<ActionResult> GetObjectsJsonResult([FromForm] int draw, [FromForm] int start, [FromForm] int length, [FromForm] string searchText)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

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

		private async Task<Customer> GetObjectByIdAsync(int referenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _customerService.GetObjectById(httpClient, 1, 2, referenceId);
				Customer customer= new();

				if (result.IsSuccess && result.Data != null)
				{
					foreach (var item in result.Data)
					{
						customer = Mapping.Mapper.Map<Customer>(item);

					}
					return customer;

				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				return null;
			}
		}

	}
}
