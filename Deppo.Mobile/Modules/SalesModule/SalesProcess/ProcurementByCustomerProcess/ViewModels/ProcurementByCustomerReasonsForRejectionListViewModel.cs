using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

public partial class ProcurementByCustomerReasonsForRejectionListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
	
	public ProcurementByCustomerReasonsForRejectionListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
	}

	public Command CloseCommand { get; }
    public Command ConfirmCommand { get; }
    public Command LoadItemsCommand { get; }
}
