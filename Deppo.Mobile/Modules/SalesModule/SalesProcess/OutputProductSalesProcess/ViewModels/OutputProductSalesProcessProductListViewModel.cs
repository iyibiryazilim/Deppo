using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class OutputProductSalesProcessProductListViewModel : BaseViewModel
{
	IHttpClientService _httpClientService;
	IWarehouseTotalService _warehouseTotalService;
	IVariantService _variantService;
	IServiceProvider _serviceProvider;
	IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;
	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	public OutputProductSalesProcessProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IVariantService variantService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_variantService = variantService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;
	}
}
