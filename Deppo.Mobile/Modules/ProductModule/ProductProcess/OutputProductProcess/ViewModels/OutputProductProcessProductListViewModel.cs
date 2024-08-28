using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Converters;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWarehouseTotalService _warehouseTotalService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IVariantService _variantService;
	private readonly IUserDialogs _userDialogs;
	public OutputProductProcessProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IServiceProvider serviceProvider, IVariantService variantService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_serviceProvider = serviceProvider;
		_variantService = variantService;
		_userDialogs = userDialogs;

		Title = "Ürün Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<object>(ItemTappedAsync);
		LoadVariantItemsCommand = new Command(async () => await LoadVariantItemsAsync());
		LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
		VariantTappedCommand = new Command<VariantModel>(async (parameter) => await VariantTappedAsync(parameter));
		ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command LoadVariantItemsCommand { get; }
	public Command LoadMoreVariantItemsCommand { get; }
	public Command VariantTappedCommand { get; }
	public Command ConfirmVariantCommand { get; }
	public Command ConfirmCommand { get; }
	public Command BackCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
	public ObservableCollection<VariantModel> ItemVariants { get; } = new();


	[ObservableProperty]
	public ObservableCollection<OutputProductBasketModel> selectedProducts = new();

	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	WarehouseTotalModel? selectedProduct;

	#endregion

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<WarehouseTotal>(product);

					Items.Add(new WarehouseTotalModel
					{
						ProductReferenceId = item.ProductReferenceId,
						ProductCode = item.ProductCode,
						ProductName = item.ProductName,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						StockQuantity = item.StockQuantity,
						WarehouseReferenceId = item.WarehouseReferenceId,
						WarehouseName = item.WarehouseName,
						WarehouseNumber = item.WarehouseNumber,
						IsSelected = false,
					});
				}
			}

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

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: Items.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<WarehouseTotal>(product);

					Items.Add(new WarehouseTotalModel
					{
						ProductReferenceId = item.ProductReferenceId,
						ProductCode = item.ProductCode,
						ProductName = item.ProductName,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						StockQuantity = item.StockQuantity,
						WarehouseReferenceId = item.WarehouseReferenceId,
						WarehouseName = item.WarehouseName,
						WarehouseNumber = item.WarehouseNumber,
						IsSelected = false,
					});
				}
			}

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

	private async void ItemTappedAsync(object obj)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectProductModel selectProductModel = (SelectProductModel)obj;
			WarehouseTotalModel item = Items.FirstOrDefault(x => x.ProductReferenceId == selectProductModel.ItemReferenceId);
			BottomSheet variantBottomSheet = selectProductModel.BottomSheet;

			if (item is not null)
			{
				#region Varyantlı olma durumu
				if (item.IsVariant)
				{
					variantBottomSheet.State = BottomSheetState.HalfExpanded;
				}
				#endregion
				else
				{
					if (!item.IsSelected)
					{
						Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;
						SelectedProduct = item;

						var basketItem = new OutputProductBasketModel
						{
							ItemReferenceId = item.ProductReferenceId,
							ItemCode = item.ProductCode,
							ItemName = item.ProductName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							MainItemReferenceId = default,  //
							MainItemCode = string.Empty,    //
							MainItemName = string.Empty,    //
							StockQuantity = item.StockQuantity,
							IsSelected = false,   //
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							TrackingType = item.TrackingType,
							Quantity = item.LocTracking == 0 ? 1 : 0,
						};

						SelectedProducts.Add(basketItem);
					}
					else
					{
						SelectedProduct = null;
						var selectedItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ProductReferenceId);
						if (selectedItem is not null)
						{
							SelectedProducts.Remove(selectedItem);
							Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = false;
						}
					}
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

	private async Task LoadVariantItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Variant Items");
			ItemVariants.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: SelectedProduct.ProductReferenceId, warehouseNumber: warehouseModel.Number, skip: 0, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var variant in result.Data)
				{
					var item = Mapping.Mapper.Map<Variant>(variant);
					ItemVariants.Add(new VariantModel
					{
						Code = item.Code,
						Name = item.Name,

						IsSelected = false,
					});
				}
			}

			_userDialogs.Loading().Hide();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadMoreVariantItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: SelectedProduct.ProductReferenceId, warehouseNumber: warehouseModel.Number, skip: ItemVariants.Count(), take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var variant in result.Data)
				{
					var item = Mapping.Mapper.Map<Variant>(variant);
					ItemVariants.Add(new VariantModel
					{
						Code = item.Code,
						Name = item.Name,

						IsSelected = false,
					});
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task VariantTappedAsync(VariantModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			//ItemVariants.ToList().ForEach(x => x.IsSelected = false);
			var selectedItem = ItemVariants.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
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
			var basketItem = new OutputProductBasketModel
			{
				ItemReferenceId = item.ReferenceId,
				ItemCode = item.Code,
				ItemName = item.Name,
				UnitsetReferenceId = item.UnitsetReferenceId,
				UnitsetCode = item.UnitsetCode,
				UnitsetName = item.UnitsetName,
				SubUnitsetReferenceId = item.SubUnitsetReferenceId,
				SubUnitsetCode = item.SubUnitsetCode,
				SubUnitsetName = item.SubUnitsetName,
				IsSelected = false,
				MainItemCode = item.ProductCode,
				MainItemName = item.ProductName,
				MainItemReferenceId = item.ProductReferenceId,
				StockQuantity = item.StockQuantity,
				Quantity = item.LocTracking == 0 ? 1 : 0,
				TrackingType = item.TrackingType,
				LocTracking = item.LocTracking
			};

			SelectedProducts.Add(basketItem);
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

			var previousViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
			if (previousViewModel is not null)
			{
				foreach (var item in SelectedProducts)
				{
					if (!previousViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
						previousViewModel.Items.Add(item);
				}

				await Shell.Current.GoToAsync($"..");
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


	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"..");
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
}
