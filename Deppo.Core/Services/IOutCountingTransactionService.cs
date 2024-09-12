using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.OutCountingTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IOutCountingTransactionService
    {
        Task<DataResult<ResponseModel>> InsertOutCountingTransaction(HttpClient httpClient, OutCountingTransactionInsert dto, int firmNumber);
    }
}
