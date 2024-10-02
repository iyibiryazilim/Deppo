using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface IVariantPropertyValueService
{
    Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int variantPropertReferenceId, int varyantRef, string search = "", int skip = 0, int take = 20);
}
