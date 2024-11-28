using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.ResponseResultModels;

namespace Deppo.Core.Services
{
	public interface IRetailSalesDispatchTransactionService
    {
        Task<DataResult<ResponseModel>> InsertRetailSalesDispatchTransaction(HttpClient httpClient, int firmNumber, RetailSalesDispatchTransactionInsert dto);

		public Task<DataResult<dynamic>> UpdateFicheStatus(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId,int transactionNumber, int isEDispatch);
		public Task<DataResult<ResponseModel>> UpdateFiche(HttpClient httpClient, int firmNumber,RetailSalesDispatchUpdateDto dto);

		public Task<DataResult<dynamic>> UpdateLine(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId, double quantity);
		public Task<DataResult<dynamic>> Delete(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId);
	}
}
