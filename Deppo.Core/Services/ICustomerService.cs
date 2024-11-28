using Deppo.Core.DataResultModel;
using Deppo.Core.SortModels;
using Deppo.Mobile.Core.Models;
using System;

namespace Deppo.Core.Services;

public interface ICustomerService
{
    // Task<DataResult<IEnumerable<Customer>>> GetObjects(HttpClient httpClient,string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber);

    // Task<DataResult<Customer>> GetObjectById(HttpClient httpClient,int ReferenceId, int firmNumber);

    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
	Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId);
}