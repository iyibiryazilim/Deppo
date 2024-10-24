using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Deppo.Web.Controllers
{
	public class UserController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IApplicationUserService _applicationUserService;
		private readonly IAuthenticateSysService _authenticationSysService;
		private readonly IMediaDataObjectService _mediaDataObjectService;

		public UserController(IHttpClientFactory httpClientFactory, IAuthenticateSysService authenticateSysService, IApplicationUserService applicationUserService, IMediaDataObjectService mediaDataObjectService)
		{
			_httpClientFactory = httpClientFactory;
			_authenticationSysService = authenticateSysService;
			_applicationUserService = applicationUserService;
			_mediaDataObjectService = mediaDataObjectService;
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
				var query = $"$expand=Image";
				var result = await _applicationUserService.GetAllAsync(httpClientSys, query);

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

		[HttpPost]
		public async Task<ActionResult> CreateUser(ApplicationUserDto dto, MediaDataObjectDto mediaDataObjectDto, IFormFile avatar)
		{
			try
			{
				var httpClientSys = _httpClientFactory.CreateClient("sys");

				var token = await _authenticationSysService.AuthenticateAsync(httpClientSys, "Admin", "");
				httpClientSys.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				await CreateImage(dto, mediaDataObjectDto, avatar, httpClientSys);

				var response = await _applicationUserService.CreateAsync(httpClientSys, dto);

				if (response != null)
				{
					return Json(new { success = true, message = "Kullanıcı başarıyla oluşturuldu." });
				}
				else
				{
					return Json(new { success = false, message = "Kullanıcı oluşturulamadı. Hata: " });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
			}
		}

		private async Task CreateImage(ApplicationUserDto dto, MediaDataObjectDto mediaDataObjectDto, IFormFile avatar, HttpClient httpClientSys)
		{
			if (avatar != null && avatar.Length > 0)
			{
				try
				{
					using (var memoryStream = new MemoryStream())
					{
						await avatar.CopyToAsync(memoryStream);
						mediaDataObjectDto.MediaData = memoryStream.ToArray();
					}

					var imageResponse = await _mediaDataObjectService.CreateAsync(httpClientSys, mediaDataObjectDto);
					if (imageResponse != null)
					{
						dto.Image = imageResponse.Oid;
					}
				}
				catch (Exception imageEx)
				{
				}
			}
		}
	}
}
