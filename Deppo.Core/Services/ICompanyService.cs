using System;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;

namespace Deppo.Core.Services;

public interface ICompanyService
{
    Task<DataResult<IEnumerable<Company>>> GetObjectsAsync(HttpClient httpClien,string query);

}
