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

    public Task<DataResult<dynamic>> GetCountingTotalProductCount(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetInProducts(HttpClient httpClient, int firmNumber, int periodNumber, int skip = 0, int take = 20);

    public Task<DataResult<dynamic>> GetInCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber , int productReferenceId);

    public  Task<DataResult<IEnumerable<dynamic>>> GetOutProducts(HttpClient httpClient, int firmNumber, int periodNumber, int skip = 0, int take = 20);

    public Task<DataResult<dynamic>> GetOutCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

    public Task<DataResult<IEnumerable<dynamic>>> GetAllCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber,int ficheReferenceId , int skip = 0, int take = 20);

    public Task<DataResult<IEnumerable<dynamic>>> GetCountingFiches(HttpClient httpClient, int firmNumber, int periodNumber, int skip = 0, int take = 20);

}
