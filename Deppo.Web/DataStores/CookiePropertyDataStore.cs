using Deppo.Web.Models;
using Deppo.Web.Services;
using System.Security.Claims;

namespace Deppo.Web.DataStores
{
    public class CookiePropertyDataStore : ICookiePropertyService
    {
        IHttpContextAccessor _httpContextAccessor;
        public CookiePropertyDataStore(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;


        }
		public CookieModel GetCookieModel()
		{
			CookieModel model = new CookieModel();

			if (_httpContextAccessor != null &&
				_httpContextAccessor.HttpContext != null &&
				_httpContextAccessor.HttpContext.Session != null)
			{
				var userClaims = _httpContextAccessor.HttpContext.User.Claims;

				var tokenClaim = userClaims.FirstOrDefault(c => c.Type == "Token");
				var usernameClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name); 

				if (tokenClaim != null)
				{
					model.Token = tokenClaim.Value;
				}
				else
				{
					model.Token = string.Empty; 
				}

				if (usernameClaim != null)
				{
					model.Username = usernameClaim.Value;
				}
				else
				{
					model.Username = string.Empty; 
				}
			}

			return model;
		}

	}
}
