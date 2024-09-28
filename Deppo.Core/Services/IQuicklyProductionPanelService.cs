using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IQuicklyProductionPanelService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetLastProductionTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastProductionFiches(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastProducts(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetInProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetOutProductCount(HttpClient httpClient, int firmNumber, int periodNumber);
}
