﻿using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface ICustomerDetailActionService
{
	Task<DataResult<IEnumerable<dynamic>>> GetWaitingSalesOrdersByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");

	Task<DataResult<IEnumerable<dynamic>>> GetShipAddressesByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20);
	Task<DataResult<IEnumerable<dynamic>>> GetApprovedProductsByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20);
}
