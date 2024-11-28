using Deppo.Core.BaseModels;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;
using System;

namespace Deppo.Core.Services;

public interface ISupplierService
{
    // Task<DataResult<IEnumerable<Supplier>>> GetObjects(HttpClient httpClient, string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber);

    // Task<DataResult<Supplier>> GetObjectById(HttpClient httpClient, int ReferenceId, int firmNumber);

    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
	Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId);

}