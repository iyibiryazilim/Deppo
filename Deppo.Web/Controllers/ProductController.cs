using Deppo.Core.BaseModels;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Deppo.Web.Models;
using Deppo.Web.Services;
using Deppo.Web.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Deppo.Web.Controllers
{
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public class ProductController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IProductService _productService;
		private readonly IProductDetailService _productDetailService;
		private readonly ILogger<ProductController> _logger;
		private readonly IProductDetailActionService _productDetailActionService;
		private readonly IVariantService _variantService;

		public ProductController(
			IHttpClientFactory httpClientFactory,
			IProductService productService,
			IProductDetailService productDetailService,
			ILogger<ProductController> logger,
			IProductDetailActionService productDetailActionService,
			IVariantService variantService)
		{
			_httpClientFactory = httpClientFactory;
			_productService = productService;
			_productDetailService = productDetailService;
			_logger = logger;
			_productDetailActionService = productDetailActionService;
			_variantService = variantService;
		}

		public async Task<ActionResult> Index()
		{
			return View();
		}

		public async Task<ActionResult> Detail(int referenceId)
		{
			var product = await GetObjectByIdAsync(referenceId);
			ProductDetailViewModel viewModel = new ProductDetailViewModel();

			if (product is not null)
			{
				viewModel.Product = product;
			}
			await Task.WhenAll(GetInputQuantityAsync(referenceId, viewModel), GetOutputQuantityAsync(referenceId, viewModel));


			return View(viewModel);
		}

		[HttpPost]
		public async Task<ActionResult> GetObjectsJsonResult([FromForm] int draw, [FromForm] int start, [FromForm] int length, [FromForm] string searchText)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productService.GetObjects(httpClient, 1, 2, searchText, start, length);

				if (result.IsSuccess && result.Data != null)
				{
					var mappedProducts = Mapping.Mapper.Map<IEnumerable<Product>>(result.Data);
					return Json(new { draw, data = mappedProducts });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

		private async Task<Product> GetObjectByIdAsync(int referenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productService.GetObjectById(httpClient, 1, 2, referenceId);
				Product product = new();

				if (result.IsSuccess && result.Data != null)
				{
					foreach (var item in result.Data)
					{
						product =  Mapping.Mapper.Map<Product>(item);

					}
					return product;

				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured while getting product by id");
				return null;
			}
		}

		private async Task GetInputQuantityAsync(int productReferenceId,ProductDetailViewModel viewModel)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");
				var result = await _productDetailService.GetInputQuantity(httpClient: httpClient, firmNumber: 1, periodNumber: 2, productReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is null)
						return;

					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<ProductDetailViewModel>(item);
						viewModel.InputQuantity = obj.InputQuantity;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured while getting product by id");

			}
			finally
			{
			}
		}

		private async Task GetOutputQuantityAsync(int productReferenceId, ProductDetailViewModel viewModel)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");
				var result = await _productDetailService.GetOutputQuantity(httpClient: httpClient, firmNumber:1, periodNumber: 2, productReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is null)
						return;

					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<ProductDetailViewModel>(item);
						viewModel.OutputQuantity = obj.OutputQuantity;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured while getting product by id");

			}
			finally
			{
			}
		}

		[HttpPost]
        public async Task<ActionResult> GetWarehouseTotals(int productReferenceId)
        {
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productDetailActionService.GetWarehouses(httpClient, 1, 2, productReferenceId, string.Empty, 0, 2000);

				if (result.IsSuccess && result.Data != null)
				{
					var warehouses = Mapping.Mapper.Map<IEnumerable<Warehouse>>(result.Data);
					return Json(new { data = warehouses });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

		public async Task<ActionResult> GetWaitingPurchaseOrders(int productReferenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productDetailActionService.GetWaitingPurchaseOrders(httpClient, 1, 2, productReferenceId, string.Empty, 0, 999999);

				if (result.IsSuccess && result.Data != null)
				{
					var purchaseOrders = Mapping.Mapper.Map<IEnumerable<WaitingPurchaseOrder>>(result.Data);
					return Json(new { data = purchaseOrders });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

		public async Task<ActionResult> GetWaitingSalesOrdersAsync(int productReferenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productDetailActionService.GetWaitingSalesOrders(httpClient, 1, 2, productReferenceId, string.Empty, 0, 999999);

				if (result.IsSuccess && result.Data != null)
				{
					var salesOrders = Mapping.Mapper.Map<IEnumerable<WaitingSalesOrder>>(result.Data);
					return Json(new { data = salesOrders });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

		public async Task<ActionResult> GetLocationTransactions(int productReferenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productDetailActionService.GetLocationTransactions(httpClient, 1, 2, productReferenceId, search:string.Empty,skip: 0, take:1000000);

				if (result.IsSuccess && result.Data != null)
				{
					var locationTransactions = Mapping.Mapper.Map<IEnumerable<LocationTransactionModel>>(result.Data);
					return Json(new { data = locationTransactions });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

		public async Task<ActionResult> GetApprovedSuppliers(int productReferenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productDetailActionService.GetApprovedSuppliers(httpClient, 1, 2, productReferenceId, search: string.Empty, skip: 0, take: 2000);

				if (result.IsSuccess && result.Data != null)
				{
					var currents = Mapping.Mapper.Map<IEnumerable<Current>>(result.Data);
					return Json(new { data = currents });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

		public async Task<ActionResult> GetVariantsAsync(int productReferenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _variantService.GetVariants(httpClient, 1, 2, productReferenceId, search: string.Empty, skip: 0, take: 2000);

				if (result.IsSuccess && result.Data != null)
				{
					var variants = Mapping.Mapper.Map<IEnumerable<Variant>>(result.Data);
					return Json(new { data = variants });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

		public async Task<ActionResult> GetVariantTotalsAsync(int variantReferemceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _productDetailActionService.GetVariantTotals(httpClient, 1, 2, variantReferemceId, search: string.Empty, skip: 0, take: 2000);

				if (result.IsSuccess && result.Data != null)
				{
					var variantModels = Mapping.Mapper.Map<IEnumerable<Warehouse>>(result.Data);
					return Json(new { data = variantModels });
				}
				else
				{
					return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
			}
		}

        public async Task<ActionResult> GetProductMeasure(int productReferenceId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("helix");

                var result = await _productDetailService.GetProductMeasure(httpClient, 1, 2, productReferenceId);

                if (result.IsSuccess && result.Data != null)
                {
                    var variantModels = Mapping.Mapper.Map<IEnumerable<ProductMeasure>>(result.Data);
                    return Json(new { data = variantModels });
                }
                else
                {
                    return Json(new { success = false, message = result.Message ?? "Ürünler yüklenemedi." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bir hata oluþtu: {ex.Message}" });
            }
        }
    }
}
