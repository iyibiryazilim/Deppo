using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface ISalesPanelService
{
    Task<DataResult<IEnumerable<dynamic>>> GetLastTransactionByCustomer(HttpClient httpClient , int firmNumber, int periodNumber);
    Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient , int firmNumber, int periodNumber,int ficheReferenceId);

    Task<DataResult<IEnumerable<dynamic>>> GetLastFiche(HttpClient httpClient, int firmNumber, int periodNumber);

    Task<DataResult<dynamic>> TotalOrderCount(HttpClient httpClient, int firmNumber, int periodNumber);
    Task<DataResult<dynamic>> ShippedOrderCount(HttpClient httpClient , int firmNumber, int periodNumber);
    Task<DataResult<IEnumerable<dynamic>>> GetWaitingProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetWaitingOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);

    Task<DataResult<IEnumerable<dynamic>>> GetShippedProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetShippedOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);

    Task<DataResult<IEnumerable<dynamic>>> GetAllFiche(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
    Task<DataResult<IEnumerable<dynamic>>> GetFicheTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20);

    Task<DataResult<dynamic>> WaitingProductCount(HttpClient httpClient, int firmNumber, int periodNumber);



}
