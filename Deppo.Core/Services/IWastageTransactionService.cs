using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.WastageTransaction;
using Deppo.Core.ResponseResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IWastageTransactionService
    {
        Task<DataResult<ResponseModel>> InsertWastageTransaction(HttpClient httpClient, WastageTransactionInsert dto, int firmNumber);
    }
}
