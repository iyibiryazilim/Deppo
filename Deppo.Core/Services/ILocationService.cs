using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ILocationService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber,int warehouseNumber,int productReferenceId, string search = "", int skip = 0, int take = 20);
}
