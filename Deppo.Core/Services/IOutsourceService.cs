using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IOutsourceService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
	Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int outsourceReferenceId);
	Task<DataResult<IEnumerable<dynamic>>> GetOutsourceWarehousesAsync(HttpClient httpClient, int firmNumber, int periodNumber,string search = "", int skip = 0, int take = 20);
    

}
