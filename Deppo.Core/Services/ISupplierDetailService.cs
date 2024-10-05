using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface ISupplierDetailService
{
	public Task<DataResult<IEnumerable<dynamic>>> GetLastFichesBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId);
	public Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheRefenceId);
}
