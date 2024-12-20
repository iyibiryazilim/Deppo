using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IPackageProductService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);

}
