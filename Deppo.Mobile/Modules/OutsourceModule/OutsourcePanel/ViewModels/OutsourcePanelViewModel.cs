using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.ViewModels;

public partial class OutsourcePanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IOutsourcePanelService _outsourcePanelService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    OutsourcePanelModel _outsourcePanelModel = new();

    public OutsourcePanelViewModel(IHttpClientService httpClientService, IOutsourcePanelService outsourcePanelService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _outsourcePanelService = outsourcePanelService;
        _userDialogs = userDialogs;

        Title = "Fason Paneli";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            await Task.WhenAll(
                GetOutsourceOutProductCountAsync(),
                GetOutsourceInProductCountAsync(),
                GetOutsourceTotalProductCountAsync()
            ).ContinueWith(async _ =>
            {
                await GetLastOutsourcesAsync();
                await GetLastOutsourceFichesAsync();
                OutsourcePanelModel.InProductCountTotalRate = (double)((double)OutsourcePanelModel.InProductCount / (double)OutsourcePanelModel.TotalProductCount);
                OutsourcePanelModel.OutProductCountTotalRate = (double)((double)OutsourcePanelModel.OutProductCount / (double)OutsourcePanelModel.TotalProductCount);
            });

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }



    private async Task GetOutsourceOutProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetOutsourceOutProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourcePanelModel>(item);
                        OutsourcePanelModel.OutProductCount = obj.OutProductCount;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetOutsourceInProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetOutsourceInProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourcePanelModel>(item);
                        OutsourcePanelModel.InProductCount = Convert.ToInt32(obj.InProductCount);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetOutsourceTotalProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetOutsourceTotalProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourcePanelModel>(item);
                        OutsourcePanelModel.TotalProductCount = obj.TotalProductCount;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetLastOutsourcesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetLastOutsources(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourceModel>(item);
                        OutsourcePanelModel.Outsources.Add(obj);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetLastOutsourceFichesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetLastOutsourceFiches(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourceFiche>(item);
                        OutsourcePanelModel.LastOutsourceFiche.Add(obj);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

}
