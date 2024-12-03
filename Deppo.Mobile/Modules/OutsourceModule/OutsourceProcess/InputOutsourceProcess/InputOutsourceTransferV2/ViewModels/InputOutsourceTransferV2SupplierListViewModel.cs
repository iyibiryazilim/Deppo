using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

public partial class InputOutsourceTransferV2SupplierListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IOutsourceService _outsourceService;
	private readonly IUserDialogs _userDialogs;

	public InputOutsourceTransferV2SupplierListViewModel(IHttpClientService httpClientService, IOutsourceService outsourceService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_outsourceService = outsourceService;
		_userDialogs = userDialogs;
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
}
