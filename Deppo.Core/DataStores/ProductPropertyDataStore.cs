using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
    public class ProductPropertyDataStore : IProductPropertyService
    {
        private string postUrl = "gateway/customQuery/CustomQuery";

        public Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            throw new NotImplementedException();
        }
    }
}
