using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ILocationTransactionService
{
	Task<DataResult<IEnumerable<dynamic>>> GetInputObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "", int locationRef = 0, int serilotRef = 0,int variantReferenceId = 0);

	Task<DataResult<IEnumerable<dynamic>>> GetOutputObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "", int locationRef = 0, int serilotRef = 0);

	Task<DataResult<IEnumerable<dynamic>>> GetLocationTransactionsInputObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "", int locationRef = 0, int serilotRef = 0, int variantReferenceId = 0);

	Task<DataResult<IEnumerable<dynamic>>> GetLineLocationTransactionsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int locationRef, int ficheReferenceId, int lineReferenceId , int skip = 0, int take = 20, string search = "");

	public Task<DataResult<dynamic>> Update(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId, double quantity);
	public Task<DataResult<dynamic>> Delete(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId);


}
