using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	public OutputProductProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;

		Title = "Form";
	}

	#region Commands
	public Command NextCommand { get; }
	#endregion

	#region Properties
	[ObservableProperty]
	OutputProductProcessType outputProductProcessType;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	string documentNumber = string.Empty;

	[ObservableProperty]
	DateTime transactionDate = DateTime.Now;
	#endregion

	
}
