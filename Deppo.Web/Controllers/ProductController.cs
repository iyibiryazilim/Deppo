using Deppo.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Deppo.Web.Controllers;

public class ProductController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProductService _productService;

    public ProductController(IHttpClientFactory httpClientFactory, IProductService productService)
    {
        _httpClientFactory = httpClientFactory;
        _productService = productService;
    }

    public ActionResult Index()
    {
        return View();
    }


}

