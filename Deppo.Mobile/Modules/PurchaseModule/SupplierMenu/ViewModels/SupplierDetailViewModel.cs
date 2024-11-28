using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.ActionModels.SupplierActionModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels.ActionViewModels;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views.ActionViews;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views.ActionViews;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

[QueryProperty(name: nameof(SupplierDetailModel), queryId: nameof(SupplierDetailModel))]
public partial class SupplierDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISupplierDetailService _supplierDetailService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private SupplierDetailModel supplierDetailModel = null!;

    [ObservableProperty]
    private PurchaseFiche selectedPurchaseFiche;

    public ObservableCollection<SupplierDetailActionModel> SupplierActionModels { get; } = new();

    public SupplierDetailViewModel(IHttpClientService httpClientService, ISupplierDetailService supplierDetailService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _supplierDetailService = supplierDetailService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;

        Title = "Tedarikçi Detayı";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());
        ActionModelProcessTappedCommand = new Command(async () => await ActionModelProcessTappedAsync());
        ActionModelsTappedCommand = new Command<SupplierDetailActionModel>(async (model) => await ActionModelsTappedAsync(model));
        AllFicheListTappedCommand = new Command(async () => await AllFicheTappedAsync());
        FicheTappedCommand = new Command<PurchaseFiche>(async (purchaseFiche) => await FicheTappedAsync(purchaseFiche));

        GoToWaitingOrderCommand = new Command(async () => await GoToWaitingOrderAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command FicheTappedCommand { get; }
    public Command InputQuantityTappedCommand { get; }

    public Command OutputQuantityTappedCommand { get; }
    public Command ActionModelProcessTappedCommand { get; }
    public Command ActionModelsTappedCommand { get; }
    public Command AllFicheListTappedCommand { get; }

    public Command GoToWaitingOrderCommand { get; }

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            await Task.WhenAll(GetInputQuantityAsync(httpClient), GetOutputQuantityAsync(httpClient), GetSupplierInputOutputQuantitiesAsync(httpClient), GetLastFichesAsync(httpClient), GetWaitingProductReferenceCountAsync(httpClient));

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata...");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetInputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _supplierDetailService.GetInputQuantity(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<SupplierDetailModel>(result.Data);
                SupplierDetailModel.InputQuantity = obj.InputQuantity;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetOutputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _supplierDetailService.GetOutputQuantity(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<SupplierDetailModel>(result.Data);
                SupplierDetailModel.OutputQuantity = obj.OutputQuantity;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetSupplierInputOutputQuantitiesAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _supplierDetailService.SupplierInputOutputQuantities(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                dateTime: DateTime.Now,
                supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                List<SupplierDetailInputOutputModel> cacheItems = new();

                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<SupplierDetailInputOutputModel>(item);
                    cacheItems.Add(value);
                }

                SupplierDetailModel.SupplierDetailInputOutputModels.Clear();
                foreach (var item in cacheItems.OrderBy(x => x.ArgumentDay))
                    SupplierDetailModel.SupplierDetailInputOutputModels.Add(item);
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetLastFichesAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _supplierDetailService.GetLastFichesBySupplier(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SupplierDetailModel.LastFiches.Add(Mapping.Mapper.Map<PurchaseFiche>(item));
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

    private async Task FicheTappedAsync(PurchaseFiche purchaseFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await LoadFicheTransactionsAsync(purchaseFiche);
            await Task.Delay(500);
            CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

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

    private async Task LoadFicheTransactionsAsync(PurchaseFiche purchaseFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            SupplierDetailModel.Transactions.Clear();

            var result = await _supplierDetailService.GetTransactionsByFiche(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheRefenceId: purchaseFiche.ReferenceId
            );
            SelectedPurchaseFiche = purchaseFiche;

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SupplierDetailModel.Transactions.Add(Mapping.Mapper.Map<SupplierTransaction>(item));
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

    private async Task InputQuantityTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(SupplierInputTransactionView)}", new Dictionary<string, object>
            {
                [nameof(SupplierDetailModel)] = SupplierDetailModel
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OutputQuantityTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(SupplierOutputTransactionView)}", new Dictionary<string, object>
            {
                [nameof(SupplierDetailModel)] = SupplierDetailModel
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ActionModelProcessTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.Loading("Yükleniyor");
            await LoadActionModelAsync();

            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.HalfExpanded;

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

    private async Task LoadActionModelAsync()
    {
        try
        {
            SupplierActionModels.Clear();

            SupplierActionModels.Add(new SupplierDetailActionModel
            {
                LineNumber = 1,
                ActionName = "Tedarik Edilebilen Ürünler",
                ActionUrl = $"{nameof(SupplierDetailApprovedProductListView)}",
                Icon = "",
                IsSelected = false
            });

            SupplierActionModels.Add(new SupplierDetailActionModel
            {
                LineNumber = 2,
                ActionName = "Bekleyen Satınalma Siparişleri",
                ActionUrl = $"{nameof(SupplierDetailWaitingPurchaseOrderListView)}",
                Icon = "",
                IsSelected = false
            });

            SupplierActionModels.Add(new SupplierDetailActionModel
            {
                LineNumber = 3,
                ActionName = "Sevk Adresleri",
                ActionUrl = $"{nameof(SupplierDetailShipAddressListView)}",
                Icon = "",
                IsSelected = false
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task ActionModelsTappedAsync(SupplierDetailActionModel model)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.Hidden;
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{model.ActionUrl}", new Dictionary<string, object>
            {
                [nameof(SupplierDetailModel)] = SupplierDetailModel
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

    private async Task AllFicheTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(SupplierDetailAllFicheListView)}", new Dictionary<string, object>
            {
                [nameof(SupplierDetailModel)] = SupplierDetailModel
            });
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

    private async Task GoToWaitingOrderAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(SupplierDetailWaitingPurchaseOrderListView)}", new Dictionary<string, object>
            {
                [nameof(SupplierDetailModel)] = SupplierDetailModel,
            });
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

    private async Task GetWaitingProductReferenceCountAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _supplierDetailService.GetWaitingProductReferenceCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<SupplierDetailModel>(result.Data);
                SupplierDetailModel.WaitingProductReferenceCount = obj.WaitingProductReferenceCount;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }
}