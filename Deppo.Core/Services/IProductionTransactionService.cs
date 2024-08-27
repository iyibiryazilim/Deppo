using System;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs;
using Deppo.Core.ResponseResultModels;

namespace Deppo.Core.Services;

public interface IProductionTransactionService
{
    Task<DataResult<ResponseModel>> InsertProductionTransaction(ProductionTransactionInsertDto dto);
}
