using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IOutputOutsourceTransferV2Service
    {
        Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber,int warehouseNumber,int currentReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");

        Task<DataResult<IEnumerable<dynamic>>> GetObjectSubProducts(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId,  string search = "", int skip = 0, int take = 20, string externalDb = "");

    }

}
