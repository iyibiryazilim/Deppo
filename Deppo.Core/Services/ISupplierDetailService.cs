using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface ISupplierDetailService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetLastFichesBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string externalDb = "");

    public Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheRefenceId, string externalDb = "");

    Task<DataResult<dynamic>> GetInputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId);

    Task<DataResult<dynamic>> GetOutputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId);

    Task<DataResult<IEnumerable<dynamic>>> SupplierInputOutputQuantities(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int supplierReferenceId);

    Task<DataResult<dynamic>> GetWaitingProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId);
}