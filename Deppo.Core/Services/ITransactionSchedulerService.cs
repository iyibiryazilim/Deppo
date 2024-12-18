using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface ITransactionSchedulerService
{
    public Task<DataResult<IEnumerable<dynamic>>> GetAllFiches(HttpClient httpClient, int firmNumber, int periodNumber, DateTime selectedDate, string search = "", int skip = 0, int take = 20, string externalDb = "");

    public Task<DataResult<IEnumerable<dynamic>>> GetLastTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string externalDb = "");
}