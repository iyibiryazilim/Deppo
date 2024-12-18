using Deppo.Core.DataResultModel;
using System;

namespace Deppo.Core.Services;

public interface IWaitingSalesOrderService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjectsByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId,string search = "", int skip = 0, int take = 20, string externalDb = "");
	Task<DataResult<IEnumerable<dynamic>>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, int productReferenceId,int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "");
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "");
	Task<DataResult<IEnumerable<dynamic>>> GetCustomers(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);

	Task<DataResult<IEnumerable<dynamic>>> GetObjectsByCustomerAndShipInfo(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "");
}
