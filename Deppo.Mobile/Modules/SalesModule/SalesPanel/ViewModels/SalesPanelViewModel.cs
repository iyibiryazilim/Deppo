using System;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
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
    public SalesPanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService , ISalesPanelService salesPanelService)
    {
        _salesPanelService = salesPanelService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Satış Paneli";
    }
    private int WaitingOrderCount;
    private int TotalOrderCount;
    private int ShippedOrderCount;
    private ObservableCollection<Customer> LastCustomer = new ObservableCollection<Customer>();
    private ObservableCollection<CustomerTransaction> LastCustomerTransaction = new ObservableCollection<CustomerTransaction>();

    private async Task GetLastTransactionByCustomer()
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
                    LastCustomer.Add(Mapping.Mapper.Map<Customer>(item));
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
    private async Task GetLastTransaction()
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
                    LastCustomerTransaction.Add(Mapping.Mapper.Map<CustomerTransaction>(item));
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
    private async Task TotalOrderCounts()
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

                result = Convert.ToInt32(result.Data);
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
    private async Task ShippedOrderCounts()
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

                result = Convert.ToInt32(result.Data);
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
