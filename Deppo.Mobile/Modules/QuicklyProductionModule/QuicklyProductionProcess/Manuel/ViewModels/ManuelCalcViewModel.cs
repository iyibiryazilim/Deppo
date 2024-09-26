using Android.App;
using AndroidX.AppCompat.View.Menu;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using static Java.Text.Normalizer;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

[QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
public partial class ManuelCalcViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseService _warehouseService;
    private readonly ILocationService _locationService;

    [ObservableProperty]
    private QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;


    [ObservableProperty]
    public ObservableCollection<LocationTransactionModel> selectedLocationTransactions = new();
    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    public QuicklyBomSubProductModel selectedItem = new();



      public ObservableCollection<LocationModel> SelectedLocations { get; } = new();
    public ObservableCollection<LocationModel> Locations { get; } = new();




    public ManuelCalcViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IWarehouseService warehouseService, ILocationService locationService)
    {
        _httpClientService = httpClientService;
        _locationTransactionService = locationTransactionService;
        _userDialogs = userDialogs;
        _warehouseService = warehouseService;
        Title = "Ürün Detayı";

        IncreaseCommand = new Command(async () => await IncreaseAsync());

        DecreaseCommand = new Command(async () => await DecreaseAsync());

        NextViewCommand = new Command(async () => await NextViewAsync());
        _locationService = locationService;


        BackCommand = new Command(async () => await BackAsync());

        AddConsumableItemCommand = new Command(async () => await AddConsumableItemAsync());

        LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
        LocationTransactionIncreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
        LocationTransactionDecreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
        LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
        LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

        SubIncreaseCommand = new Command<QuicklyBomSubProductModel>(async (item) => await SubIncreaseAsync(item));
        SubDecreaseCommand = new Command<QuicklyBomSubProductModel>(async (item) => await SubDecreaseAsync(item));




        //Locations
        LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
        LocationCloseCommand = new Command(async () => await LocationCloseAsync());
        LocationConfirmCommand = new Command(async () => await LocationConfirmAsync());
        LocationTappedCommand = new Command<LocationModel>(async (item) => await LocationTappedAsync(item));
        LocationIncreaseCommand = new Command<LocationModel>(async (item) => await LocationIncreaseAsync(item));
        LocationDecreaseCommand = new Command<LocationModel>(async (item) => await LocationDecraseAsync(item));





    }

    public ContentPage CurrentPage { get; set; } = null!;

    #region Commands

    public Command ShowProductViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command DeleteItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }
    public Command AddConsumableItemCommand { get; }

    //LocationTransaction
    public Command LoadMoreLocationTransactionsCommand { get; }
    public Command<LocationTransactionModel> LocationTransactionIncreaseCommand { get; }
    public Command<LocationTransactionModel> LocationTransactionDecreaseCommand { get; }
    public Command LocationTransactionConfirmCommand { get; }
    public Command LocationTransactionCloseCommand { get; }

    public Command ConfirmLocationTransactionCommand { get; }

    //SubProduct Increase Decrease
    public Command<QuicklyBomSubProductModel> SubIncreaseCommand { get; }

    public Command<QuicklyBomSubProductModel> SubDecreaseCommand { get; }





    //Locations
    public Command LoadMoreLocationsCommand { get; }
    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }
    public Command<LocationModel> LocationTappedCommand { get; }




    #endregion Commands

    private async Task IncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel is not null && QuicklyBomProductBasketModel.QuicklyBomProduct.LocTracking == 1)
            {
                await LoadWarehouseLocationsAsync();
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else if(QuicklyBomProductBasketModel is not null)
                {
                   QuicklyBomProductBasketModel.BOMQuantity += 1;
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

    private async Task DecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel is not null && QuicklyBomProductBasketModel.QuicklyBomProduct.LocTracking == 1)
            {
                await LoadWarehouseLocationsAsync();
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else if (QuicklyBomProductBasketModel is not null && QuicklyBomProductBasketModel.BOMQuantity > 0)
            {
                QuicklyBomProductBasketModel.BOMQuantity -= 1;
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel != null)
            {
                var result = await _userDialogs.ConfirmAsync("Çıkıyorsunuz Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                    return;



                QuicklyBomProductBasketModel.SubProducts.Clear();
                LocationTransactions.Clear();
                SelectedLocationTransactions.Clear();

                selectedItem = null;
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.GoToAsync("..");
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
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ManuelFormListView)}", new Dictionary<string, object>
            {
                [nameof(QuicklyBomProductBasketModel)] = QuicklyBomProductBasketModel
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

    // + işareti tıklanınca çalışacak olan fonksiyon
    private async Task AddConsumableItemAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel.QuicklyBomProduct != null)
                await Shell.Current.GoToAsync($"{nameof(ManuelCalcOutWarehouseListView)}");
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

    public async Task SubIncreaseAsync(QuicklyBomSubProductModel item)
    {
        if (IsBusy)
            return;
        try
        {

            if (item is not null)
            {
                SelectedItem = item;
                if (item.ProductModel.LocTracking == 1)
                {
                    await LoadLocationTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    if (item.SubBOMQuantity < item.ProductModel.StockQuantity)
                        item.SubBOMQuantity = item.SubBOMQuantity + 1;
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

    public async Task SubDecreaseAsync(QuicklyBomSubProductModel item)
    {
        if (IsBusy)
            return;
        try
        {

            if (item is not null)
            {
                SelectedItem = item;
                if (item.ProductModel.LocTracking == 1)
                {
                    await LoadLocationTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    if (item.SubBOMQuantity < item.ProductModel.StockQuantity)
                        item.SubBOMQuantity = item.SubBOMQuantity - 1;
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

    public async Task LoadLocationTransactionsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Load Location Items...");
          //  await Task.Delay(1000);
            LocationTransactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ProductModel.ReferenceId,
                warehouseNumber: SelectedItem.WarehouseModel.Number,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    LocationTransactionModel model = Mapping.Mapper.Map<LocationTransactionModel>(item);
                    if(model != null)
                    {
                        if(selectedItem.LocationTransactions.Any(x=>x.LocationCode == model.LocationCode))
                        {
                            model.OutputQuantity = SelectedItem.LocationTransactions.FirstOrDefault(x=>x.ReferenceId == model.ReferenceId).OutputQuantity;
                        }
                    }
                    LocationTransactions.Add(model);
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

    public async Task LoadMoreLocationTransactionsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Load Location Items...");
          //  await Task.Delay(1000);
            LocationTransactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ProductModel.ReferenceId,
                warehouseNumber: SelectedItem.WarehouseModel.Number,
                skip: LocationTransactions.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    LocationTransactionModel model = Mapping.Mapper.Map<LocationTransactionModel>(item);
                    if (model != null)
                    {
                        if (selectedItem.LocationTransactions.Any(x => x.LocationCode == model.LocationCode))
                        {
                            model.OutputQuantity = SelectedItem.LocationTransactions.FirstOrDefault(x => x.LocationCode == model.LocationCode).OutputQuantity;
                        }
                    }
                    LocationTransactions.Add(model);
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

    public async Task LocationTransactionIncreaseAsync(LocationTransactionModel item)
    {
        if (IsBusy)
            return;

        try
            {
            IsBusy = true;

            if (item is not null)
            {
                var totalQuantity =  LocationTransactions.Sum(x => x.OutputQuantity);
                if (totalQuantity >= SelectedItem.ProductModel.StockQuantity)
                {
                    _userDialogs.Alert("Stok miktarından fazla ürün girişi yapamazsınız.", "Uyarı", "Tamam");
                    return;
                }
                else
                {
                    if (item.OutputQuantity < item.Quantity)
                    {
                        item.OutputQuantity += 1;
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

    public async Task LocationTransactionDecreaseAsync(LocationTransactionModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                if (item.OutputQuantity > 0)
                {
                    item.OutputQuantity -= 1;
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

    public async Task LocationTransactionConfirmAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (LocationTransactions.Count > 0)
            {
                SelectedLocationTransactions.Clear();
                foreach (var item in LocationTransactions.Where(x => x.OutputQuantity > 0))
                {
                    if(SelectedItem.LocationTransactions.Any(x => x.ReferenceId == item.ReferenceId))
                    {
                        SelectedItem.LocationTransactions.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).OutputQuantity = item.OutputQuantity;
                    }
                    else
                    SelectedItem.LocationTransactions.Add(item);
                }
                SelectedItem.SubBOMQuantity = SelectedItem.LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
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

    public async Task LocationTransactionCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
        });
    }





















    private async Task LoadWarehouseLocationsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Locations.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, QuicklyBomProductBasketModel.WarehouseNumber, QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
                    foreach (var location in Locations)
                    {
                        var matchingItem = quicklyBomProductBasketModel.MainLocations.FirstOrDefault(item => item.ReferenceId == location.ReferenceId);
                        if (matchingItem != null)
                        {
                            location.InputQuantity = matchingItem.InputQuantity;
                        }
                    }
                }
            }


            _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
    }
    private async Task LoadMoreWarehouseLocationsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Locations.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, QuicklyBomProductBasketModel.WarehouseNumber, QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId, string.Empty, LocationTransactions.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
                    foreach (var location in Locations)
                    {
                        var matchingItem = quicklyBomProductBasketModel.MainLocations.FirstOrDefault(item => item.ReferenceId == location.ReferenceId);
                        if (matchingItem != null)
                        {
                            location.InputQuantity = matchingItem.InputQuantity;
                        }
                    }
                }
            }


            _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
    }
    private async Task LocationCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task LocationTappedAsync(LocationModel locationModel)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (locationModel.IsSelected != true)
            {
                var tappedItem = Locations.ToList().FirstOrDefault(x => x.Code == locationModel.Code);
                if (tappedItem != null)
                    tappedItem.IsSelected = true;
                SelectedLocations.Add(locationModel);
            }
            else
            {
                var tappedItem = Locations.ToList().FirstOrDefault(x => x.Code == locationModel.Code);
                if (tappedItem != null)
                    tappedItem.IsSelected = false;
                SelectedLocations.Remove(locationModel);
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }


    private async Task LocationIncreaseAsync(LocationModel locationModel)
    {

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            double count = (double)Locations.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);

             if (locationModel is not null)
            {
                locationModel.InputQuantity++;
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

    private async Task LocationDecraseAsync(LocationModel locationModel)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (locationModel is not null)
            {
                if (locationModel.InputQuantity > 0)
                    locationModel.InputQuantity--;

                if (locationModel.InputQuantity == 0)
                    locationModel.IsSelected = false;

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

    private async Task LocationConfirmAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (Locations.Count > 0)
            {

                double count = (double)Locations.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);

                if (QuicklyBomProductBasketModel is not null)
                {
                    foreach (var i in Locations.Where(x => x.InputQuantity > 0))
                    {

                        if (QuicklyBomProductBasketModel.MainLocations.Any(x => x.ReferenceId == i.ReferenceId))
                        {
                           QuicklyBomProductBasketModel.MainLocations.FirstOrDefault(x => x.ReferenceId == i.ReferenceId).InputQuantity = i.InputQuantity;
                        }
                        else
                        {
                            QuicklyBomProductBasketModel.MainLocations.Add(i);
                        }

                    }
                    QuicklyBomProductBasketModel.BOMQuantity = (double)QuicklyBomProductBasketModel.MainLocations.Sum(x => x.InputQuantity);

                    CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
                }
               
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }







}