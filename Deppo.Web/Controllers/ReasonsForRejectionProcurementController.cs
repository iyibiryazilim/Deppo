using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Deppo.Web.Controllers
{
	public class ReasonsForRejectionProcurementController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IReasonsForRejectionProcurementService _reasonsForRejectionProcurementService;
		public ReasonsForRejectionProcurementController(IHttpClientFactory httpClientFactory , IReasonsForRejectionProcurementService reasonsForRejectionProcurementService)
		{
			_httpClientFactory = httpClientFactory;
			_reasonsForRejectionProcurementService = reasonsForRejectionProcurementService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetObjectsJsonResult()
		{
			try
			{
				var httpClientSys = _httpClientFactory.CreateClient("sys");

				var result = await _reasonsForRejectionProcurementService.GetAllAsync(httpClientSys);

				if (result.Any())
				{
					return Json(new { data = result });
				}
				else
				{
					return Json(new { success = false, message = "Red nedenleri yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
			}
		}

		[HttpPost]
		public async Task<ActionResult> CreateReasonForRejectionProcurement(ReasonsForRejectionDto dto)
		{
			try
			{
				var httpClientSys = _httpClientFactory.CreateClient("sys");


				var response = await _reasonsForRejectionProcurementService.CreateAsync(httpClientSys, dto);

				if (response != null)
				{
					return Json(new { success = true, message = "Red Nedeni başarıyla oluşturuldu." });
				}
				else
				{
					return Json(new { success = false, message = "Red Nedeni oluşturulamadı. Hata: " });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
			}
		}
	}
}
