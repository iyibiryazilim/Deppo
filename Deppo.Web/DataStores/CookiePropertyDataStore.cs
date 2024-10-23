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
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Session != null)
            {
                
                model.Token = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
                model.Username = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Authentication).Value;

            }

            return model;
        }
    }
}
