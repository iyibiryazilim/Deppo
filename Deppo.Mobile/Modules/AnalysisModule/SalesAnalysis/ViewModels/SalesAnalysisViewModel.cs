using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
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

    public SalesAnalysisViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, ISalesAnalysisService salesAnalysisService)
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
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            await Task.WhenAll(DueDatePassedCustomersCount(), DueDatePassedProductsCount(), ReturnProductReferenceCount(), SoldProductReferenceCount(), GetSalesProductReferenceAnalysisAsync());

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

    private async Task GetSalesProductReferenceAnalysisAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.SalesProductReferenceAnalysis(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, DateTime.Now);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;


                List<SalesProductReferenceAnalysis> cacheItems = new();
                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<SalesProductReferenceAnalysis>(item);
                    cacheItems.Add(value);
                }

                SalesAnalysisModel.SalesProductReferenceAnalysis.Clear();
                foreach (var item in cacheItems.OrderBy(x=> x.ArgumentMonth))
                    SalesAnalysisModel.SalesProductReferenceAnalysis.Add(item);
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }


    /// <summary>
    /// Termin Tarihi Geçen Müşteri Sayısı
    /// </summary>
    /// <returns></returns>
    private async Task DueDatePassedCustomersCount()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.DueDatePassedCustomersCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<SalesAnalysisModel>(item));
                    SalesAnalysisModel.DueDatePassedCustomersCount = value.DueDatePassedCustomersCount;
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

    /// <summary>
    /// Termin Tarihi Geçen Ürün Sayısı
    /// </summary>
    /// <returns></returns>
    private async Task DueDatePassedProductsCount()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.DueDatePassedProductsCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<SalesAnalysisModel>(item));
                    SalesAnalysisModel.DueDatePassedProductsCount = value.DueDatePassedProductsCount;
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

    /// <summary>
    /// İade Ürün Referans Sayısı
    /// </summary>
    /// <returns></returns>
    private async Task ReturnProductReferenceCount()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.ReturnProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<SalesAnalysisModel>(item));
                    SalesAnalysisModel.ReturnProductReferenceCount = value.ReturnProductReferenceCount;
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

    /// <summary>
    /// Satılan Ürün Referans Sayısı
    /// </summary>
    private async Task SoldProductReferenceCount()
    {

        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesAnalysisService.SoldProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<SalesAnalysisModel>(item));
                    SalesAnalysisModel.SoldProductReferenceCount = value.SoldProductReferenceCount;
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
    private async Task LastCustomer()
    {


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

        }
    }
    private async Task LastProduct()
    {

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

        }
    }

}
