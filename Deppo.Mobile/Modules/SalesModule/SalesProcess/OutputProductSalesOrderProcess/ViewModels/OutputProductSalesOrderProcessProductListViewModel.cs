using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(CustomerModel), queryId: nameof(CustomerModel))]
public partial class OutputProductSalesOrderProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;
	private readonly IUserDialogs _userDialogs;

	public OutputProductSalesOrderProcessProductListViewModel(IHttpClientService httpClientService, IWaitingSalesOrderService waitingSalesOrderService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_waitingSalesOrderService = waitingSalesOrderService;
		_userDialogs = userDialogs;
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	#endregion

	#region Collections

	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	CustomerModel customerModel = null!;
	#endregion
}
