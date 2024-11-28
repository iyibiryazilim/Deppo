using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ICustomerDetailInputProductService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20);
}
