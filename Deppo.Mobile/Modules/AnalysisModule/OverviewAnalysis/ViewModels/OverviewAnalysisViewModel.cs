using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

public partial class OverviewAnalysisViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IOverviewAnalysisService _overviewAnalysisService;

    public OverviewAnalysisViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IOverviewAnalysisService overviewAnalysisService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Genel Analiz";
        _overviewAnalysisService = overviewAnalysisService;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

    [ObservableProperty]
    public OverviewAnalysisModel overviewAnalysisModel = new();

    public Command LoadItemsCommand { get; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Task.WhenAll(GetTotalProductCountAsync(), GetTotalInputProductCountAsync(), GetTotalOutputProductCountAsync(),GetInputTransactionCountAsync(),GetOutputTransactionCountAsync());

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
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetTotalProductCountAsync(httpClient, _httpClientService.FirmNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                OverviewAnalysisModel.TotalProductCount = Convert.ToInt32(result.Data);
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

    private async Task GetTotalInputProductCountAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetTotalInputProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                OverviewAnalysisModel.TotalInputProductCount = Convert.ToInt32(result.Data);
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

    private async Task GetTotalOutputProductCountAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetTotalOutputProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                OverviewAnalysisModel.TotalOutputProductCount = Convert.ToInt32(result.Data);
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

    private async Task GetInputTransactionCountAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _overviewAnalysisService.GetInputTransactionCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                OverviewAnalysisModel.InputTransactionCount = Convert.ToInt32(result.Data);
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
            var result = await _overviewAnalysisService.GetOutputTransactionCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                OverviewAnalysisModel.TotalOutputProductCount = Convert.ToInt32(result.Data);
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
