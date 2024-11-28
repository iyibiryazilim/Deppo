using System;
using Deppo.Core.BaseModels;
using Deppo.Core.DataResultModel;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface IProductService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);

    Task<DataResult<IEnumerable<dynamic>>> GetObjectsPurchaseProduct(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
	Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

}