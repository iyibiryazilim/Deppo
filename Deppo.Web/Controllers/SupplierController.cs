using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Deppo.Web.ViewModels.PurchaseViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deppo.Web.Controllers
{
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public class SupplierController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ISupplierService _supplierService;

		public SupplierController(IHttpClientFactory httpClientFactory, ISupplierService supplierService)
		{
			_httpClientFactory = httpClientFactory;
			_supplierService = supplierService;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Detail(int referenceId)
		{
			var supplier = await GetObjectByIdAsync(referenceId);
			SupplierDetailViewModel viewModel = new();

			if (supplier is not null)
			{
				viewModel.Supplier = supplier;
			}

			return View(viewModel);
		}

		[HttpPost]
		public async Task<ActionResult> GetObjectsJsonResult([FromForm] int draw, [FromForm] int start, [FromForm] int length, [FromForm] string searchText)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _supplierService.GetObjects(httpClient, 1, 2, searchText, start, length);

				if (result.IsSuccess && result.Data != null)
				{
					var mappedProducts = Mapping.Mapper.Map<IEnumerable<Supplier>>(result.Data);
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

		private async Task<Supplier> GetObjectByIdAsync(int referenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _supplierService.GetObjectById(httpClient, 1, 2, referenceId);
				Supplier supplier = new();

				if (result.IsSuccess && result.Data != null)
				{
					foreach (var item in result.Data)
					{
						supplier = Mapping.Mapper.Map<Supplier>(item);

					}
					return supplier;

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
