using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.ViewModels;

public partial class ProductAnalysisViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductAnalysisService _productAnalysisService;

    public ProductAnalysisViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IProductAnalysisService productAnalysisService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Ürün Analizi";
        _productAnalysisService = productAnalysisService;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());


    }

    [ObservableProperty]
    ProductAnalysisModel productAnalysisModel = new();

    // public Color[] IOColors {
    //         get;
    //         set;
    //     }

    public Command LoadItemsCommand { get; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            // IOColors = new Color[] { 
            //     Color.FromArgb("#44B0C0"),
            //     Color.FromArgb("#E6BE0C")
            // };
            await Task.Delay(1000);
            ProductAnalysisModel.IORates.Clear();
            ProductAnalysisModel.IORates.Add(new IORateModel("Giriş İşlem Oranı", 52.5));
            ProductAnalysisModel.IORates.Add(new IORateModel("Çıkış İşlem Oranı", 47.5));

            //var tasks = new List<Task>
            //{
            //    GetInputTransactionCountAsync(),
            //    GetOutputTransactionCountAsync(),
            //    GetNegativeStockProductsCountAsync(),
            //    GetLastWarehousesAsync()
            //


            //await Task.WhenAll(GetInputTransactionCountAsync(), GetOutputTransactionCountAsync(),GetNegativeStockProductsCountAsync(), GetLastWarehousesAsync());

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

    private async Task GetInputTransactionCountAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productAnalysisService.GetInputTransactionCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductAnalysisModel>(item);
                    ProductAnalysisModel.InputTransactionCount = obj.InputTransactionCount;
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
            IsBusy = false;
        }
    }

    private async Task GetOutputTransactionCountAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productAnalysisService.GetOutputTransactionCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductAnalysisModel>(item);
                    ProductAnalysisModel.OutputTransactionCount = obj.OutputTransactionCount;
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
            IsBusy = false;
        }
    }

    private async Task GetNegativeStockProductsCountAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productAnalysisService.GetNegativeStockProductsCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductAnalysisModel>(item);
                    ProductAnalysisModel.NegativeStockProductQuantity = obj.NegativeStockProductQuantity;
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
            IsBusy = false;
        }
    }

    private async Task GetLastWarehousesAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productAnalysisService.GetLastWarehousesAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    ProductAnalysisModel.LastWarehouses.Add(Mapping.Mapper.Map<WarehouseModel>(item));
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
