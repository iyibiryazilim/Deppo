using Deppo.Core.DataResultModel;
using Deppo.Core.SortModels;
using Deppo.Mobile.Core.Models;
using System;
using Deppo.Core.DataResultModel;
using Deppo.Core.SortModels;
using Deppo.Mobile.Core.Models;

namespace Deppo.Core.Services;

public interface ICustomerService
{
    Task<DataResult<IEnumerable<Customer>>> GetObjects(HttpClient httpClient,string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber);

    Task<DataResult<Customer>> GetObjectById(HttpClient httpClient,int ReferenceId, int firmNumber);
}
