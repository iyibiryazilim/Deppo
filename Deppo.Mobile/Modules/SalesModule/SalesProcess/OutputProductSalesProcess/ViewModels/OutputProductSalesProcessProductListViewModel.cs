using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesProcessProductListViewModel : BaseViewModel
{
    private IHttpClientService _httpClientService;
    private IWarehouseTotalService _warehouseTotalService;
    private IVariantWarehouseTotalService _variantWarehouseTotalService;
    private IServiceProvider _serviceProvider;
    private IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private WarehouseTotalModel? selectedProduct;

    public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
    public ObservableCollection<WarehouseTotalModel> SelectedItems { get; } = new();

    [ObservableProperty]
    public ObservableCollection<OutputSalesBasketModel> selectedProducts = new();

    public ObservableCollection<VariantWarehouseTotalModel> ItemVariants { get; } = new();

    public OutputProductSalesProcessProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IVariantWarehouseTotalService variantWarehouseTotalService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseTotalService = warehouseTotalService;
        _variantWarehouseTotalService = variantWarehouseTotalService;
        _serviceProvider = serviceProvider;
        _userDialogs = userDialogs;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseTotalModel>(async (item) => await ItemTappedAsync(item));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantWarehouseTotalModel>(async (item) => await VariantTappedAsync(item));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }

    public Command LoadMoreVariantItemsCommand { get; }
    public Command VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;

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
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: 0,
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    var warehouseTotalModel = new WarehouseTotalModel
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
                        IsSelected = matchingItem != null ? matchingItem.IsSelected : false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                        Image = product.Image,
                    };

                    Items.Add(warehouseTotalModel);
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
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: Items.Count,
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    var warehouseTotalModel = new WarehouseTotalModel
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
                        IsSelected = matchingItem != null ? matchingItem.IsSelected : false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                        Image = product.Image,
                    };

                    Items.Add(warehouseTotalModel);
                }
            }

            if(_userDialogs.IsHudShowing)
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

    private async Task ItemTappedAsync(WarehouseTotalModel item)
    {
        if (IsBusy) return;
        if (item is null) return;

        try
        {
            IsBusy = true;
            SelectedProduct = item;

            if(!item.IsSelected)
            {
                if(item.IsVariant)
                {
                    await LoadVariantItemsAsync(item);
					CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
				}
                else
                {
					Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;
					
					var basketItem = new OutputSalesBasketModel
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
                        LocTrackingIcon = item.LocTrackingIcon,
                        VariantIcon = item.VariantIcon,
                        TrackingTypeIcon = item.TrackingTypeIcon,
                        Image = item.ImageData
                    };

					SelectedItems.Add(item);
                    SelectedProducts.Add(basketItem);
				}
			}
            else
            {
                if(SelectedProduct is not null)
                {
					SelectedProduct.IsSelected = false;
					SelectedProduct = null;
				}

				var selectedItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ProductReferenceId);

				if (item.IsVariant)
				{
					selectedItem = SelectedProducts.FirstOrDefault(x => x.MainItemReferenceId == item.ProductReferenceId);
				}

				if (selectedItem is not null)
				{
					SelectedProducts.Remove(selectedItem);
					Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = false;
					SelectedItems.Remove(item);
				}
			}            
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

    private async Task LoadVariantItemsAsync(WarehouseTotalModel item)
    {
        try
        {
            _userDialogs.Loading("Loading Variant Items...");
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
                take: 20,
                externalDb: _httpClientService.ExternalDatabase
            );

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

            _userDialogs.Loading("Loading More Variant Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantWarehouseTotalService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                productReferenceId: SelectedProduct.ProductReferenceId,
                search: string.Empty,
                skip: ItemVariants.Count,
                take: 20,
				externalDb: _httpClientService.ExternalDatabase
			);

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
            _userDialogs.Loading().Dispose();
        }
    }

    private async Task VariantTappedAsync(VariantWarehouseTotalModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

			if (item.IsSelected)
			{
				item.IsSelected = false;
			}
			else
			{
				item.IsSelected = true;
				ItemVariants.Where(x => x.VariantReferenceId != item.VariantReferenceId).ToList().ForEach(x => x.IsSelected = false);
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
				CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;
                return;
			}

            var basketItem = new OutputSalesBasketModel
            {
                ItemReferenceId = item.VariantReferenceId,
                ItemCode = item.VariantCode,
                ItemName = item.VariantName,
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
                LocTracking = item.LocTracking,
                TrackingType = item.TrackingType,
                IsVariant = true,
                LocTrackingIcon = item.LocTrackingIcon,
                VariantIcon = item.VariantIcon,
                TrackingTypeIcon = item.TrackingTypeIcon,
                Quantity = item.LocTracking == 0 ? 1 : 0,
                Image = item.ImageData
            };

            SelectedProducts.Add(basketItem);

            if (SelectedProduct is not null)
            {
                SelectedProduct.IsSelected = true;
                SelectedItems.Add(SelectedProduct);
            }

            CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;
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

            var previousViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();

            if (previousViewModel is not null)
            {
                if (SelectedProducts.Any())
                {
                    foreach (var item in SelectedProducts)
                        if (!previousViewModel.Items.Any(x => x.ItemReferenceId == item.ItemReferenceId))
                            previousViewModel.Items.Add(item);

                    SelectedProducts.Clear();
                }

                SelectedItems.Clear();
				SearchText.Text = string.Empty;

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

                if (!result)
                {
                    return;
                }

                SelectedProducts.Clear();
            }

            SelectedItems.ForEach(x => x.IsSelected = false);
			SelectedItems.Clear();

			SearchText.Text = string.Empty;
            await Shell.Current.GoToAsync("..");
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
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: 0,
                take: 20, 
                externalDb: _httpClientService.ExternalDatabase);

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
                        IsSelected = false,
                        LocTrackingIcon = product.LocTrackingIcon,
                        VariantIcon = product.VariantIcon,
                        TrackingTypeIcon = product.TrackingTypeIcon,
                        Image = product.Image,
                    };

                    Items.Add(item);
                }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
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