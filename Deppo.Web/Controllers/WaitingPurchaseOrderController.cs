using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public class WaitingPurchaseOrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;

        public WaitingPurchaseOrderController(IHttpClientFactory httpClientFactory, IWaitingPurchaseOrderService waitingPurchaseOrderService)
        {
            _httpClientFactory = httpClientFactory;
            _waitingPurchaseOrderService = waitingPurchaseOrderService;
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

                var result = await _waitingPurchaseOrderService.GetObjects(httpClient, 1, 2, searchText, start, length);

                if (result.IsSuccess && result.Data != null)
                {
                    var mappedProducts = Mapping.Mapper.Map<IEnumerable<WaitingPurchaseOrder>>(result.Data);
                    return Json(new { draw, data = mappedProducts });
                }
                else
                {
                    return Json(new { success = false, message = result.Message ?? "Bekleyen Satınalma Siparişleri yüklenemedi." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
    }
}
