using System;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface IWarehouseTotalService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient,int firmNumber,int periodNumber,int warehouseNumber, string search = "", int skip = 0, int take = 20 );
}
