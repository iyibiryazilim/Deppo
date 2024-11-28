using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface IWaitingPurchaseProductService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);

	Task<DataResult<IEnumerable<dynamic>>> GetOrderById(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);
}
