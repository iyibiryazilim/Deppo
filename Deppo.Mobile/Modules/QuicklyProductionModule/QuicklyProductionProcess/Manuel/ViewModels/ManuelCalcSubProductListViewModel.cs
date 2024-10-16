using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ManuelCalcSubProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	private readonly IWarehouseTotalService _warehouseTotalService;
	private readonly IVariantWarehouseTotalService _variantWarehouseTotalService;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	public SearchBar searchText;

	[ObservableProperty]
	WarehouseTotalModel? selectedWarehouseTotalModel;

	public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
	public ObservableCollection<VariantWarehouseTotalModel> ItemVariants { get; } = new();
	public ObservableCollection<BOMSubProductModel> SelectedProducts { get; } = new();

	//Arama İşlemi için kullanılan liste
	public ObservableCollection<WarehouseTotalModel> SelectedSearchItems { get; } = new();

	public ObservableCollection<WarehouseTotalModel> SelectedItems { get; } = new();

	public ManuelCalcSubProductListViewModel(
		IHttpClientService httpClientService,
		IUserDialogs userDialogs,
		IServiceProvider serviceProvider,
		IWarehouseTotalService warehouseTotalService,
		IVariantWarehouseTotalService variantWarehouseTotalService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_warehouseTotalService = warehouseTotalService;
		_variantWarehouseTotalService = variantWarehouseTotalService;

		Title = "Ürün Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseTotalModel>(async (item) => await ItemTappedAsync(item));
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

		LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
		VariantTappedCommand = new Command<VariantWarehouseTotalModel>(async (item) => await VariantTappedAsync(item));
		ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
	}

	public Page CurrentPage { get; set; }

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command NextViewCommand { get; }
	public Command<WarehouseTotalModel> ItemTappedCommand { get; }
	public Command ConfirmCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }

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
			Items.Clear();
			SelectedItems.Clear();

			_userDialogs.Loading("Loading Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await Task.Delay(1000);

			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				search: SearchText.Text,
				skip: 0,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
					var matchedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
					if (matchedItem is not null)
						item.IsSelected = matchedItem.IsSelected;
					else
						item.IsSelected = false;

					Items.Add(item);
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
			SelectedItems.Clear();

			_userDialogs.Loading("Loading Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await Task.Delay(500);

			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				search: SearchText.Text,
				skip: Items.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
					var matchedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
					if (matchedItem is not null)
						item.IsSelected = matchedItem.IsSelected;
					else
						item.IsSelected = false;

					Items.Add(item);
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ItemTappedAsync(WarehouseTotalModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			var manuelCalcViewModel = _serviceProvider.GetRequiredService<ManuelCalcViewModel>();

			SelectedWarehouseTotalModel = item;

			if (item.IsSelected)
			{
				Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = false;
				SelectedItems.Remove(item);
				SelectedSearchItems.Remove(item);

				if (manuelCalcViewModel is not null)
				{
					if(item.IsVariant) {
						var subProduct = manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.FirstOrDefault(x => x.ProductModel.MainProductReferenceId == item.ProductReferenceId);
						if (subProduct is not null)
							manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.Remove(subProduct);
					}
					else
					{
						var subProduct = manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.FirstOrDefault(x => x.ProductModel.ReferenceId == item.ProductReferenceId);
						if (subProduct is not null)
							manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.Remove(subProduct);
					}
				}
			}
			else
			{
				if (item.IsVariant)
				{
					await LoadVariantItemsAsync(item);
					CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
				}
				else
				{
					manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.Add(new QuicklyBomSubProductModel
					{
						WarehouseModel = WarehouseModel,
						SubBOMQuantity = 0,
						SubOutputQuantity = 0,
						ProductModel = new BOMSubProductModel
						{
							ReferenceId = item.ProductReferenceId,
							Code = item.ProductCode,
							Name = item.ProductName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							Amount = default,
							BrandCode = item.BrandCode,
							BrandName = item.BrandName,
							GroupCode = item.GroupCode,
							//Image = item.Image,
							BrandReferenceId = item.BrandReferenceId,
							IsSelected = false,
							LocTracking = item.LocTracking,
							LocTrackingIcon = item.LocTrackingIcon,
							MainProductCode = item.ProductCode,
							MainProductReferenceId = item.ProductReferenceId,
							MainProductName = item.ProductName,
							StockQuantity = item.StockQuantity,
							TrackingType = item.TrackingType,
							VariantIcon = item.VariantIcon,
							TrackingTypeIcon = item.TrackingTypeIcon,
							VatRate = default,
							WarehouseName = WarehouseModel.Name,
							WarehouseNumber = WarehouseModel.Number,
							IsVariant = item.IsVariant
						},
						LocationTransactions = new()
					});

					Items.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;
					SelectedItems.Add(item);
					SelectedSearchItems.Add(item);
				}
			}
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

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			//var manuelCalcViewModel = _serviceProvider.GetRequiredService<ManuelCalcViewModel>();
			//if (manuelCalcViewModel is not null)
			//{
			//	foreach (var item in SelectedItems)
			//	{
			//		if (!manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.Any(x => x.ProductModel.Code == item.ProductCode))
			//		{
			//			manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.Add(new QuicklyBomSubProductModel
			//			{
			//				WarehouseModel = WarehouseModel,
			//				SubBOMQuantity = 0,
			//				SubOutputQuantity = 0,
			//				ProductModel = new BOMSubProductModel
			//				{
			//					ReferenceId = item.ProductReferenceId,
			//					Code = item.ProductCode,
			//					Name = item.ProductName,
			//					UnitsetReferenceId = item.UnitsetReferenceId,
			//					UnitsetCode = item.UnitsetCode,
			//					UnitsetName = item.UnitsetName,
			//					SubUnitsetReferenceId = item.SubUnitsetReferenceId,
			//					SubUnitsetCode = item.SubUnitsetCode,
			//					SubUnitsetName = item.SubUnitsetName,
			//					Amount = default,
			//					BrandCode = item.BrandCode,
			//					BrandName = item.BrandName,
			//					GroupCode = item.GroupCode,
			//					//Image = item.Image,
			//					BrandReferenceId = item.BrandReferenceId,
			//					IsSelected = false,
			//					LocTracking = item.LocTracking,
			//					LocTrackingIcon = item.LocTrackingIcon,
			//					MainProductCode = item.ProductCode,
			//					MainProductReferenceId = item.ProductReferenceId,
			//					StockQuantity = item.StockQuantity,
			//					TrackingType = item.TrackingType,
			//					VariantIcon = item.VariantIcon,
			//					TrackingTypeIcon = item.TrackingTypeIcon,
			//					VatRate = default,
			//					WarehouseName = WarehouseModel.Name,
			//					WarehouseNumber = WarehouseModel.Number,
			//					IsVariant = item.IsVariant
			//				},
			//				LocationTransactions = new()
			//			});
			//		}
			//	}
			//}
			//SelectedSearchItems.Clear();
			await Shell.Current.GoToAsync("../..");
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
				warehouseNumber: WarehouseModel.Number,
				search: SearchText.Text,
				skip: 0,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
					var matchedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
					if (matchedItem is not null)
						item.IsSelected = matchedItem.IsSelected;
					else
						item.IsSelected = false;

					Items.Add(item);
				}
			}

			_userDialogs.Loading().Hide();
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

	private async Task LoadVariantItemsAsync(WarehouseTotalModel item)
	{
		try
		{
			_userDialogs.ShowLoading("Loading Variant Items...");
			ItemVariants.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantWarehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				productReferenceId: item.ProductReferenceId,
				search: string.Empty,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var itemVariant in result.Data)
				{
					var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(itemVariant);
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

			_userDialogs.ShowLoading("Loading More Variant Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantWarehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				productReferenceId: SelectedWarehouseTotalModel.ProductReferenceId,
				search: string.Empty,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var itemVariant in result.Data)
				{
					var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(itemVariant);
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

			if (item is null)
			{
				await _userDialogs.AlertAsync("Lütfen bir varyant seçiniz.", "Uyarı", "Tamam");
				return;
			}

			var manuelCalcViewModel = _serviceProvider.GetRequiredService<ManuelCalcViewModel>();

			manuelCalcViewModel?.QuicklyBomProductBasketModel.SubProducts.Add(new QuicklyBomSubProductModel
			{
				WarehouseModel = WarehouseModel,
				SubBOMQuantity = 0,
				SubOutputQuantity = 0,
				ProductModel = new BOMSubProductModel
				{
					ReferenceId = item.VariantReferenceId,
					Code = item.VariantCode,
					Name = item.VariantName,
					UnitsetReferenceId = item.UnitsetReferenceId,
					UnitsetCode = item.UnitsetCode,
					UnitsetName = item.UnitsetName,
					SubUnitsetReferenceId = item.SubUnitsetReferenceId,
					SubUnitsetCode = item.SubUnitsetCode,
					SubUnitsetName = item.SubUnitsetName,
					Amount = default,
					BrandCode = item.BrandCode,
					BrandName = item.BrandName,
					GroupCode = item.GroupCode,
					//Image = item.Image,
					BrandReferenceId = item.BrandReferenceId,
					IsSelected = false,
					LocTracking = item.LocTracking,
					LocTrackingIcon = item.LocTrackingIcon,
					MainProductCode = item.ProductCode,
					MainProductReferenceId = item.ProductReferenceId,
					StockQuantity = item.StockQuantity,
					TrackingType = item.TrackingType,
					VariantIcon = item.VariantIcon,
					TrackingTypeIcon = item.TrackingTypeIcon,
					VatRate = default,
					WarehouseName = WarehouseModel.Name,
					WarehouseNumber = WarehouseModel.Number,
					IsVariant = true
				},
				LocationTransactions = new()
			});

			if (SelectedWarehouseTotalModel is not null)
			{
				SelectedWarehouseTotalModel.IsSelected = true;
				Items.FirstOrDefault(x => x.ProductReferenceId == SelectedWarehouseTotalModel.ProductReferenceId).IsSelected = true;

			}

			CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;
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