using Deppo.Core.BaseModels;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers;

public class ProductController : Controller
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IProductService _productService;
	private readonly IAuthenticationService _authenticationService;

	public ProductController(IHttpClientFactory httpClientFactory, IProductService productService, IAuthenticationService authenticationService)
	{
		_httpClientFactory = httpClientFactory;
		_productService = productService;
		_authenticationService = authenticationService;
	}

	public ActionResult Index()
	{
		return View();	
	}

	[HttpPost]
	public async Task<JsonResult> GetProductJsonResult()
	{
		var products = new List<Product>();

		try
		{
			var httpClient = _httpClientFactory.CreateClient("helix");
			var token = await _authenticationService.Authenticate(httpClient, "Admin", "");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await _productService.GetObjects(httpClient, 1, 2, string.Empty, 0, 9999999);


			if (result.IsSuccess && result.Data != null)
			{
				products.AddRange(Mapping.Mapper.Map<IEnumerable<Product>>(result.Data));
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

		return Json(new { success = true, data = products });
	}


}


