using System;
using Deppo.Core.BaseModels;
using Deppo.Core.DataResultModel;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface IProductService
{
    Task<DataResult<IEnumerable<Product>>> GetObjects(HttpClient httpClient,string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber);

    Task<DataResult<Product>> GetObjectById(HttpClient httpClient,int ReferenceId, int firmNumber);

    Task<DataResult<Product>> GetObjectByCode(HttpClient httpClient,string Code, int firmNumber);
}
