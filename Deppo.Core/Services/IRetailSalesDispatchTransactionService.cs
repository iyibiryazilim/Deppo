using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IRetailSalesDispatchTransactionService
    {
        Task<DataResult<ResponseModel>> InsertRetailSalesDispatchTransaction(HttpClient httpClient, int firmNumber, RetailSalesDispatchTransactionInsert dto);
    }
}
