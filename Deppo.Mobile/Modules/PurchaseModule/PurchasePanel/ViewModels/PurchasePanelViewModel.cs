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

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

public partial class PurchasePanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPurchasePanelService _purchasePanelService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    PurchasePanelModel purchasePanelModel = new();

    public PurchasePanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService , IPurchasePanelService purchasePanelService)
    {
        
        _httpClientService = httpClientService;
        _purchasePanelService = purchasePanelService;
        _userDialogs = userDialogs;
        Title = "Satınalma Paneli";       
        LoadItemsCommand = new Command(async () => await LoadItemAsync());
    }
    public Command LoadItemsCommand { get;  }

    private async Task LoadItemAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Task.WhenAll(GetLastTransactionBySupplierAsync(), GetLastTransactionAsync(), TotalOrderCountsAsync(), ShippedOrderCountsAsync());

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
    private async Task GetLastTransactionAsync()
    {

     
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchasePanelService.SupplierTransaction(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

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
                PurchasePanelModel.WaitingOrderCount = PurchasePanelModel.AmountTotal - PurchasePanelModel.ShippedQuantityTotal;


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
