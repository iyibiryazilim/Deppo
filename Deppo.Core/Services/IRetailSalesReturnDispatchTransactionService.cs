using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.RetailSalesReturnDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IRetailSalesReturnDispatchTransactionService
    {
        Task<DataResult<ResponseModel>> InsertRetailSalesReturnDispatchTransaction(HttpClient httpClient, int firmNumber, RetailSalesReturnDispatchTransactionInsert dto);
    }
}
