using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    public partial class DemandProcessProductListViewModel : BaseViewModel
    {
        private IHttpClientService _httpClientService;
        private IDemandProcessProductService _demandProcessProductService;
        private IWarehouseParameterService _warehouseParameterService;
        private IServiceProvider _serviceProvider;
        private IUserDialogs _userDialogs;

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private ProductWarehouseTotalModel? selectedProduct;

        public ObservableCollection<ProductWarehouseTotalModel> Items { get; } = new();
        public ObservableCollection<ProductWarehouseTotalModel> SelectedItems { get; } = new();

        [ObservableProperty]
        public ObservableCollection<DemandProcessBasketModel> selectedProducts = new();

        [ObservableProperty]
        public WarehouseParameterModel warehouseParameterModel = new();

        public DemandProcessProductListViewModel(IHttpClientService httpClientService, IDemandProcessProductService demandProcessProductService, IServiceProvider serviceProvider, IUserDialogs userDialogs, IWarehouseParameterService warehouseParameterService)
        {
            _httpClientService = httpClientService;
            _demandProcessProductService = demandProcessProductService;
            _serviceProvider = serviceProvider;
            _warehouseParameterService = warehouseParameterService;
            _userDialogs = userDialogs;

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<ProductWarehouseTotalModel>(async (item) => await ItemTappedAsync(item));
            ConfirmCommand = new Command(async () => await ConfirmAsync());
            BackCommand = new Command(async () => await BackAsync());
            PerformSearchCommand = new Command(async () => await PerformSearchAsync());
            PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
            SwipeItemCommand = new Command<ProductWarehouseTotalModel>(async (item) => await SwipeItemAsync(item));
        }

        public Page CurrentPage { get; set; }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command ConfirmCommand { get; }
        public Command BackCommand { get; }
        public Command PerformSearchCommand { get; }
        public Command PerformEmptySearchCommand { get; }
        public Command SwipeItemCommand { get; }

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
                var result = await _demandProcessProductService.GetProducts(
                    httpClient,
                    _httpClientService.FirmNumber,
                    _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    search: SearchText.Text,
                    skip: 0,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<ProductWarehouseTotalModel>(product);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                        if (matchingItem != null)
                            item.IsSelected = matchingItem.IsSelected;
                        else
                            item.IsSelected = false;

                        Items.Add(item);
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

        private async Task LoadMoreItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.Loading("Loading More Items...");

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _demandProcessProductService.GetProducts(
                    httpClient,
                    _httpClientService.FirmNumber,
                    _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    search: SearchText.Text,
                    skip: Items.Count,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<ProductWarehouseTotalModel>(product);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                        if (matchingItem != null)
                            item.IsSelected = matchingItem.IsSelected;
                        else
                            item.IsSelected = false;

                        Items.Add(item);
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

        private async Task LoadWarehouseParameterAsync()
        {

            try
            {
                IsBusy = true;

                _userDialogs.Loading("Loading Items...");
                await Task.Delay(200);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseParameterService.GetObjectsByProduct(
                    httpClient,
                    _httpClientService.FirmNumber,
                    _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    productReferenceId: SelectedProduct.ProductReferenceId,
                    search: SearchText.Text,
                    skip: 0,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<WarehouseParameterModel>(product);
                        WarehouseParameterModel = item;
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

        private async Task SwipeItemAsync(ProductWarehouseTotalModel item)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    SelectedProduct = item;
                    await LoadWarehouseParameterAsync();
                    CurrentPage.FindByName<BottomSheet>("warehouseParameterBottomSheet").State = BottomSheetState.HalfExpanded;
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

        private async Task ItemTappedAsync(ProductWarehouseTotalModel item)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    if (!item.IsSelected)
                    {
                        Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;
                        SelectedProduct = item;
                        SelectedItems.Add(item);

                        var basketItem = new DemandProcessBasketModel
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
                            SafeLevel = item.SafeLevel,
                            Quantity = item.SafeLevel - item.StockQuantity,
                            LocTrackingIcon = item.LocTrackingIcon,
                            VariantIcon = item.VariantIcon,
                            TrackingTypeIcon = item.TrackingTypeIcon,
                            Image = item.ImageData
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
                            SelectedItems.Remove(item);
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

                var previousViewModel = _serviceProvider.GetRequiredService<DemandProcessBasketListViewModel>();

                if (previousViewModel is not null)
                {
                    if (SelectedProducts.Any())
                    {
                        foreach (var item in SelectedProducts)
                            if (!previousViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
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
                    foreach (var item in Items)
                        item.IsSelected = false;

                    foreach (var item in SelectedProducts)
                        item.IsSelected = false;

                    SelectedProducts.Clear();
                }

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
                var result = await _demandProcessProductService.GetProducts(
                    httpClient,
                    _httpClientService.FirmNumber,
                    _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    search: SearchText.Text,
                    skip: 0,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<ProductWarehouseTotalModel>(product);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                        if (matchingItem != null)
                            item.IsSelected = matchingItem.IsSelected;
                        else
                            item.IsSelected = false;

                        Items.Add(item);
                    }
                }
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
}