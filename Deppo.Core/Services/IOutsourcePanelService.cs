using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IOutsourcePanelService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetLastOutsourceTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastOutsourceFiches(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastOutsources(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetOutsourceInProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetOutsourceOutProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetOutsourceTotalProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetAllOutsourceFiches(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
}