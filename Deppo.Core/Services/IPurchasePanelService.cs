using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IPurchasePanelService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetLastTransactionBySupplier(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<IEnumerable<dynamic>>> SupplierTransaction(HttpClient httpClient, int firmNumber, int periodNumber,int ficheReferenceId);
        Task<DataResult<IEnumerable<dynamic>>> GetLastFiche(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> TotalOrderCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> ShippedOrderCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<IEnumerable<dynamic>>> GetWaitingProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetWaitingOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetReceivedProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetReceivedOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetAllFiche(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetFicheTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20);

        Task<DataResult<dynamic>> WaitingProductCount(HttpClient httpClient, int firmNumber, int periodNumber);
    }
}
