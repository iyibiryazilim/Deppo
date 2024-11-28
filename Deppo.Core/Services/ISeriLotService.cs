using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ISeriLotService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20);

    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20);
}
