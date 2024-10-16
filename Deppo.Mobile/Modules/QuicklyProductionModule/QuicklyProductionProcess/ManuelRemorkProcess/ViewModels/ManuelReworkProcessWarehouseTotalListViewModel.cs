using Android.Icu.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessWarehouseTotalListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IWarehouseTotalService _warehouseTotalService;
	private readonly IVariantWarehouseTotalService _variantWarehouseTotalService;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;

	[ObservableProperty]
	public SearchBar searchText;

	public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
	public ObservableCollection<VariantWarehouseTotalModel> ItemVariants { get; } = new();

	[ObservableProperty]
	WarehouseTotalModel selectedWarehouseTotalModel = null!;

    public ManuelReworkProcessWarehouseTotalListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseTotalService warehouseTotalService, IVariantWarehouseTotalService variantWarehouseTotalService)
    {
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_warehouseTotalService = warehouseTotalService;
		_variantWarehouseTotalService = variantWarehouseTotalService;

		Title = "Çıkış Ürünü Seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseTotalModel>(async (x) => await ItemTappedAsync(x));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

		LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
		VariantTappedCommand = new Command<VariantWarehouseTotalModel>(async (x) => await VariantTappedAsync(x));
		ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());

    }
	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command NextViewCommand { get; }
	public Command<WarehouseTotalModel> ItemTappedCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }
	public Command BackCommand { get; }

	public Command LoadMoreVariantItemsCommand { get; }
	public Command VariantTappedCommand { get; }
	public Command ConfirmVariantCommand { get; }

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
				search: SearchText.Text,
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
				search: SearchText.Text,
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

			SelectedWarehouseTotalModel = item;

			if(item.IsSelected)
			{
				item.IsSelected = false;
				SelectedWarehouseTotalModel = null;
			}
			else
			{
				if(item.IsVariant)
				{
					await LoadVariantItemsAsync(item);
					CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
				}
				else
				{
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

					Items.ToList().ForEach(x => x.IsSelected = false);
					item.IsSelected = true;
				}
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

	private async Task LoadVariantItemsAsync(WarehouseTotalModel item)
	{
		try
		{
			_userDialogs.Loading("Loading Variant Items");
			ItemVariants.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantWarehouseTotalService.GetObjects(
				httpClient, 
				firmNumber: _httpClientService.FirmNumber, 
				periodNumber: _httpClientService.PeriodNumber, 
				productReferenceId: item.ProductReferenceId,
				warehouseNumber: ReworkBasketModel.OutWarehouseModel.Number, 
				search: string.Empty,
				skip: 0, 
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var variant in result.Data)
				{
					var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
					ItemVariants.Add(obj);
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadMoreVariantItemsAsync()
	{
		if (ItemVariants.Count < 18)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading More Variant Items");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantWarehouseTotalService.GetObjects(
				httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedWarehouseTotalModel.ProductReferenceId,
				warehouseNumber: ReworkBasketModel.OutWarehouseModel.Number,
				search: string.Empty,
				skip: ItemVariants.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var variant in result.Data)
				{
					var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
					ItemVariants.Add(obj);
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

	private async Task VariantTappedAsync(VariantWarehouseTotalModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			ItemVariants.ToList().ForEach(x => x.IsSelected = false);
			var selectedItem = ItemVariants.FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId);
			if (selectedItem != null)
				selectedItem.IsSelected = true;
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ConfirmVariantAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var item = ItemVariants.FirstOrDefault(x => x.IsSelected);
			if(item is not null)
			{
				ReworkBasketModel.ReworkOutProductModel = new ReworkOutProductModel
				{
					ReferenceId = item.VariantReferenceId,
					Code = item.VariantCode,
					Name = item.VariantName,
					MainItemReferenceId = item.ProductReferenceId,
					MainItemCode = item.ProductCode,
					MainItemName = item.ProductName,
					UnitsetReferenceId = item.UnitsetReferenceId,
					UnitsetCode = item.UnitsetCode,
					UnitsetName = item.UnitsetName,
					SubUnitsetReferenceId = item.SubUnitsetReferenceId,
					SubUnitsetCode = item.SubUnitsetCode,
					SubUnitsetName = item.SubUnitsetName,
					GroupCode = item.GroupCode,
					IsSelected = false,
					Image = item.Image,
					BrandReferenceId = item.BrandReferenceId,
					BrandCode = item.BrandCode,
					BrandName = item.BrandName,
					IsVariant = item.IsVariant,
					LocTracking = item.LocTracking,
					TrackingType = item.TrackingType,
					LocTrackingIcon = item.LocTrackingIcon,
					TrackingTypeIcon = item.TrackingTypeIcon,
					VariantIcon = item.VariantIcon,
					WarehouseReferenceId = item.WarehouseReferenceId,
					WarehouseName = item.WarehouseName,
					WarehouseNumber = item.WarehouseNumber,
					StockQuantity = item.StockQuantity,
					OutputQuantity = 0
				};

				Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;
			}
			

			CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SearchText.Text = string.Empty;

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

	public async Task ClearPageAsync()
	{
		try
		{
			await Task.Run(() =>
			{
				if(ReworkBasketModel is not null)
				{
					ReworkBasketModel.OutWarehouseModel = null;
					ReworkBasketModel.ReworkOutProductModel = null;
				}
				SearchText.Text = string.Empty;
			});
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task PerformSearchAsync()
	{
		if (IsBusy)
			return;
		try
		{
			if (string.IsNullOrWhiteSpace(SearchText.Text))
			{
				await LoadItemsAsync();
				SearchText.Unfocus();
				return;
			}

			IsBusy = true;

			Items.Clear();
			_userDialogs.Loading("Searching Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: ReworkBasketModel.OutWarehouseModel.Number,
				search: SearchText.Text,
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
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task PerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(SearchText.Text))
		{
			await PerformSearchAsync();
		}
	}
}
