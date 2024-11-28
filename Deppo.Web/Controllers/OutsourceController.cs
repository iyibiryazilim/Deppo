using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Web.Helpers.MappingHelper;
using Deppo.Web.Services;
using Deppo.Web.ViewModels.OutsourceViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public class OutsourceController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOutsourceService _outsourceService;

		public OutsourceController(
			IHttpClientFactory httpClientFactory,
			IOutsourceService outsourceService)
		{
			_httpClientFactory = httpClientFactory;
			_outsourceService = outsourceService;
		}

		public ActionResult Index()
        {
            return View();
        }

		public async Task<IActionResult> Detail(int referenceId)
		{
			var outsource = await GetObjectByIdAsync(referenceId);
			OutsourceDetailViewModel viewModel = new();

			if (outsource is not null)
			{
				viewModel.Outsource = outsource;
			}

			return View(viewModel);
		}

		[HttpPost]
        public async Task<ActionResult> GetObjectsJsonResult([FromForm] int draw, [FromForm] int start, [FromForm] int length, [FromForm] string searchText)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("helix");

                var result = await _outsourceService.GetObjects(httpClient, 1, 2, searchText, start, length);

                if (result.IsSuccess && result.Data != null)
                {
                    var mappedProducts = Mapping.Mapper.Map<IEnumerable<Outsource>>(result.Data);
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

		private async Task<Outsource> GetObjectByIdAsync(int referenceId)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("helix");

				var result = await _outsourceService.GetObjectById(httpClient, 1, 2, referenceId);
				Outsource outsource = new();

				if (result.IsSuccess && result.Data != null)
				{
					foreach (var item in result.Data)
					{
						outsource = Mapping.Mapper.Map<Outsource>(item);

					}
					return outsource;

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
