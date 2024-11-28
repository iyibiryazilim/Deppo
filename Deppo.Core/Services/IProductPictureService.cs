using System;
using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface IProductPictureService
{
    public Task<DataResult<IEnumerable<dynamic>>> InsertPictureAsync(HttpClient httpClient, int firmNumber, int productReferenceId,byte[] picture);
}
