using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.ResponseResultModels;

namespace Deppo.Core.Services
{
    public interface ITransferTransactionService
    {
        Task<DataResult<ResponseModel>> InsertTransferTransaction(HttpClient httpClient, TransferTransactionInsert dto, int firmNumber);
        Task<DataResult<dynamic>> UpdateDocumentTrackingNumber(HttpClient httpClient, int firmNumber, int periodNumber,string ficheNumber, int ficheReferenceId);
        Task<DataResult<ResponseModel>> DeleteTransferTransaction(HttpClient httpClient, int referenceId, int firmNumber);
    }
}
