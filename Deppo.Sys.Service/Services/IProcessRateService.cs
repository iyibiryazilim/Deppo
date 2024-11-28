using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services;

public interface IProcessRateService
{
    public Task<IEnumerable<ProcessRate>> GetAllAsync(HttpClient httpClient);

    public Task<IEnumerable<ProcessRate>> GetAllAsync(HttpClient httpClient, string filter);
}