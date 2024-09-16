using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services; 

public interface IPurchaseDispatchTransactionService
{
    Task<DataResult<ResponseModel>> InsertPurchaseDispatchTransaction(HttpClient httpClient, int firmNumber, PurchaseDispatchTransactionInsert dto);


}
