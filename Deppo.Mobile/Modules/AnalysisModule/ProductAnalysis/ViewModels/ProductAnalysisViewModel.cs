using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
using Deppo.Mobile.Core.Models.SalesModels;
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

    [ObservableProperty]
    private ProductAnalysisModel productAnalysisModel = new();

    public ProductAnalysisViewModel(
        IHttpClientService httpClientService,
        IProductAnalysisService productAnalysisService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _productAnalysisService = productAnalysisService;
        _userDialogs = userDialogs;

        Title = "Malzeme Analizi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

    public Command LoadItemsCommand { get; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            await Task.WhenAll(
                GetNegativeStockProductsCountAsync(),
                GetTotalProductCountAsync(),
                GetInStockProductCountAsync(),
                GetOutStockProductCountAsync(),
                GetInputOutputReferenceAnalysisAsync()
			);

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

    private async Task GetTotalProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productAnalysisService.GetTotalProductCountAsync(httpClient, _httpClientService.FirmNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductAnalysisModel>(item);
                    ProductAnalysisModel.TotalProductCount = obj.TotalProductCount;
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

    private async Task GetInStockProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productAnalysisService.GetInStockProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductAnalysisModel>(item);
                    ProductAnalysisModel.InStockProductCount = obj.InStockProductCount;
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

    private async Task GetOutStockProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productAnalysisService.GetOutStockProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<ProductAnalysisModel>(item);
                    ProductAnalysisModel.OutStockProductCount = obj.OutStockProductCount;
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

	private async Task GetInputOutputReferenceAnalysisAsync()
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productAnalysisService.InputOutputProductReferenceAnalysis(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, DateTime.Now);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;


				List<InputOutputProductReferenceAnalysis> cacheItems = new();
				foreach (var item in result.Data)
				{
					var value = Mapping.Mapper.Map<InputOutputProductReferenceAnalysis>(item);
					cacheItems.Add(value);
				}

				ProductAnalysisModel.InputOutputProductReferenceAnalysis.Clear();
				foreach (var item in cacheItems.OrderBy(x => x.ArgumentMonth))
					ProductAnalysisModel.InputOutputProductReferenceAnalysis.Add(item);
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