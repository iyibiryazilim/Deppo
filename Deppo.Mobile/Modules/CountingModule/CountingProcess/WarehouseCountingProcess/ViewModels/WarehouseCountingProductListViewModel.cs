using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseCountingWarehouseModel), queryId: nameof(WarehouseCountingWarehouseModel))]
public partial class WarehouseCountingProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IWarehouseTotalService _warehouseTotalService;

	[ObservableProperty]
	WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;
	public WarehouseCountingProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseTotalService warehouseTotalService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_warehouseTotalService = warehouseTotalService;

		Title = "Ürün Listesi";
	}

	public Command LoadItemsCommand {get; }
	public Command LoadMoreItemsCommand { get; }
	public Command BackCommand { get; }
	public Command NextCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }

}
