using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface ISalesAnalysisService
    {
        Task<DataResult<dynamic>> DueDatePassedCustomersCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> DueDatePassedProductsCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> SoldProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> ReturnProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<IEnumerable<dynamic>>> SalesProductReferenceAnalysis(HttpClient httpClient, int firmNumber, int periodNumber,DateTime dateTime);
        Task<DataResult<IEnumerable<dynamic>>> LastCustomers(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<IEnumerable<dynamic>>> LastProducts(HttpClient httpClient, int firmNumber, int periodNumber);
       
    }
}
