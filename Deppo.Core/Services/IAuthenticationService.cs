using System;

namespace Deppo.Core.Services;

public interface IAuthenticationService
{
    public Task<string> Authenticate(HttpClient httpClient, string username, string password);
}
