using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IProductDetailActionService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetWarehouses(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetWaitingSalesOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetWaitingPurchaseOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetLocationTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int skip = 0, int take = 20, string search = "");
        Task<DataResult<IEnumerable<dynamic>>> GetVariantTotals(HttpClient httpClient, int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetApprovedSuppliers(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);
		Task<DataResult<IEnumerable<dynamic>>> GetAlternativeProducts(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);


	}
}
