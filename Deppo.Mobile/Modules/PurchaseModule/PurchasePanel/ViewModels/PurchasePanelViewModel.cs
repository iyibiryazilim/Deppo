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
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

public partial class PurchasePanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPurchasePanelService _purchasePanelService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    PurchasePanelModel purchasePanelModel = new();

    public PurchasePanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IPurchasePanelService purchasePanelService)
    {

        _httpClientService = httpClientService;
        _purchasePanelService = purchasePanelService;
        _userDialogs = userDialogs;
        Title = "Satınalma Paneli";
        LoadItemsCommand = new Command(async () => await LoadItemAsync());
        ItemTappedCommand = new Command<PurchaseFiche>(async (purchaseFiche) => await ItemTappedAsync(purchaseFiche));
    }

    public Page CurrentPage { get; set; }
    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }

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
            
            await Task.WhenAll(TotalOrderCountsAsync(), ShippedOrderCountsAsync()).ContinueWith((task) =>
            {
               PurchasePanelModel.WaitingOrderCount = PurchasePanelModel.AmountTotal - PurchasePanelModel.ShippedQuantityTotal;
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

}
