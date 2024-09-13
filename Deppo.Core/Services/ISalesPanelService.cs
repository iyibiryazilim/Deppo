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


}
