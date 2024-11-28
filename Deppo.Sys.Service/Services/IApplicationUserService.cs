using System;
using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;

namespace Deppo.Sys.Service.Services;

public interface IApplicationUserService
{
    public Task<IEnumerable<ApplicationUser>> GetAllAsync(HttpClient httpClient);

    public Task<IEnumerable<ApplicationUser>> GetAllAsync(HttpClient httpClient, string filter);
	public Task<ApplicationUser> CreateAsync(HttpClient httpClient, ApplicationUserDto dto);
}
