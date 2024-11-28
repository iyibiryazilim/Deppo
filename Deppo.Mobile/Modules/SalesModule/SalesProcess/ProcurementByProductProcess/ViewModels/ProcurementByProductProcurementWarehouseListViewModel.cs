using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

[QueryProperty(name: nameof(SelectedCustomers), queryId: nameof(SelectedCustomers))]
[QueryProperty(name: nameof(ProductOrderModel), queryId: nameof(ProductOrderModel))]
[QueryProperty(name: nameof(OrderWarehouse), queryId: nameof(OrderWarehouse))]
public partial class ProcurementByProductProcurementWarehouseListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWarehouseService _warehouseService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ObservableCollection<CustomerOrderModel> selectedCustomers;

	[ObservableProperty]
	WarehouseModel orderWarehouse;

	[ObservableProperty]
	ProductOrderModel productOrderModel;


	public ObservableCollection<WarehouseModel> Items { get; } = new();

	[ObservableProperty]
	WarehouseModel selectedItem;

	public ProcurementByProductProcurementWarehouseListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWarehouseService warehouseService)
	{
		_httpClientService = httpClientService;
		_warehouseService = warehouseService;
		_userDialogs = userDialogs;

		Title = "Toplama Ambarını Seçiniz";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command BackCommand { get; }
	public Command NextViewCommand { get; }
	public Command ItemTappedCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Yükleniyor...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseService.GetObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: string.Empty,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
					foreach (var item in result.Data)
					{
						var warehouse = Mapping.Mapper.Map<WarehouseModel>(item);
						Items.Add(warehouse);
					}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadMoreItemsAsync()
	{
		if (Items.Count < 18)
			return;
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Yükleniyor...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseService.GetObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: string.Empty,
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
					foreach (var item in result.Data)
					{
						var warehouse = Mapping.Mapper.Map<WarehouseModel>(item);
						Items.Add(warehouse);
					}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (System.Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ItemTappedAsync(WarehouseModel warehouse)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (warehouse == SelectedItem)
			{
				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}
			else
			{
				if (SelectedItem != null)
				{
					SelectedItem.IsSelected = false;
				}

				SelectedItem = warehouse;
				SelectedItem.IsSelected = true;
			}

		}
		catch (System.Exception ex)
		{
			_userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ProcurementByProductProcurableProductListView)}", new Dictionary<string, object>
			{
				["SelectedCustomers"] = SelectedCustomers,
				[nameof(ProductOrderModel)] = ProductOrderModel,
				["OrderWarehouse"] = OrderWarehouse,
				["ProcurementWarehouse"] = SelectedItem
			});
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync("..");

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
