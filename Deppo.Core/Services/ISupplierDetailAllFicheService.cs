using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ISupplierDetailAllFicheService
{

    Task<DataResult<IEnumerable<dynamic>>> GetAllFichesBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20);

	Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId);
}
