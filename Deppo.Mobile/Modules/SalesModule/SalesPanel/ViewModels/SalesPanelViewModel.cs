using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;

public partial class SalesPanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISalesPanelService _salesPanelService;

    [ObservableProperty]
    SalesPanelModel salesPanelModel = new();
    public SalesPanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService , ISalesPanelService salesPanelService)
    {
        _salesPanelService = salesPanelService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Satış Paneli";
        LoadItemsCommand = new Command(async () => await LoadItemAsync());
    }
    public Command LoadItemsCommand { get; }

    private async Task LoadItemAsync()
    {
        try
        {
            await Task.WhenAll(GetLastTransactionByCustomerAsync(), GetLastTransactionAsync(), TotalOrderCountsAsync(), ShippedOrderCountsAsync());
           
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

    private async Task GetLastTransactionByCustomerAsync()
    {

        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.GetLastTransactionByCustomer(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    SalesPanelModel.LastCustomer.Add(Mapping.Mapper.Map<Customer>(item));
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
    private async Task GetLastTransactionAsync()
    {

        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.GetLastTransaction(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    SalesPanelModel.LastCustomerTransaction.Add(Mapping.Mapper.Map<CustomerTransaction>(item));
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
    private async Task TotalOrderCountsAsync()
    {

        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.TotalOrderCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                SalesPanelModel.AmountTotal = response;
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
    private async Task ShippedOrderCountsAsync()
    {

        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.ShippedOrderCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

               var response = Convert.ToInt32(result.Data);
                SalesPanelModel.ShippedQuantityTotal = response;
                SalesPanelModel.WaitingOrderCount = SalesPanelModel.AmountTotal - SalesPanelModel.ShippedQuantityTotal;
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
    


}
