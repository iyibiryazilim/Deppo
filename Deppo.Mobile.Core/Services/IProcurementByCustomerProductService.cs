using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Mobile.Core.Services;

public interface IProcurementByCustomerProductService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber,int orderWarehouseNumber,int customerReferenceId,string search = "", int skip = 0, int take = 20);

}
