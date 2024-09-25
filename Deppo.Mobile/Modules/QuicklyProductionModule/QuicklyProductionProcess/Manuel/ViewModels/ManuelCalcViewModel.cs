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

    [ObservableProperty]
    private QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;


    [ObservableProperty]
    public ObservableCollection<LocationTransactionModel> selectedLocationTransactions = new();
    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    public QuicklyBomSubProductModel selectedItem = new();




    public ManuelCalcViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IWarehouseService warehouseService)
    {
        _httpClientService = httpClientService;
        _locationTransactionService = locationTransactionService;
        _userDialogs = userDialogs;
        _warehouseService = warehouseService;
        Title = "Ürün Detayı";

        IncreaseCommand = new Command(async () => await IncreaseAsync());

        DecreaseCommand = new Command(async () => await DecreaseAsync());

        NextViewCommand = new Command(async () => await NextViewAsync());

        BackCommand = new Command(async () => await BackAsync());

        AddConsumableItemCommand = new Command(async () => await AddConsumableItemAsync());

        LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
        LocationTransactionIncreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
        LocationTransactionDecreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
        LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
        LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

        SubIncreaseCommand = new Command<QuicklyBomSubProductModel>(async (item) => await SubIncreaseAsync(item));
        SubDecreaseCommand = new Command<QuicklyBomSubProductModel>(async (item) => await SubDecreaseAsync(item));
        ConfirmLocationTransactionCommand = new Command(async () => await ConfirmLocationTransactionAsync());
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

    #endregion Commands

    private async Task IncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel is not null)
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

            if (QuicklyBomProductBasketModel is not null && QuicklyBomProductBasketModel.BOMQuantity > 0)
            {
                QuicklyBomProductBasketModel.BOMQuantity -= 1;
            }
            else
            {
                _userDialogs.Alert("Ürün Miktarı 0'dan büyük olmalıdır. Daha Fazla Azaltamazsınız", "Uyarı", "Tamam");
                return;
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

                QuicklyBomProductBasketModel = null;
                SelectedLocationTransactions.Clear();
                LocationTransactions.Clear();
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
            await Task.Delay(1000);
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
            await Task.Delay(1000);
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
                        SelectedItem.SubBOMQuantity = item.OutputQuantity;
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
                    SelectedItem.SubBOMQuantity = item.OutputQuantity;
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

    public async Task ConfirmLocationTransactionAsync()
    {
    }
}