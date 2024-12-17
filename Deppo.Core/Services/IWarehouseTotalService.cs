using System;
using System.Threading.Tasks;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface IWarehouseTotalService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient,int firmNumber,int periodNumber,int warehouseNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");

    Task<DataResult<IEnumerable<dynamic>>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");
    Task<DataResult<IEnumerable<dynamic>>> TotalQuery(HttpClient httpClient, int firmNumber, int periodNumber,  string search = "", int skip = 0, int take = 20, string externalDb = "");
}
