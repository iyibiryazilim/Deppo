using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IShipAddressService
{
	public Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int currentReferenceId, string search = "", int skip = 0, int take = 20);

    public Task<DataResult<IEnumerable<dynamic>>> GetObjectsByOrder(HttpClient httpClient, int firmNumber, int periodNumber, int currentReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");
}
