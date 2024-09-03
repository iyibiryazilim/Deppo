using Deppo.Core.DataResultModel;
using System;

namespace Deppo.Core.Services;

public interface IWaitingSalesOrderService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
}
