using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IWarehouseDetailAllFicheListService
    {
        public Task<DataResult<IEnumerable<dynamic>>> GetAllFiches(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");

        public Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int ficheReferenceId, int skip = 0, int take = 20, string externalDb = "");
    }
}