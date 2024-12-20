﻿using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IQuicklyBomService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetObjectsWorkOrder(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");

        Task<DataResult<IEnumerable<dynamic>>> GetObjectsWorkSubProducts(HttpClient httpClient, int firmNumber, int mainProductReferenceId, int periodNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");
    }
}
