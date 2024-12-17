using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ILocationService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber,int warehouseNumber,int productReferenceId,int variantReferenceId = 0,
        string search = "", int skip = 0, int take = 20, string externalDb = "");

    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");

    Task<DataResult<IEnumerable<dynamic>>> GetLocationsWithStock(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int productReferenceId, int variantReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "");
}
