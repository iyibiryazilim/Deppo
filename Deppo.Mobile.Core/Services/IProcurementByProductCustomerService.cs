using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Mobile.Core.Services;

public interface IProcurementByProductCustomerService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int productReferenceId, string search = "", int skip = 0, int take = 20);

}
