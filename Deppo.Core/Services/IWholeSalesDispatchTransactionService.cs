using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface IWholeSalesDispatchTransactionService
{
    Task<DataResult<ResponseModel>> InsertWholeSalesDispatchTransaction(HttpClient httpClient, int firmNumber, WholeSalesDispatchTransactionInsert dto);
}
