using Deppo.Sys.Service.Services;
using Deppo.Web.Models;
using Deppo.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Deppo.Web.Controllers
{
	public class AuthenticationController : Controller
	{
		ILogger<AuthenticationController> _logger;
		IHttpClientFactory _httpClientFactory;
		IAuthenticateSysService _authenticateSysService;
		IApplicationUserService _applicationUserService;
		IHttpContextAccessor _httpContextAccessor;
		ICookiePropertyService _cookiePropertyService;
		protected string _token;
		public AuthenticationController(ILogger<AuthenticationController> logger, IHttpClientFactory httpClientFactory, IAuthenticateSysService authenticateSysService, IApplicationUserService applicationUserService, IHttpContextAccessor httpContextAccessor, ICookiePropertyService cookiePropertyService)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_authenticateSysService = authenticateSysService;
			_applicationUserService = applicationUserService;
			_token = string.Empty;
			_httpContextAccessor = httpContextAccessor;
			_cookiePropertyService = cookiePropertyService;

		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> LogOut()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Authenticate(string username, string password)
		{
			return Ok(Authentication(username, password).Result);
		}

		private async Task<AuthenticationResultModel> Authentication(string username, string password)
		{
			AuthenticationResultModel authResult = new AuthenticationResultModel();
			authResult.Result = false;
			authResult.IsFirst = false;
			var httpClient = _httpClientFactory.CreateClient("sys");

			var authenticationResult = await _authenticateSysService.AuthenticateAsync(httpClient, username, password);

			if (!string.IsNullOrEmpty(authenticationResult))
			{
				authResult.Result = true;
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult);

				var applicationUser = await _applicationUserService.GetAllAsync(httpClient, $"filter = UserName eq '{username}'");
				if (applicationUser != null)
				{

					foreach (var item in applicationUser)
					{

						ViewData["UserFullName"] = string.Format("{0} {1}", item.FirstName, item.LastName);
						ViewData["UserPosition"] = "Kullanıcı";
						ViewData["UserEMail"] = item.EMail;

						//if ((bool)item.ChangePasswordOnFirstLogon)
						//{
						//    authResult.IsFirst = true;
						//}


						var claims = new List<Claim>
						{
							new Claim("Token",authenticationResult),
							new Claim(ClaimTypes.Authentication,item.UserName),
							new Claim(ClaimTypes.Name,item.FirstName == null ? "" : item.FirstName),
							new Claim(ClaimTypes.Surname,item.LastName == null ? "" : item.LastName),
							new Claim(ClaimTypes.Email,item.EMail == null ? "" : item.EMail),
							new Claim(ClaimTypes.Actor,"Kullanıcı"),
						 
						};


						var userIdentity = new ClaimsIdentity(claims, "Login");

						ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

						//if (applicationUser.Image != null)
						//{
						//	_httpContextAccessor.HttpContext.Session.Set("UserImage", applicationUser.Image.MediaData);
						//	ViewData["UserImage"] = applicationUser.Image.MediaData;
						//}

						await _httpContextAccessor.HttpContext.SignInAsync(principal);
						authResult.Result = true;
					}



				}
			}

			return authResult;
		}
	}
}
