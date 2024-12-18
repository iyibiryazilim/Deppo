using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface ISupplierDetailActionService
{
	Task<DataResult<IEnumerable<dynamic>>> GetWaitingPurchaseOrdersBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");

	Task<DataResult<IEnumerable<dynamic>>> GetShipAddressesBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20);
	Task<DataResult<IEnumerable<dynamic>>> GetApprovedProductsBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20);
}
