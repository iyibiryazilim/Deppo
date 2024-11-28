using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.Demand;
using Deppo.Core.ResponseResultModels;

namespace Deppo.Core.Services
{
    public interface IDemandService
    {
        Task<DataResult<ResponseModel>> DemandInsert(HttpClient httpClient, DemandInsert dto, int firmNumber);
    }
}
