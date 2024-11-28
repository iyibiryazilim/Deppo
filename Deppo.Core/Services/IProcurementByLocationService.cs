using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
	public interface IProcurementByLocationService
	{
		Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20);
	}
}
