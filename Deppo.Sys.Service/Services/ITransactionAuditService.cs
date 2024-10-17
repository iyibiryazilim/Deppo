using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services;

public interface ITransactionAuditService
{
    public Task<IEnumerable<TransactionAudit>> GetAllAsync(HttpClient httpClient);

    public Task<IEnumerable<TransactionAudit>> GetAllAsync(HttpClient httpClient, string filter);

    public Task<TransactionAudit> CreateAsync(HttpClient httpClient, TransactionAuditDto dto);
}