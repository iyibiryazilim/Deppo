using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IVariantService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber,int productReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20 );

}
