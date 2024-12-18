using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
	public interface IOverviewAnalysisTransactionService
	{
		Task<DataResult<IEnumerable<dynamic>>> GetTransactionByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");
	}
}
