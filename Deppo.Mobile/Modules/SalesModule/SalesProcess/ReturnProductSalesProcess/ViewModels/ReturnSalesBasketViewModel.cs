using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    public partial class ReturnSalesBasketViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly ILocationService _locationService;
        private readonly ISeriLotService _seriLotService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private ReturnSalesBasketModel? selectedInputProductBasketModel;

        public ReturnSalesBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IHttpClientService httpClientService2, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _locationService = locationService;
            _seriLotService = seriLotService;
            _serviceProvider = serviceProvider;
            Title = "Sepet Listesi";

            ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());

            DeleteItemCommand = new Command<ReturnSalesBasketModel>(async (item) => await DeleteItemAsync(item));
            IncreaseCommand = new Command<ReturnSalesBasketModel>(async (item) => await IncreaseAsync(item));
            DecreaseCommand = new Command<ReturnSalesBasketModel>(async (item) => await DecreaseAsync(item));

            LocationCloseCommand = new Command(async () => await LocationCloseAsync());
            LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
            LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
            LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));

            LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

            NextViewCommand = new Command(async () => await NextViewAsync());
            BackCommand = new Command(async () => await BackAsync());

            Items.Clear();
        }

        public Page CurrentPage { get; set; } = null!;

        public Command ShowProductViewCommand { get; }
        public Command<ReturnSalesBasketModel> DeleteItemCommand { get; }
        public Command<ReturnSalesBasketModel> IncreaseCommand { get; }
        public Command<ReturnSalesBasketModel> DecreaseCommand { get; }

        public Command<LocationModel> LocationDecreaseCommand { get; }
        public Command<LocationModel> LocationIncreaseCommand { get; }
        public Command<LocationModel> LocationConfirmCommand { get; }
        public Command LocationCloseCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }

        public Command LocationTransactionCloseCommand { get; }

        public ObservableCollection<ReturnSalesBasketModel> Items { get; } = new();
        public ObservableCollection<LocationModel> Locations { get; }

        private async Task ShowProductViewAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var viewModel = _serviceProvider.GetRequiredService<ReturnSalesProductListViewModel>();
                await viewModel.LoadPageAsync();
                await Shell.Current.GoToAsync($"{nameof(ReturnSalesProductListView)}", new Dictionary<string, object>
            {
                {nameof(WarehouseModel), WarehouseModel}
            });
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

        private async Task DeleteItemAsync(ReturnSalesBasketModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                    return;

                Items.Remove(item);
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

        private async Task IncreaseAsync(ReturnSalesBasketModel item)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                if (item.LocTracking == 1)
                {
                    var nextViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketLocationListViewModel>();

                    await Shell.Current.GoToAsync($"{nameof(ReturnSalesBasketLocationListView)}", new Dictionary<string, object>
                {
                    {nameof(WarehouseModel), WarehouseModel},
                    {nameof(ReturnSalesBasketModel), item}
                });
                    await nextViewModel.LoadSelectedItemsAsync();
                }
                else
                    item.Quantity++;
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

        private async Task DecreaseAsync(ReturnSalesBasketModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    if (item.Quantity > 1)
                    {
                        // Stok Yeri takipli ise locationTransactionBottomSheet aç
                        if (item.LocTracking == 1)
                        {
                            var nextViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketLocationListViewModel>();

                            await Shell.Current.GoToAsync($"{nameof(ReturnSalesBasketLocationListView)}", new Dictionary<string, object>
                        {
                            {nameof(WarehouseModel), WarehouseModel},
                            {nameof(ReturnSalesBasketModel), item}
                            });

                            await nextViewModel.LoadSelectedItemsAsync();
                        }
                        // Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
                        else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
                        {
                            await Shell.Current.GoToAsync($"{nameof(ReturnSalesBasketSeriLotListView)}", new Dictionary<string, object>
                        {
                            {nameof(WarehouseModel), WarehouseModel},
                            {nameof(ReturnSalesBasketModel), item}
                        });
                        }
                        // Stok yeri ve SeriLot takipli değilse
                        else
                        {
                            item.Quantity--;
                        }
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

        private async Task LoadLocationItemsAsync(ReturnSalesBasketModel item)
        {
            try
            {
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, item.ItemReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var location in result.Data)
                    {
                        Locations.Add(Mapping.Mapper.Map<LocationModel>(location));
                    }
                }
                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }
        }

        private async Task LocationCloseAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            });
        }

        private async Task LocationIncreaseAsync(LocationModel locationModel)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                locationModel.InputQuantity++;
            }
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync($"{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LocationConfirmAsync(LocationModel locationModel)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                if (Locations.Count > 0)
                {
                    double totalInputQuantity = 0;
                    foreach (var location in Locations)
                    {
                        if (location.InputQuantity > 0)
                        {
                            totalInputQuantity += location.InputQuantity;
                        }
                    }
                    SelectedInputProductBasketModel.Quantity = totalInputQuantity;

                    CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
                }
                else
                {
                    CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LocationDecreaseAsync(LocationModel locationModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (locationModel.InputQuantity > 0)
                {
                    locationModel.InputQuantity -= 1;
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

        private async Task LocationTransactionCloseAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            });
        }

        private async Task NextViewAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (Items.Count == 0)
                {
                    await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
                    return;
                }
                
                await Shell.Current.GoToAsync($"{nameof(ReturnSalesFormView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(Items)] = Items
                });
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
                    var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                    if (!result)
                        return;

                    Items.Clear();
                    await Shell.Current.GoToAsync("..");
                }
                else
                    await Shell.Current.GoToAsync("..");
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
        public async Task LoadPageAsync()
        {
            try
            {
   
                
                if (Items?.Count > 0)
                    Items.Clear();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }
           
        }
    }
}