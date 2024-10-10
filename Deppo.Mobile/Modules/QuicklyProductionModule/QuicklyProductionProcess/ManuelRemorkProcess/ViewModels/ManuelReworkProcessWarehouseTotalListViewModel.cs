using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessWarehouseTotalListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IWarehouseTotalService _warehouseTotalService;


	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;

	public ObservableCollection<WarehouseTotalModel> Items { get; } = new();

	[ObservableProperty]
	WarehouseTotalModel selectedWarehouseTotalModel = null!;

    public ManuelReworkProcessWarehouseTotalListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseTotalService warehouseTotalService)
    {
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_warehouseTotalService = warehouseTotalService;

		Title = "Çıkış Ürünü Seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseTotalModel>(async (x) => await ItemTappedAsync(x));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
    }

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command NextViewCommand { get; }
	public Command<WarehouseTotalModel> ItemTappedCommand { get; }
	public Command BackCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: ReworkBasketModel.OutWarehouseModel.Number,
				search: "",
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						Items.Add(Mapping.Mapper.Map<WarehouseTotalModel>(item));
					}
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
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

			_userDialogs.ShowLoading("Loading More Items...");
		
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: ReworkBasketModel.OutWarehouseModel.Number,
				search: "",
				skip: Items.Count,
				take: 20

			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						Items.Add(Mapping.Mapper.Map<WarehouseTotalModel>(item));
					}
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ItemTappedAsync(WarehouseTotalModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item == SelectedWarehouseTotalModel)
			{
				SelectedWarehouseTotalModel.IsSelected = false;
				SelectedWarehouseTotalModel = null;
			}
			else
			{
				if (SelectedWarehouseTotalModel != null)
				{
					SelectedWarehouseTotalModel.IsSelected = false;
				}

				SelectedWarehouseTotalModel = item;
				SelectedWarehouseTotalModel.IsSelected = true;
			}
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			ReworkBasketModel.ReworkOutProductModel = new ReworkOutProductModel
			{
				ReferenceId = SelectedWarehouseTotalModel.ProductReferenceId,
				Code = SelectedWarehouseTotalModel.ProductCode,
				Name = SelectedWarehouseTotalModel.ProductName,
				MainItemReferenceId = SelectedWarehouseTotalModel.ProductReferenceId,
				MainItemCode = SelectedWarehouseTotalModel.ProductCode,
				MainItemName = SelectedWarehouseTotalModel.ProductName,
				UnitsetReferenceId = SelectedWarehouseTotalModel.UnitsetReferenceId,
				UnitsetCode = SelectedWarehouseTotalModel.UnitsetCode,
				UnitsetName = SelectedWarehouseTotalModel.UnitsetName,
				SubUnitsetReferenceId = SelectedWarehouseTotalModel.SubUnitsetReferenceId,
				SubUnitsetCode = SelectedWarehouseTotalModel.SubUnitsetCode,
				SubUnitsetName = SelectedWarehouseTotalModel.SubUnitsetName,
				GroupCode = SelectedWarehouseTotalModel.GroupCode,
				IsSelected = false,
				Image = SelectedWarehouseTotalModel.Image,
				BrandReferenceId = SelectedWarehouseTotalModel.BrandReferenceId,
				BrandCode = SelectedWarehouseTotalModel.BrandCode,
				BrandName = SelectedWarehouseTotalModel.BrandName,
				IsVariant = SelectedWarehouseTotalModel.IsVariant,
				LocTracking = SelectedWarehouseTotalModel.LocTracking,
				TrackingType = SelectedWarehouseTotalModel.TrackingType,
				LocTrackingIcon = SelectedWarehouseTotalModel.LocTrackingIcon,
				TrackingTypeIcon = SelectedWarehouseTotalModel.TrackingTypeIcon,
				VariantIcon = SelectedWarehouseTotalModel.VariantIcon,
				WarehouseReferenceId = SelectedWarehouseTotalModel.WarehouseReferenceId,
				WarehouseName = SelectedWarehouseTotalModel.WarehouseName,
				WarehouseNumber = SelectedWarehouseTotalModel.WarehouseNumber,
				StockQuantity = SelectedWarehouseTotalModel.StockQuantity,
				OutputQuantity = 0
			};

			await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessBasketView)}", new Dictionary<string, object>
			{
				[nameof(ReworkBasketModel)] = ReworkBasketModel
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
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
