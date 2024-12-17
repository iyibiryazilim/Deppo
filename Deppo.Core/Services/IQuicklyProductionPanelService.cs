using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IQuicklyProductionPanelService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetLastProductionTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string externalDb = "");

    public Task<DataResult<IEnumerable<dynamic>>> GetLastProductionFiches(HttpClient httpClient, int firmNumber, int periodNumber, string externalDb = "");

    public Task<DataResult<IEnumerable<dynamic>>> GetLastProducts(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetInProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<dynamic>> GetOutProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetQuicklyProductionInputProductListAsync(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);

    public Task<DataResult<IEnumerable<dynamic>>> GetQuicklyProductionOutputProductListAsync(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);

    public Task<DataResult<IEnumerable<dynamic>>> GetAllProductionFiches(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");

    Task<DataResult<IEnumerable<dynamic>>> GetProductionTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");

    public Task<DataResult<IEnumerable<dynamic>>> GetInputTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string externalDb = "");

    public Task<DataResult<IEnumerable<dynamic>>> GetOutputTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string externalDb = "");
}