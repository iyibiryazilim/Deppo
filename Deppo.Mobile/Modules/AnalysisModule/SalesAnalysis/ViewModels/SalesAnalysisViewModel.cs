using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.ViewModels;

public partial class SalesAnalysisViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISalesAnalysisService _salesAnalysisService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    SalesAnalysisModel salesAnalysisModel = new();

    public SalesAnalysisViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService , ISalesAnalysisService salesAnalysisService)
    {
        _httpClientService = httpClientService;
        _salesAnalysisService = salesAnalysisService;
        _userDialogs = userDialogs;
        Title = "Satış Analizi";
        LoadItemsCommand = new Command(async () => await LoadItemAsync());
    }

    public Command LoadItemsCommand { get; }

    private async Task LoadItemAsync()
    {
        try
        {
            await Task.WhenAll(LastProduct(), LastCustomer(), DueDatePassedCustomersCount(), DueDatePassedProductsCount(), ReturnProductReferenceCount(), SoldProductReferenceCount());

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


    private async Task DueDatePassedCustomersCount()
      {
        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.DueDatePassedCustomersCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                SalesAnalysisModel.DueDatePassedCustomersCount = response;
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
    private async Task DueDatePassedProductsCount()
    {
        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.DueDatePassedProductsCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                SalesAnalysisModel.DueDatePassedProductsCount = response;
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
    private async Task ReturnProductReferenceCount()
    {
        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.ReturnProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                SalesAnalysisModel.ReturnProductReferenceCount = response;
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
    private async Task SoldProductReferenceCount()
    {
        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.SoldProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                SalesAnalysisModel.SoldProductReferenceCount = response;
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
    private async Task LastCustomer()
    {

        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.LastCustomers(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    SalesAnalysisModel.LastCustomer.Add(Mapping.Mapper.Map<Customer>(item));
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
    private async Task LastProduct()
    {

        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.LastProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    SalesAnalysisModel.LastProduct.Add(Mapping.Mapper.Map<Product>(item));
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
