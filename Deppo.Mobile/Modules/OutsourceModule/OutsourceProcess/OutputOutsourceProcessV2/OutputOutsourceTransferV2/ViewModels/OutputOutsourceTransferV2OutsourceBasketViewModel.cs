using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;
using DevExpress.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels
{


    [QueryProperty(name: nameof(OutputOutsourceTransferV2BasketModel), queryId: nameof(OutputOutsourceTransferV2BasketModel))]

    public partial class OutputOutsourceTransferV2OutsourceBasketViewModel : BaseViewModel
    {

        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly ILocationTransactionService _locationTransactionService;
        private readonly IOutputOutsourceTransferV2Service _outputOutsourceTransferV2Service;
        private readonly IProductService _productService;
        private readonly IServiceProvider _serviceProvider;


        [ObservableProperty]
        OutputOutsourceTransferV2BasketModel? outputOutsourceTransferV2BasketModel = null!;

        [ObservableProperty]
        private OutputOutsourceTransferV2ProductModel? selectedProduct;

        [ObservableProperty]
        GroupLocationTransactionModel? selectedLocationTransaction;

        [ObservableProperty]
        OutputOutsourceTransferV2SubProductModel? outputOutsourceTransferV2SubProductModel;

        [ObservableProperty]
        OutputOutsourceTransferV2SubProductModel selectedSubProductModel;

        [ObservableProperty]
        public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();
        public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();
        public ObservableCollection<LocationModel> Items { get; } = new();
        public ObservableCollection<LocationModel> SelectedSearchItems { get; } = new();
        public ObservableCollection<LocationModel> SelectedItems { get; } = new();


        [ObservableProperty]
        private LocationModel selectedItem;

        public Page CurrentPage { get; set; }

        [ObservableProperty]
        public SearchBar searchText;

        public OutputOutsourceTransferV2OutsourceBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, IOutputOutsourceTransferV2Service outputOutsourceTransferV2Service, IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _locationTransactionService = locationTransactionService;
            _outputOutsourceTransferV2Service = outputOutsourceTransferV2Service;
            _serviceProvider = serviceProvider;


            Title = "Ürün - Reçete";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            IncreaseCommand = new Command(async () => await IncreaseAsync());
            DecreaseCommand = new Command(async () => await DecreaseAsync());
            NextViewCommand = new Command(async () => await NextViewAsync());
            BackCommand = new Command(async () => await BackAsync());

            LeftSwipeShowLocationTransactionCommand = new Command<OutputOutsourceTransferV2SubProductModel>(async (subProductModel) => await LeftSwipeShowLocationTransactionAsync(subProductModel));
            RightSwipeShowLocationCommand = new Command<OutputOutsourceTransferV2SubProductModel>(async (subProductModel) => await RightSwipeShowLocationAsync(subProductModel));




            LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
            LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
            LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
            LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionQuantityTappedAsync(item));
            LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
            LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());




           


        }

        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }

        public Command LoadItemsCommand { get; }

        public Command LeftSwipeShowLocationTransactionCommand { get; }
        public Command RightSwipeShowLocationCommand { get; }

        #region  locationTransaction Command
        public Command LoadMoreLocationTransactionsCommand { get; }
        public Command LocationTransactionIncreaseCommand { get; }
        public Command LocationTransactionDecreaseCommand { get; }
        public Command LocationTransactionQuantityTappedCommand { get; }
        public Command LocationTransactionConfirmCommand { get; }
        public Command LocationTransactionCloseCommand { get; }

        #endregion


        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                _userDialogs.Loading("Yükleniyor...");
                OutputOutsourceTransferV2BasketModel?.OutputOutsourceTransferSubProducts?.Clear();
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _outputOutsourceTransferV2Service.GetObjectSubProducts(httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    productReferenceId: OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.ProductReferenceId,
                    search: "",
                    skip: 0,
                    take: 20

                    );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<OutputOutsourceTransferV2SubProductModel>(product);
                        item.Details = new List<OutputOutsourceTransferSubProductDetailModel>();
                        OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts.Add(item);
                    }

                }

                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

            }
            catch (Exception)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync("Error", "SubProducts not loaded!", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task IncreaseAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                //if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.LocTracking == 1)
                //{

                //    var locationListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferV2MainProductLocationListViewModel>();
                //    locationListViewModel.OutputOutsourceTransferV2BasketModel = OutputOutsourceTransferV2BasketModel;

                //    await locationListViewModel.LoadSelectedItemsAsync();

                //    await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2MainProductLocationListView)}", new Dictionary<string, object>
                //    {
                //        [nameof(OutputOutsourceTransferV2BasketModel)] = OutputOutsourceTransferV2BasketModel
                //    });

                //}
                //else
                //{
                //    //raf takipli değilse
                //    OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity++;

                //    foreach (var subProduct in OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts)
                //    {
                //        subProduct.Quantity = subProduct.BOMQuantity * OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity;
                //    }
                //}

                OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity++;
                // Alt ürünlerin miktarlarını güncelleme
                foreach (var subProduct in OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts)
                {
                    subProduct.Quantity = subProduct.BOMQuantity * OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity;
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

        private async Task DecreaseAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                //if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.LocTracking == 1 && OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity>0)
                //{

                //    var locationListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferV2MainProductLocationListViewModel>();
                //    locationListViewModel.OutputOutsourceTransferV2BasketModel = OutputOutsourceTransferV2BasketModel;
                //    await locationListViewModel.LoadSelectedItemsAsync();

                //    await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2MainProductLocationListView)}", new Dictionary<string, object>
                //    {
                //        [nameof(OutputOutsourceTransferV2BasketModel)] = OutputOutsourceTransferV2BasketModel
                //    });
                //    //show fason ambar raf takipleri
                //    //var locationQuantity = 


                //    //OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity = locationQuantity;
                //    //foreach (var subProduct in OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts)
                //    //{
                //    //    subProduct.Quantity = subProduct.BomQuantity * OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity;
                //    //}
                //}
                //else
                //{
                //    //raf takipli değilse
                //    OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity--;

                //    foreach (var subProduct in OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts)
                //    {
                //        subProduct.Quantity = subProduct.BOMQuantity * OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity;
                //    }
                //}


                // Ana ürünün miktarını azaltma
                

                // 0'dan küçük olup olmadığını kontrol etme

                if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity > 0)
                {

                    OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity--;
                    // Alt ürünlerin miktarlarını güncelleme
                    foreach (var subProduct in OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts)
                    {
                        subProduct.Quantity = subProduct.BOMQuantity * OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity;
                    }

                }
                else if(OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity < 0)
                {
                    _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olamaz", "Uyarı", "Tamam");
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

        private async Task NextViewAsync()
        {
            if (IsBusy)
                return;



            if (OutputOutsourceTransferV2BasketModel is null && OutputOutsourceTransferV2BasketModel.OutsourceWarehouseModel is null && OutputOutsourceTransferV2BasketModel.OutsourceModel is null)
                return;
            try
            {
                IsBusy = true;

                foreach (var subProduct in OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts)
                {
                    var itemDetailTotalQuantity = subProduct.Details.Sum(x => x.Quantity);

                    if (subProduct.StockQuantity <= subProduct.Quantity)
                    {
                        await _userDialogs.AlertAsync(
                   $"{subProduct.ProductName} ürününde yeterli stok bulunmamaktadır.\n" +
                   $"Sarf Edilmek İstenen: {subProduct.Quantity}, Stok Miktarı: {subProduct.StockQuantity}",
                   "Uyarı", "Tamam");
                        return;
                    }

                    else if (subProduct.LocTracking == 1 && subProduct.Quantity != itemDetailTotalQuantity)
                    {
                        _userDialogs.ShowToast($"Ürünün ({subProduct.ProductCode})  miktarı ({subProduct.Quantity}) alt ürünün girilen raf miktarına ({itemDetailTotalQuantity}) eşit olmalıdır!");
                        return;
                    }

                }

                OutputOutsourceTransferV2SubProductModel = SelectedSubProductModel;

                await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2OutsourceFormView)}", new Dictionary<string, object>
                {

                    [nameof(OutputOutsourceTransferV2BasketModel)] = OutputOutsourceTransferV2BasketModel,
                    [nameof(OutputOutsourceTransferV2SubProductModel)] = OutputOutsourceTransferV2SubProductModel,


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

        private async Task BackAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;


                if (OutputOutsourceTransferV2BasketModel is not null)
                {
                    OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.IsSelected = false;
                    OutputOutsourceTransferV2BasketModel = null;
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

        private async Task LeftSwipeShowLocationTransactionAsync(OutputOutsourceTransferV2SubProductModel subProductModel)
        {
           
            SelectedSubProductModel= subProductModel;
                if (IsBusy)
                    return;

                try
                {
                await LoadLocationTransactionsAsync(subProductModel);
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                catch (Exception ex)
                {
                    _userDialogs.Alert(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            
        }

        private async Task RightSwipeShowLocationAsync(OutputOutsourceTransferV2SubProductModel subProductModel)
        {

            SelectedSubProductModel = subProductModel;
            OutputOutsourceTransferV2SubProductModel = subProductModel;
            if (IsBusy)
                return;

            if (IsBusy)
                return;
            try
            {

                if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.LocTracking == 1)
                {

                    var locationListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferV2SubProductLocationListViewModel>();
                    locationListViewModel.OutputOutsourceTransferV2BasketModel = OutputOutsourceTransferV2BasketModel;
                    locationListViewModel.OutputOutsourceTransferV2SubProductModel = OutputOutsourceTransferV2SubProductModel;
                    await locationListViewModel.LoadSelectedItemsAsync();

                    await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2SubProductLocationListView)}", new Dictionary<string, object>
                    {
                        [nameof(OutputOutsourceTransferV2SubProductModel)] = OutputOutsourceTransferV2SubProductModel,
                        [nameof(OutputOutsourceTransferV2BasketModel)] = OutputOutsourceTransferV2BasketModel
                    });
                }
                   
                
            }

            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }

        }

        private async Task LoadLocationTransactionsAsync(OutputOutsourceTransferV2SubProductModel subProductModel)
        {
            subProductModel = SelectedSubProductModel;

            if (SelectedSubProductModel is null)
                return;

            try
            {
                _userDialogs.Loading("Loading Location Transaction Items...");
                LocationTransactions.Clear();
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    productReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : SelectedSubProductModel.ProductReferenceId,
                    variantReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : 0,
                    warehouseNumber: SelectedSubProductModel.OutWarehouseNumber,
                    skip: 0,
                    take: 20,
                    search: ""
                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                        var matchingItem = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                        obj.OutputQuantity = matchingItem?.Quantity ?? 0;
                        LocationTransactions.Add(obj);

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

        private async Task LoadMoreLocationTransactionsAsync()
        {
            if (IsBusy)
                return;

            if (LocationTransactions.Count < 18)
                return;

            if (SelectedSubProductModel is null)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.Loading("Loading more Location Transaction Items...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    productReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : SelectedSubProductModel.ProductReferenceId,
                    variantReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : 0,
                    warehouseNumber: SelectedSubProductModel.OutWarehouseNumber,
                    skip: 0,
                    take: LocationTransactions.Count,
                    search: ""
                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                        var matchingItem = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                        obj.OutputQuantity = matchingItem?.Quantity ?? 0;
                        LocationTransactions.Add(obj);

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

        private async Task LocationTransactionIncreaseAsync(GroupLocationTransactionModel item)
        {
            if (IsBusy)
                return;
            if (item is null)
                return;
            try
            {
                IsBusy = true;

                if (item.RemainingQuantity <= item.OutputQuantity)
                    return;

                item.OutputQuantity += 1;
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

        private async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
        {
            if (IsBusy)
                return;
            if (item is null)
                return;
            try
            {
                IsBusy = true;

                if (item.OutputQuantity == 0)
                    return;

                item.OutputQuantity -= 1;
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

        private async Task LocationTransactionQuantityTappedAsync(GroupLocationTransactionModel item)
        {
            if (IsBusy)
                return;
            if (item is null)
                return;
            try
            {
                IsBusy = true;

                var result = await CurrentPage.DisplayPromptAsync(
                    title: item.ItemCode,
                    message: "Miktarı giriniz",
                    cancel: "Vazgeç",
                    accept: "Tamam",
                    placeholder: item.OutputQuantity.ToString(),
                    keyboard: Keyboard.Numeric
                );

                if (string.IsNullOrEmpty(result))
                    return;

                var quantity = Convert.ToDouble(result);

                if (quantity < 0)
                {
                    _userDialogs.ShowToast("Girilen miktar 0'dan küçük olmamalıdır.");
                    return;
                }

                if (quantity > item.RemainingQuantity)
                {
                    _userDialogs.ShowToast("Girilen miktar, kalan miktarı geçemez.");
                    return;
                }

                if (quantity > SelectedSubProductModel?.StockQuantity)
                {
                    _userDialogs.ShowToast($"Girilen miktar, ilgili ürünün ({SelectedSubProductModel.ProductCode}) stok miktarını {SelectedSubProductModel.StockQuantity} aşmamalıdır.");
                    return;
                }

                var totalQuantity = LocationTransactions.Where(x => x.LocationReferenceId != item.LocationReferenceId).Sum(x => x.OutputQuantity);
                if (totalQuantity + quantity > SelectedSubProductModel?.StockQuantity)
                {
                    _userDialogs.ShowToast($"Toplam girilen miktar, ilgili ürünün ({SelectedSubProductModel.ProductCode}) stok miktarını ({SelectedSubProductModel.StockQuantity}) aşmamalıdır.");
                    return;
                }

                item.OutputQuantity = quantity;

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

        private async Task LocationTransactionConfirmAsync()
        {
            if (IsBusy)
                return;

            if (SelectedSubProductModel is null)
                return;

            try
            {
                IsBusy = true;

                if (!LocationTransactions.Any())
                {
                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
                    return;
                }

                foreach (var item in LocationTransactions.Where(x => x.OutputQuantity <= 0))
                {
                    SelectedSubProductModel.Details.Remove(SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId));
                }

                foreach (var item in LocationTransactions.Where(x => x.OutputQuantity > 0))
                {
                    var selectedLocationTransaction = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId);
                    if (selectedLocationTransaction is not null)
                    {
                        selectedLocationTransaction.Quantity = item.OutputQuantity;
                    }
                    else
                    {
                        SelectedSubProductModel.Details.Add(new OutputOutsourceTransferSubProductDetailModel
                        {
                            LocationReferenceId = item.LocationReferenceId,
                            LocationCode = item.LocationCode,
                            LocationName = item.LocationName,
                            Quantity = item.OutputQuantity,
                            RemainingQuantity = item.RemainingQuantity,
                        });
                    }
                }

                //var totalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => x.OutputQuantity);
                //SelectedSubProductModel.OutputQuantity = totalOutputQuantity;

                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
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

        private async Task LocationTransactionCloseAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            });
        }






    }
}
