﻿using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IWarehouseInputTransactionService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetWarehouseInputProducts(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20);

        Task<DataResult<IEnumerable<dynamic>>> GetWarehouseInputTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");
    }
}