using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ICountingPanelService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetLastCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastCountingFiches(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastProducts(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetCountingInProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetCountingOutProductCount(HttpClient httpClient, int firmNumber, int periodNumber);
}
