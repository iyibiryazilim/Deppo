using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface ITransferTransactionService
    {
        Task<DataResult<ResponseModel>> InsertTransferTransaction(HttpClient httpClient, TransferTransactionInsert dto, int firmNumber);
    }
}
