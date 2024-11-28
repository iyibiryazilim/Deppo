using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IConsumableTransactionService
    {
        Task<DataResult<ResponseModel>> InsertConsumableTransaction(HttpClient httpClient, ConsumableTransactionInsert dto, int firmNumber);
    }
}
