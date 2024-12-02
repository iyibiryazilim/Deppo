using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services;

public interface ISubunitsetService
{
    public Task<IEnumerable<SubUnitset>> GetAllAsync(HttpClient httpClient);

    public Task<IEnumerable<SubUnitset>> GetAllAsync(HttpClient httpClient, string filter);

    public Task<SubUnitset> CreateAsync(HttpClient httpClient, SubUnitsetDto dto);

}
