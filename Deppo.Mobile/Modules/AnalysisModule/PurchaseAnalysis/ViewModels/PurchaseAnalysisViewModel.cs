using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.ViewModels;

public partial class PurchaseAnalysisViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IPurchaseAnalysisService _purchaseAnalysisService;

    [ObservableProperty]
    PurchaseAnalysisModel purchaseAnalysisModel = new();

    public PurchaseAnalysisViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService , IPurchaseAnalysisService purchaseAnalysisService)
    {
        _userDialogs = userDialogs;
        _purchaseAnalysisService = purchaseAnalysisService;
        _httpClientService = httpClientService;

        Title = "Satın Alma Analizi";
        LoadItemsCommand = new Command(async () => await LoadItemAsync());

    }
    public Command LoadItemsCommand { get; }

    private async Task LoadItemAsync()
    {
        try
        {
            await Task.WhenAll(LastProduct(), LastSupplier(), DueDatePassedSupplierCount(), DueDatePassedProductsCount(), ReturnProductReferenceCount(), PurchaseProductReferenceCount());

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

    private async Task DueDatePassedSupplierCount()
    {
        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.DueDatePassedSuppliersCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                PurchaseAnalysisModel.DueDatePassedSuppliersCount = response;
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
            var result = await _purchaseAnalysisService.DueDatePassedProductsCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                PurchaseAnalysisModel.DueDatePassedProductsCount = response;
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
            var result = await _purchaseAnalysisService.ReturnProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                PurchaseAnalysisModel.ReturnProductReferenceCount = response;
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
    private async Task PurchaseProductReferenceCount()
    {
        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.PurchaseProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                var response = Convert.ToInt32(result.Data);
                PurchaseAnalysisModel.PurchaseProductReferenceCount = response;
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
    private async Task LastSupplier()
    {

        if (IsBusy)
            return;
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.LastSuppliers(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    PurchaseAnalysisModel.LastSupplier.Add(Mapping.Mapper.Map<Supplier>(item));
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
            var result = await _purchaseAnalysisService.LastProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    PurchaseAnalysisModel.LastProduct.Add(Mapping.Mapper.Map<Product>(item));
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
