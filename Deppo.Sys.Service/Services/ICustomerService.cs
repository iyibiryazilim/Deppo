using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services;

public interface ICustomerService
{
    public Task<IEnumerable<Customer>> GetAllAsync(HttpClient httpClient);

    public Task<IEnumerable<Customer>> GetAllAsync(HttpClient httpClient, string filter);

    public Task<Customer> CreateAsync(HttpClient httpClient, CustomerDto dto);

}
