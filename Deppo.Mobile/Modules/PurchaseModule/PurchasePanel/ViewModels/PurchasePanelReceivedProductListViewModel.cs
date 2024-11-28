using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels
{
    public partial class PurchasePanelReceivedProductListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IPurchasePanelService _purchasePanelService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        public Product selectedItem;

        public ObservableCollection<Product> Items { get; } = new();

        public ObservableCollection<PurchaseTransactionModel> ReceivedOrders { get; } = new();

        public PurchasePanelReceivedProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchasePanelService purchasePanelService)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _purchasePanelService = purchasePanelService;

            Title = "Mal Kabul Ürün Listesi";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<Product>(async (item) => await ItemTappedAsync(item));
            ReceivedOrdersCloseCommand = new Command(async () => await ReceivedOrdersCloseAsync());
            LoadMoreReceivedOrdersCommand = new Command(async () => await LoadMoreReceivedOrdersAsync());
            BackCommand = new Command(async () => await BackAsync());
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command NextViewCommand { get; }
        public Command ReceivedOrdersCloseCommand { get; }
        public Command LoadMoreReceivedOrdersCommand { get; }
        public Command BackCommand { get; }

        public Page CurrentPage { get; set; }

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
                var result = await _purchasePanelService.GetReceivedProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var waitingOrder = Mapping.Mapper.Map<PurchaseTransactionModel>(item);

                        // Veri tekrarını kontrol et
                        if (!Items.Any(x => x.ReferenceId == waitingOrder.ProductReferenceId))
                        {
                            Items.Add(new Product
                            {
                                ReferenceId = waitingOrder.ProductReferenceId,
                                Code = waitingOrder.ProductCode,
                                Name = waitingOrder.ProductName,
                                StockQuantity = waitingOrder.Quantity,
                                UnitsetReferenceId = waitingOrder.UnitsetReferenceId,
                                UnitsetName = waitingOrder.UnitsetName,
                                UnitsetCode = waitingOrder.UnitsetCode,
                                SubUnitsetReferenceId = waitingOrder.SubUnitsetReferenceId,
                                SubUnitsetName = waitingOrder.SubUnitsetName,
                                SubUnitsetCode = waitingOrder.SubUnitsetCode,
                                LocTracking = waitingOrder.LocTracking,
                                TrackingType = waitingOrder.TrackingType,
                                IsVariant = waitingOrder.IsVariant,
                                Image = waitingOrder.Image,
                            });
                        }
                    }
                }

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

                _userDialogs.ShowLoading("Loading...");

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _purchasePanelService.GetReceivedProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
                if (result.IsSuccess && result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var waitingOrder = Mapping.Mapper.Map<PurchaseTransactionModel>(item);

                        // Veri tekrarını kontrol et
                        if (!Items.Any(x => x.ReferenceId == waitingOrder.ProductReferenceId))
                        {
                            Items.Add(new Product
                            {
                                ReferenceId = waitingOrder.ProductReferenceId,
                                Code = waitingOrder.ProductCode,
                                Name = waitingOrder.ProductName,
                                StockQuantity = waitingOrder.Quantity,
                                UnitsetReferenceId = waitingOrder.UnitsetReferenceId,
                                UnitsetName = waitingOrder.UnitsetName,
                                UnitsetCode = waitingOrder.UnitsetCode,
                                SubUnitsetReferenceId = waitingOrder.SubUnitsetReferenceId,
                                SubUnitsetName = waitingOrder.SubUnitsetName,
                                SubUnitsetCode = waitingOrder.SubUnitsetCode,
                                LocTracking = waitingOrder.LocTracking,
                                TrackingType = waitingOrder.TrackingType,
                                IsVariant = waitingOrder.IsVariant,
                                Image = waitingOrder.Image,
                            });
                        }
                    }
                }

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


        private async Task ItemTappedAsync(Product product)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = product;

                await LoadShippedOrdersAsync(product);
                CurrentPage.FindByName<BottomSheet>("receivedOrdersBottomSheet").State = BottomSheetState.HalfExpanded;
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

        private async Task ReceivedOrdersCloseAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("receivedOrdersBottomSheet").State = BottomSheetState.Hidden;
            });
        }

        private async Task LoadShippedOrdersAsync(Product product)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);

                ReceivedOrders.Clear();

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _purchasePanelService.GetReceivedOrders(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, product.ReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        ReceivedOrders.Add(Mapping.Mapper.Map<PurchaseTransactionModel>(item));
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
        }

        private async Task LoadMoreReceivedOrdersAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _purchasePanelService.GetReceivedOrders(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedItem.ReferenceId, string.Empty, ReceivedOrders.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        ReceivedOrders.Add(Mapping.Mapper.Map<PurchaseTransactionModel>(item));
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
                if (Items.Count > 0)
                {
                    Items.Clear();
                }
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
}