using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(SelectedItems), nameof(SelectedItems))]

public partial class ProcurementByLocationOrderWarehouseListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWarehouseService _warehouseService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	private WarehouseModel? selectedOrderWarehouseModel;

	[ObservableProperty]
	private WarehouseModel? warehouseModel = null!;

	[ObservableProperty]
	private LocationModel? locationModel = null!;

	[ObservableProperty]
	private ObservableCollection<ProcurementByLocationProduct> selectedItems;

	public ProcurementByLocationOrderWarehouseListViewModel(
		IHttpClientService httpClientService,
		IWarehouseService warehouseService,
		IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_warehouseService = warehouseService;
		_userDialogs = userDialogs;

		Title = "Sipariş Ambarı Seçiniz";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
		NextViewCommand = new Command(async () => await NextViewAsync());
	}

	public ObservableCollection<WarehouseModel> Items { get; } = new();

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command<WarehouseModel> ItemTappedCommand { get; }
	public Command NextViewCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			Items.Clear();

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

	private async Task LoadMoreItemsAsync()
	{
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

			if (warehouse == SelectedOrderWarehouseModel)
			{
				SelectedOrderWarehouseModel.IsSelected = false;
				SelectedOrderWarehouseModel = null;
			}
			else
			{
				if (SelectedOrderWarehouseModel != null)
				{
					SelectedOrderWarehouseModel.IsSelected = false;
				}

				SelectedOrderWarehouseModel = warehouse;
				SelectedOrderWarehouseModel.IsSelected = true;
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

			if (SelectedOrderWarehouseModel is not null)
			{
				await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationCustomerListView)}", new Dictionary<string, object>
				{
					[nameof(WarehouseModel)] = WarehouseModel,
					[nameof(LocationModel)] = LocationModel,
					["SelectedProducts"] = SelectedItems,
					[nameof(SelectedOrderWarehouseModel)] = SelectedOrderWarehouseModel
				});

			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}
}
