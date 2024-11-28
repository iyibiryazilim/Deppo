using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface ICustomerDetailOutputProductService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20);
}
