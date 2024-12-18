using Deppo.Core.DataResultModel;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.Services;

public interface IProductPanelService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetLastProducts(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastTransactions(HttpClient httpClient, int firmNumber, int periodNumber,int ficheReferenceId, string externalDb = "");


    public Task<DataResult<IEnumerable<dynamic>>> GetLastWarehouses(HttpClient httpClient, int firmNumber, int periodNumber, string externalDb = "");

    public Task<DataResult<dynamic>> GetInputProductQuantity(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetLastFiche(HttpClient httpClient, int firmNumber, int periodNumber, string externalDb = "");

    public Task<DataResult<dynamic>> GetOutputProductQuantity(HttpClient httpClient, int firmNumber, int periodNumber);

    public Task<DataResult<IEnumerable<dynamic>>> GetInputProduct(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
	public Task<DataResult<IEnumerable<dynamic>>> GetOutputProduct(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);


    public  Task<DataResult<IEnumerable<dynamic>>> GetOutputTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string externalDb = "");
    public Task<DataResult<IEnumerable<dynamic>>> GetInputTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string externalDb = "");

	public Task<DataResult<IEnumerable<dynamic>>> GetAllFiches(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");
	public Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int skip = 0, int take = 20, string externalDb = "");

}
