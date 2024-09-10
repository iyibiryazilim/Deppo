using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

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

	[ObservableProperty]
	WarehouseTotalModel? selectedProduct;

	public ObservableCollection<WarehouseTotalModel> Items { get; } = new();

	[ObservableProperty]
	public ObservableCollection<OutputProductBasketModel> selectedProducts = new();

	public OutputProductSalesProcessProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IVariantService variantService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_variantService = variantService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseTotalModel>(async (item) => await ItemTappedAsync(item));
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command ConfirmCommand { get; }
	public Command BackCommand { get; }

	public Command LoadVariantItemsCommand { get; }
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

			_userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTotalService.GetObjects(
				httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				search: string.Empty,
				skip: 0,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = new WarehouseTotalModel
					{
						ProductReferenceId = product.ProductReferenceId,
						ProductCode = product.ProductCode,
						ProductName = product.ProductName,
						UnitsetReferenceId = product.UnitsetReferenceId,
						UnitsetCode = product.UnitsetCode,
						UnitsetName = product.UnitsetName,
						SubUnitsetReferenceId = product.SubUnitsetReferenceId,
						SubUnitsetCode = product.SubUnitsetCode,
						SubUnitsetName = product.SubUnitsetName,
						StockQuantity = product.StockQuantity,
						WarehouseReferenceId = product.WarehouseReferenceId,
						WarehouseName = product.WarehouseName,
						WarehouseNumber = product.WarehouseNumber,
						LocTracking = product.LocTracking,
						IsVariant = product.IsVariant,
						TrackingType = product.TrackingType,
						IsSelected = false
					};

					Items.Add(item);
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTotalService.GetObjects(
				httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				search: string.Empty,
				skip: Items.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = new WarehouseTotalModel
					{
						ProductReferenceId = product.ProductReferenceId,
						ProductCode = product.ProductCode,
						ProductName = product.ProductName,
						UnitsetReferenceId = product.UnitsetReferenceId,
						UnitsetCode = product.UnitsetCode,
						UnitsetName = product.UnitsetName,
						SubUnitsetReferenceId = product.SubUnitsetReferenceId,
						SubUnitsetCode = product.SubUnitsetCode,
						SubUnitsetName = product.SubUnitsetName,
						StockQuantity = product.StockQuantity,
						WarehouseReferenceId = product.WarehouseReferenceId,
						WarehouseName = product.WarehouseName,
						WarehouseNumber = product.WarehouseNumber,
						LocTracking = product.LocTracking,
						IsVariant = product.IsVariant,
						TrackingType = product.TrackingType,
						IsSelected = false
					};

					Items.Add(item);
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

	private async Task ItemTappedAsync(WarehouseTotalModel item)
	{
		if (IsBusy) return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				if (item.IsVariant)
				{

				}
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

			var previousViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();

			if(previousViewModel is not null)
			{
				if(SelectedProducts.Any())
				{
					foreach (var item in SelectedProducts)
						if (!previousViewModel.Items.Any(x => x.ItemCode == item.ItemName))
							previousViewModel.Items.Add(item);

					SelectedProducts.Clear();
				} 
				

				await Shell.Current.GoToAsync("..");
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (SelectedProducts.Count > 0)
			{
				var result = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

				if (result)
				{
					SelectedProducts.Clear();
					await Shell.Current.GoToAsync("..");
				}

			}
			else
			{
				await Shell.Current.GoToAsync("..");

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

}
