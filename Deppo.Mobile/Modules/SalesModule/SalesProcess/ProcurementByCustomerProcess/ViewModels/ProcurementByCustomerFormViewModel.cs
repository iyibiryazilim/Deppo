using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

public partial class ProcurementByCustomerFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IHttpClientSysService _httpClientSysService;
	public ProcurementByCustomerFormViewModel(IHttpClientService httpClientService, IHttpClientSysService httpClientSysService)
	{
		_httpClientService = httpClientService;
		_httpClientSysService = httpClientSysService;
	}

	public Page CurrentPage { get; set; } = null!;

    public Command SaveCommand { get; }
}
