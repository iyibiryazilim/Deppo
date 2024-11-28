using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface IPurchaseReturnDispatchTransactionService
{
    Task<DataResult<ResponseModel>> InsertPurchaseReturnDispatchTransaction(HttpClient httpClient, int firmNumber, PurchaseReturnDispatchTransactionInsert dto);
}
