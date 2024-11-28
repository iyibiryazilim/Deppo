using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using DevExpress.Data.Async.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(nameof(WarehouseCountingWarehouseModel), nameof(WarehouseCountingWarehouseModel))]
[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(ProductVariantType), nameof(ProductVariantType))]
public partial class WarehouseCountingShowProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IWarehouseCountingService _warehouseCountingService;
	private readonly IUserDialogs _userDialogs;
	private readonly IProductService _productService;

	[ObservableProperty]
	private LocationModel? locationModel;

	[ObservableProperty]
	private WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;

	[ObservableProperty]
	private ProductVariantType productVariantType;

	public ObservableCollection<ProductModel> Items { get; } = new();
	public ObservableCollection<ProductModel> SelectedItems { get; } = new();
	public ObservableCollection<WarehouseCountingBasketModel> BasketItems { get; } = new();

	[ObservableProperty]
	ProductModel selectedItem;

	[ObservableProperty]
	public SearchBar searchText;

	public WarehouseCountingShowProductListViewModel(IHttpClientService httpClientService, IServiceProvider serviceProvider, IUserDialogs userDialogs, IWarehouseCountingService warehouseCountingService, IProductService productService)
	{
		_httpClientService = httpClientService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;
		_warehouseCountingService = warehouseCountingService;
		_productService = productService;

		BackCommand = new Command(async () => await BackAsync());
		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<ProductModel>(async (item) => await ItemTappedAsync(item));
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command ConfirmCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }
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

			var result = await _productService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<ProductModel>(item);
					var matchingItem = SelectedItems.FirstOrDefault(x => x.Code == obj.Code);

					Items.Add(new ProductModel
					{
						ReferenceId = item.ReferenceId,
						Code = item.Code,
						Name = item.Name,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						SubUnitsetName = item.SubUnitsetName,
						StockQuantity = item.StockQuantity,
						TrackingType = item.TrackingType,
						LocTracking = item.LocTracking,
						GroupCode = item.GroupCode,
						BrandReferenceId = item.BrandReferenceId,
						BrandCode = item.BrandCode,
						BrandName = item.BrandName,
						VatRate = item.VatRate,
						Image = item.Image,
						IsVariant = item.IsVariant,
						IsSelected = matchingItem != null ? matchingItem.IsSelected : false,
					});
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		if (Items.Count < 18)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<ProductModel>(item);
					var matchingItem = SelectedItems.FirstOrDefault(x => x.Code == obj.Code);

					Items.Add(new ProductModel
					{
						ReferenceId = item.ReferenceId,
						Code = item.Code,
						Name = item.Name,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						SubUnitsetName = item.SubUnitsetName,
						StockQuantity = item.StockQuantity,
						TrackingType = item.TrackingType,
						LocTracking = item.LocTracking,
						GroupCode = item.GroupCode,
						BrandReferenceId = item.BrandReferenceId,
						BrandCode = item.BrandCode,
						BrandName = item.BrandName,
						VatRate = item.VatRate,
						Image = item.Image,
						IsVariant = item.IsVariant,
						IsSelected = matchingItem != null ? matchingItem.IsSelected : false,
					});
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

	private async Task ItemTappedAsync(ProductModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = item;

			if(item.IsSelected)
			{
				SelectedItem.IsSelected = false;
				SelectedItem = null;
				var selectedItem = BasketItems.FirstOrDefault(x => x.ItemCode == item.Code);
				if(selectedItem is not null)
				{
					Items.ToList().FirstOrDefault(x => x.Code == item.Code).IsSelected = false;
					BasketItems.Remove(selectedItem);
					SelectedItems.Remove(item);
				}
			}
			else
			{

				var basketItem = new WarehouseCountingBasketModel
				{
					ItemReferenceId = item.ReferenceId,
					ItemCode = item.Code,
					ItemName = item.Name,
					MainItemReferenceId = 0,
					MainItemCode = "",
					MainItemName = "",
					IsVariant = item.IsVariant,
					LocTracking = item.LocTracking,
					TrackingType = item.TrackingType,
					SubUnitsetCode = item.SubUnitsetCode,
					SubUnitsetReferenceId = item.SubUnitsetReferenceId,
					SubUnitsetName = item.SubUnitsetName,
					UnitsetReferenceId = item.UnitsetReferenceId,
					UnitsetCode = item.UnitsetCode,
					UnitsetName = item.UnitsetName,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
					StockQuantity = item.StockQuantity,
					OutputQuantity = item.StockQuantity,
					Image = item.Image,
				};

				BasketItems.Add(basketItem);

				SelectedItem.IsSelected = true;
				SelectedItems.Add(item);
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

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var previousViewModel = _serviceProvider.GetRequiredService<WarehouseCountingBasketViewModel>();
			if(BasketItems.Any())
			{
                foreach (var item in BasketItems)
                {
                    if(!previousViewModel.Items.Any( x => x.ItemCode == item.ItemCode))
					{
						previousViewModel.Items.Add(item);
					}
                }
            }

			await Shell.Current.GoToAsync("..");
			SearchText.Text = string.Empty;
			SelectedItems.Clear();
			BasketItems.Clear();
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
			var result = await _productService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<ProductModel>(item);
					var matchingItem = SelectedItems.FirstOrDefault(x => x.Code == obj.Code);

					Items.Add(new ProductModel
					{
						ReferenceId = item.ReferenceId,
						Code = item.Code,
						Name = item.Name,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						SubUnitsetName = item.SubUnitsetName,
						StockQuantity = item.StockQuantity,
						TrackingType = item.TrackingType,
						LocTracking = item.LocTracking,
						GroupCode = item.GroupCode,
						BrandReferenceId = item.BrandReferenceId,
						BrandCode = item.BrandCode,
						BrandName = item.BrandName,
						VatRate = item.VatRate,
						Image = item.Image,
						IsVariant = item.IsVariant,
						IsSelected = matchingItem != null ? matchingItem.IsSelected : false,
					});
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

	private async Task PerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(SearchText.Text))
		{
			await PerformSearchAsync();
		}
	}



	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			if(SelectedItems.Count > 0)
			{
				var result = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
				if (!result)
				{
					return;
				}
				SelectedItems.Clear();
				BasketItems.Clear();
			}

			SearchText.Text = string.Empty;
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
