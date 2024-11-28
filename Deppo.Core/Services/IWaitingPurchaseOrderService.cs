using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IWaitingPurchaseOrderService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjectsBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int supplierReferenceId, int productReferenceId, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetSuppliers(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
	Task<DataResult<IEnumerable<dynamic>>> GetObjectsBySupplierAndShipInfo(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, int shipInfoReferenceId = 0,  string search = "", int skip = 0, int take = 20);

}