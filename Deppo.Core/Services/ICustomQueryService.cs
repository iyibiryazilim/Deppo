using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ICustomQueryService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, string query);
    public Task<DataResult<dynamic>> GetObjectAsync(HttpClient httpClient, string query);
}
