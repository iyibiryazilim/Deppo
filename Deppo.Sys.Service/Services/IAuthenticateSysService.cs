using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IAuthenticateSysService
    {
        public Task<string> AuthenticateAsync(HttpClient httpClient, string username, string password);
    }
}