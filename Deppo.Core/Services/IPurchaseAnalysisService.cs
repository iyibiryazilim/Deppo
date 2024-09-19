using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IPurchaseAnalysisService
    {
        Task<DataResult<dynamic>> DueDatePassedSuppliersCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> DueDatePassedProductsCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> PurchaseProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> ReturnProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<IEnumerable<dynamic>>> PurchaseProductReferenceAnalysis(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime);
        Task<DataResult<IEnumerable<dynamic>>> LastSuppliers(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<IEnumerable<dynamic>>> LastProducts(HttpClient httpClient, int firmNumber, int periodNumber);
    }
}
