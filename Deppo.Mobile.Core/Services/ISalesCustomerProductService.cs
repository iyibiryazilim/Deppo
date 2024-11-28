using Deppo.Core.DataResultModel;

namespace Deppo.Mobile.Core.Services;

public interface ISalesCustomerProductService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20);
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int warehouseNumber, int shipInfoReferenceId, string search = "", int skip = 0, int take = 20);
}
