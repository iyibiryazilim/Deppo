using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IProcurementSalesProductService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<dynamic>> SearchByBarcode(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string barcode);
        Task<DataResult<dynamic>> SearchByItemCode(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string barcode);
    }
}
