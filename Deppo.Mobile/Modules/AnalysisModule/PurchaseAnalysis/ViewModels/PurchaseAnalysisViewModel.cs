using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
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
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Task.WhenAll(DueDatePassedSupplierCount(), DueDatePassedProductsCount(), ReturnProductReferenceCount(), PurchaseProductReferenceCount(),GetPurchaseProductReferenceAnalysisAsync());

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

    private async Task GetPurchaseProductReferenceAnalysisAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.PurchaseProductReferenceAnalysis(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, DateTime.Now);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;


                List<PurchaseProductReferenceAnalysis> cacheItems = new();
                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<PurchaseProductReferenceAnalysis>(item);
                    cacheItems.Add(value);
                }

                PurchaseAnalysisModel.PurchaseProductReferenceAnalysis.Clear();
                foreach (var item in cacheItems.OrderBy(x=> x.ArgumentMonth))
                    PurchaseAnalysisModel.PurchaseProductReferenceAnalysis.Add(item);
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task DueDatePassedSupplierCount()
    {
       
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.DueDatePassedSuppliersCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;


                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<PurchaseAnalysisModel>(item);
                    PurchaseAnalysisModel.DueDatePassedSuppliersCount = value.DueDatePassedSuppliersCount;
                }


            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        
    }
    private async Task DueDatePassedProductsCount()
    {
       
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.DueDatePassedProductsCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<PurchaseAnalysisModel>(item));
                    PurchaseAnalysisModel.DueDatePassedProductsCount = value.DueDatePassedProductsCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
       
    }
    private async Task ReturnProductReferenceCount()
    {
     
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.ReturnProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<PurchaseAnalysisModel>(item));
                    PurchaseAnalysisModel.ReturnProductReferenceCount = value.ReturnProductReferenceCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        
    }
    private async Task PurchaseProductReferenceCount()
    {
        
        try
        {

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _purchaseAnalysisService.PurchaseProductReferenceCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<PurchaseAnalysisModel>(item));
                    PurchaseAnalysisModel.PurchaseProductReferenceCount = value.PurchaseProductReferenceCount;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
       
    }
    private async Task LastSupplier()
    {

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
        
    }
    private async Task LastProduct()
    {

        
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
       
    }
}
