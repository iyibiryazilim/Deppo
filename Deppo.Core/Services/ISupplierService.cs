using System;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface ISupplierService
{
    Task<DataResult<IEnumerable<Supplier>>> GetObjects(HttpClient httpClient,string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber);

    Task<DataResult<Supplier>> GetObjectById(HttpClient httpClient,int ReferenceId, int firmNumber);
}
