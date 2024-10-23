using Deppo.Sys.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	public class TransactionAuditController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ITransactionAuditService _transactionAuditService;
		private readonly IAuthenticateSysService _authenticationSysService;

		public TransactionAuditController(IHttpClientFactory httpClientFactory, IAuthenticateSysService authenticateSysService, ITransactionAuditService transactionAuditService)
		{
			_httpClientFactory = httpClientFactory;
			_authenticationSysService = authenticateSysService;
			_transactionAuditService = transactionAuditService;
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
				var token = await _authenticationSysService.AuthenticateAsync(httpClientSys, "Admin", "");

				httpClientSys.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var query = $"$expand=ApplicationUser";
				var result = await _transactionAuditService.GetAllAsync(httpClientSys, query);

				if (result.Any())
				{
					return Json(new { data = result });
				}
				else
				{
					return Json(new { success = false, message = "Kullanıcılar yüklenemedi." });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
			}
		}
	}
}
