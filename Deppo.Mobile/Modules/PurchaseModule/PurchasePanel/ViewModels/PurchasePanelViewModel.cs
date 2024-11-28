using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

public partial class PurchasePanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPurchasePanelService _purchasePanelService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private PurchasePanelModel purchasePanelModel = new();

    [ObservableProperty]
    private PurchaseFiche selectedPurchaseFiche;

    public PurchasePanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IPurchasePanelService purchasePanelService)
    {
        _httpClientService = httpClientService;
        _purchasePanelService = purchasePanelService;
        _userDialogs = userDialogs;
        Title = "Satınalma Paneli";
        LoadItemsCommand = new Command(async () => await LoadItemAsync());
        ItemTappedCommand = new Command<PurchaseFiche>(async (purchaseFiche) => await ItemTappedAsync(purchaseFiche));
        WaitingOrderTappedCommand = new Command(async () => await WaitingOrderTappedAsync());
        ReceivedOrderTappedCommand = new Command(async () => await ReceivedOrderTappedAsync());
        SupplierTappedCommand = new Command<Supplier>(async (supplier) => await SupplierTappedAsync(supplier));
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
    }

    public Page CurrentPage { get; set; }
    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command WaitingOrderTappedCommand { get; }
    public Command ReceivedOrderTappedCommand { get; }
    public Command SupplierTappedCommand { get; }
    public Command AllFicheTappedCommand { get; }

    private async Task LoadItemAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CancellationTokenSource cancellationTokenSource = new();

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);

            await Task.WhenAll(TotalOrderCountsAsync(), WaitingProductCountAsync(), ShippedOrderCountsAsync()).ContinueWith((task) =>
            {
                PurchasePanelModel.WaitingOrderCount = PurchasePanelModel.WaitingOrderCount;
                PurchasePanelModel.WaitingOrderCountRate = (double)((double)PurchasePanelModel.WaitingOrderCount / (double)PurchasePanelModel.AmountTotal);
                PurchasePanelModel.ShippedQuantityTotalRate = (double)((double)PurchasePanelModel.ShippedQuantityTotal / (double)PurchasePanelModel.AmountTotal);
            });

            await Task.WhenAll(GetLastTransactionBySupplierAsync(), GetLastFicheAsync());

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

    private async Task ItemTappedAsync(PurchaseFiche purchaseFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await GetLastTransactionAsync(purchaseFiche);
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

    private async Task GetLastTransactionBySupplierAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchasePanelService.GetLastTransactionBySupplier(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                PurchasePanelModel.LastSuplier.Clear();
                foreach (var item in result.Data)
                    PurchasePanelModel.LastSuplier.Add(Mapping.Mapper.Map<Supplier>(item));
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
        }
    }

    private async Task GetLastTransactionAsync(PurchaseFiche purchaseFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchasePanelService.SupplierTransaction(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, purchaseFiche.ReferenceId);

            SelectedPurchaseFiche = purchaseFiche;

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                PurchasePanelModel.LastSupplierTransaction.Clear();
                foreach (var item in result.Data)
                    PurchasePanelModel.LastSupplierTransaction.Add(Mapping.Mapper.Map<SupplierTransaction>(item));
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
        }
    }

    private async Task GetLastFicheAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchasePanelService.GetLastFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                PurchasePanelModel.LastPurchaseFiche.Clear();
                foreach (var item in result.Data)
                    PurchasePanelModel.LastPurchaseFiche.Add(Mapping.Mapper.Map<PurchaseFiche>(item));
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
        }
    }

    private async Task TotalOrderCountsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchasePanelService.TotalOrderCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<PurchasePanelModel>(item));
                    PurchasePanelModel.AmountTotal = value.AmountTotal;
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
        }
    }

    private async Task ShippedOrderCountsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchasePanelService.ShippedOrderCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<PurchasePanelModel>(item));
                    PurchasePanelModel.ShippedQuantityTotal = value.ShippedQuantityTotal;
                }
                //PurchasePanelModel.WaitingOrderCount = PurchasePanelModel.AmountTotal - PurchasePanelModel.ShippedQuantityTotal;
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
        }
    }

    private async Task WaitingOrderTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(PurchasePanelWaitingProductListView)}");
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

    private async Task ReceivedOrderTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(PurchasePanelReceivedProductListView)}");
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

    private async Task WaitingProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchasePanelService.WaitingProductCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<PurchasePanelModel>(item));
                   PurchasePanelModel.WaitingOrderCount = value.WaitingOrderCount;
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
        }
    }


    private async Task AllFicheTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(PurchasePanelAllFicheListView)}");
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

    private async Task SupplierTappedAsync(Supplier supplier)
    {
        if (supplier == null)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SupplierDetailModel supplierDetailModel = new();
            supplierDetailModel.Supplier = supplier;

            await Shell.Current.GoToAsync($"{nameof(SupplierDetailView)}", new Dictionary<string, object> { {
                nameof(SupplierDetailModel), supplierDetailModel
                }
                });
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }
}