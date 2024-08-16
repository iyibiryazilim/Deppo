using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;
using System;

namespace Deppo.Core.Services;

public interface IWarehouseService
{
	Task<DataResult<IEnumerable<Warehouse>>> GetObjects(HttpClient httpClient, string search, SortModel? orderBy, int page, int pageSize, int firmNumber);
}
