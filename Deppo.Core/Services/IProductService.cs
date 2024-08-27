using System;
using Deppo.Core.BaseModels;
using Deppo.Core.DataResultModel;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface IProductService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient,int firmNumber,int periodNumber, string search = "", int skip = 0, int take = 20 );

}
