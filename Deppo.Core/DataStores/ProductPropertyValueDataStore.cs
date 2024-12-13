using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
    public class ProductPropertyValueDataStore : IProductPropertyValueService
    {
        private string postUrl = "gateway/customQuery/CustomQuery";
        public Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productPropertyReferenceId, string search = "", int skip = 0, int take = 20)
        {
            
        }
    }
}
