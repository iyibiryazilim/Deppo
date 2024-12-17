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
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber,int supplierReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");

    Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFicheReferenceId(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");

}
