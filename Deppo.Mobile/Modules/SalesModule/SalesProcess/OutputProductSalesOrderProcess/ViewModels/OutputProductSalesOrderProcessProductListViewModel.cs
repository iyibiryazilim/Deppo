using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesOrderProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	public OutputProductSalesOrderProcessProductListViewModel(IHttpClientService httpClientService, IWaitingSalesOrderService waitingSalesOrderService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_waitingSalesOrderService = waitingSalesOrderService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		Title = "Sipariş seçimi";
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
	#endregion

	#region Collections

	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;


	#endregion


}
