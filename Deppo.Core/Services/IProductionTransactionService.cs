using System;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.ResponseResultModels;

namespace Deppo.Core.Services;

public interface IProductionTransactionService
{
  //  Task<DataResult<ResponseModel>> InsertProductionTransactionv1(ProductionTransactionInsertDto dto);
    Task<DataResult<ResponseModel>> InsertProductionTransaction(HttpClient httpClient, ProductionTransactionInsert dto, int firmNumber);
}
