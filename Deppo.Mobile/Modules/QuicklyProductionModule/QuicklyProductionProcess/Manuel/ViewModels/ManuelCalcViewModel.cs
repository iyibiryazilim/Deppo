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
        LoadLocationTransactionsCommand = new Command(async () => await LoadLocationTransactionsAsync());
        LocationTransactionIncreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
        LocationTransactionDecreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
        LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
        LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
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
    public Command LoadLocationTransactionsCommand { get; }

    public Command LoadMoreLocationTransactionsCommand { get; }
    public Command<LocationTransactionModel> LocationTransactionIncreaseCommand { get; }
    public Command<LocationTransactionModel> LocationTransactionDecreaseCommand { get; }
    public Command LocationTransactionConfirmCommand { get; }
    public Command LocationTransactionCloseCommand { get; }

    public Command ConfirmLocationTransactionCommand { get; }

    //SubProduct Increase Decrease
    public Command SubIncreaseCommand { get; }

    public Command SubDecreaseCommand { get; }

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

    public async Task SubIncreaseAsync(BOMSubProductModel item)
    {
    }

    public async Task SubDecreaseAsync(BOMSubProductModel item)
    {
    }

    public async Task LoadLocationTransactionsAsync()
    {
    }

    public async Task LoadMoreLocationTransactionsAsync()
    {
    }

    public async Task LocationTransactionIncreaseAsync(LocationTransactionModel item)
    {
    }

    public async Task LocationTransactionDecreaseAsync(LocationTransactionModel item)
    {
    }

    public async Task LocationTransactionConfirmAsync()
    {
    }

    public async Task LocationTransactionCloseAsync()
    {
    }

    public async Task ConfirmLocationTransactionAsync()
    {
    }
}