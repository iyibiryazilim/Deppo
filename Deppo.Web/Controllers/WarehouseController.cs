using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Deppo.Web.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public class WarehouseController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IWarehouseService _warehouseService;

		public WarehouseController(
			IHttpClientFactory httpClientFactory,
			IWarehouseService warehouseService)
		{
			_httpClientFactory = httpClientFactory;
			_warehouseService = warehouseService;
		}

		public ActionResult Index()
		{
			return View();
		}

		public async Task<ActionResult> Detail(int number)
		{
			var warehouse = await GetObjectByIdAsync(number);
			WarehouseDetailViewModel viewModel = new();

			if (warehouse is not null)
			{
				viewModel.Warehouse = warehouse;
			}

			return View(viewModel);
		}

		[HttpPost]
		public async Task<ActionResult> GetObjectsJsonResult([FromForm] int draw, [FromForm] int start, [FromForm] int length, [FromForm] string searchText)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _warehouseService.GetObjectsAsync(httpClient,1,2, searchText, start, length);

				if (result.IsSuccess && result.Data != null)
				{
					var mappedProducts = Mapping.Mapper.Map<IEnumerable<Warehouse>>(result.Data);
					return Json(new { draw, data = mappedProducts });
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

		private async Task<Warehouse> GetObjectByIdAsync(int number)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _warehouseService.GetObjectById(httpClient, 1, 2, number);
				Warehouse warehouse = new();

				if (result.IsSuccess && result.Data != null)
				{
					foreach (var item in result.Data)
					{
						warehouse = Mapping.Mapper.Map<Warehouse>(item);

					}
					return warehouse;

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
