using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.WholeSalesReturnDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IWholeSalesReturnDispatchTransactionService
    {
        Task<DataResult<ResponseModel>> InsertWholeSalesReturnDispatchTransaction(HttpClient httpClient, int firmNumber, WholeSalesReturnDispatchTransactionInsert dto);
    }
}
