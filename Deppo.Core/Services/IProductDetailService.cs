using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IProductDetailService
    {
        public Task<DataResult<IEnumerable<dynamic>>> GetLastFichesByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int productReferenceId);

        Task<DataResult<dynamic>> GetProductMeasure(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<IEnumerable<dynamic>>> ProductInputOutputQuantities(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int productReferenceId);

        Task<DataResult<dynamic>> GetInputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<dynamic>> GetOutputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<dynamic>> GetFirstStockQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<dynamic>> GetSalesQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<dynamic>> GetLastStockQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<dynamic>> GetPurchaseQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int month);

        Task<DataResult<dynamic>> UpdateProductMeasure(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, double width, double height, double length, double weight, double volume, int subunitsetId);
    }
}