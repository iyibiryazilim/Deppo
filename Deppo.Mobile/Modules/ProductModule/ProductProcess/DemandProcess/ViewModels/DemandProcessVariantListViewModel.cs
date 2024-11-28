using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    public partial class DemandProcessVariantListViewModel : BaseViewModel
    {
        private IHttpClientService _httpClientService;
        private IDemandProcessVariantService _demandProcessVariantService;
        private IWarehouseParameterService _warehouseParameterService;
        private IServiceProvider _serviceProvider;
        private IUserDialogs _userDialogs;

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private VariantWarehouseTotalModel? selectedVariant;

        [ObservableProperty]
        private WarehouseParameterModel warehouseParameterModel = new();

        public ObservableCollection<VariantWarehouseTotalModel> Items { get; } = new();
        public ObservableCollection<VariantWarehouseTotalModel> SelectedItems { get; } = new();

        [ObservableProperty]
        public ObservableCollection<DemandProcessBasketModel> selectedVariants = new();

        public DemandProcessVariantListViewModel(IHttpClientService httpClientService, IDemandProcessVariantService demandProcessVariantService, IWarehouseParameterService warehouseParameterService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _demandProcessVariantService = demandProcessVariantService;
            _warehouseParameterService = warehouseParameterService;
            _serviceProvider = serviceProvider;
            _userDialogs = userDialogs;

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<VariantWarehouseTotalModel>(async (item) => await ItemTappedAsync(item));
            ConfirmCommand = new Command(async () => await ConfirmAsync());
            BackCommand = new Command(async () => await BackAsync());
            PerformSearchCommand = new Command(async () => await PerformSearchAsync());
            PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        }

        public Page CurrentPage { get; set; }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command ConfirmCommand { get; }
        public Command BackCommand { get; }
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
                var result = await _demandProcessVariantService.GetVariants(
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

                    foreach (var variant in result.Data)
                    {
                        var item = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId);
                        if (matchingItem != null)
                            item.IsSelected = matchingItem.IsSelected;
                        else
                            item.IsSelected = false;

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

                _userDialogs.Loading("Loading Items...");

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _demandProcessVariantService.GetVariants(
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

                    foreach (var variant in result.Data)
                    {
                        var item = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId);
                        if (matchingItem != null)
                            item.IsSelected = matchingItem.IsSelected;
                        else
                            item.IsSelected = false;

                        Items.Add(item);
                    }
                }

                _userDialogs.HideHud();
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

        private async Task LoadWarehouseParameterAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.Loading("Loading Items...");
                Items.Clear();
                await Task.Delay(200);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseParameterService.GetObjectsByVariant(
                    httpClient,
                    _httpClientService.FirmNumber,
                    _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    variantReferenceId: SelectedVariant.VariantReferenceId,
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

        private async Task SwipeItemAsync(VariantWarehouseTotalModel item)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    SelectedVariant = item;
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

        private async Task ItemTappedAsync(VariantWarehouseTotalModel item)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    if (!item.IsSelected)
                    {
                        Items.ToList().FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId).IsSelected = true;
                        SelectedVariant = item;
                        SelectedItems.Add(item);

                        var basketItem = new DemandProcessBasketModel
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
                            MainItemReferenceId = item.ProductReferenceId,  //
                            MainItemCode = item.ProductCode,    //
                            MainItemName = item.ProductName,    //
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
                            Image = item.ImageData,
                        };

                        SelectedVariants.Add(basketItem);
                    }
                    else
                    {
                        SelectedVariant = null;
                        var selectedItem = SelectedVariants.FirstOrDefault(x => x.ItemReferenceId == item.VariantReferenceId);
                        if (selectedItem is not null)
                        {
                            SelectedVariants.Remove(selectedItem);
                            Items.ToList().FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId).IsSelected = false;
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
                    if (SelectedVariants.Any())
                    {
                        foreach (var item in SelectedVariants)
                            if (!previousViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
                                previousViewModel.Items.Add(item);

                        SelectedVariants.Clear();
                    }

                    SelectedItems.Clear();

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

                if (SelectedVariants.Count > 0)
                {
                    var result = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

                    if (!result)
                    {
                        return;
                    }
                    foreach (var item in Items)
                        item.IsSelected = false;

                    foreach (var item in SelectedVariants)
                        item.IsSelected = false;

                    SelectedVariants.Clear();
                }
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
                var result = await _demandProcessVariantService.GetVariants(
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

                    foreach (var variant in result.Data)
                    {
                        var item = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId);
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