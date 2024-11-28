using Deppo.Core.DataResultModel;

namespace Deppo.Mobile.Core.Services;

public interface IProcurementByProductBasketService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int[] itemsReferenceId, string search = "", int skip = 0, int take = 20);
}
