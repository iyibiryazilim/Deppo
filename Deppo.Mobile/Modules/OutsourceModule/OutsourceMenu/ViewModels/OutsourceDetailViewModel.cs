using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;

[QueryProperty(name: nameof(OutsourceDetailModel), queryId: nameof(OutsourceDetailModel))]
public partial class OutsourceDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IOutsourcePanelService _outsourcePanelService;

    [ObservableProperty]
    private OutsourceDetailModel outsourceDetailModel = null!;

    public OutsourceDetailViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IOutsourcePanelService outsourcePanelService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _outsourcePanelService = outsourcePanelService;

        Title = "Fason Detayý";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());

        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());

        ItemTappedCommand = new Command<OutsourceFiche>(async (outsourceFiche) => await ItemTappedAsync(outsourceFiche));

        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
        GetLastTransactionCommand = new Command<OutsourceFiche>(async (outsourceFiche) => await GetLastTransactionAsync(outsourceFiche));
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }

    public Command OutputQuantityTappedCommand { get; }

    public Command AllFicheTappedCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command GetLastTransactionCommand { get; }

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            await Task.WhenAll(GetInputQuantityAsync(httpClient), GetOutputQuantityAsync(httpClient), OutsourceInputOutputQuantitiesAsync(httpClient), GetLastFicheAsync());

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetInputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _outsourcePanelService.GetOutsourceInProductCountByProduct(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, OutsourceDetailModel.Outsource.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<OutsourceDetailModel>(item);
                    OutsourceDetailModel.InputQuantity = value.InputQuantity;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetOutputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _outsourcePanelService.GetOutsourceOutProductCountByProduct(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, OutsourceDetailModel.Outsource.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<OutsourceDetailModel>(result.Data);
                OutsourceDetailModel.OutputQuantity = obj.OutputQuantity;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }

    private async Task OutsourceInputOutputQuantitiesAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _outsourcePanelService.OutsourceInputOutputQuantities(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, DateTime.Now, OutsourceDetailModel.Outsource.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                List<OutsourceDetailInputOutputModel> cacheItems = new();

                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<OutsourceDetailInputOutputModel>(item);
                    cacheItems.Add(value);
                }

                OutsourceDetailModel.OutsourceDetailInputOutputModels.Clear();
                foreach (var item in cacheItems.OrderBy(x => x.ArgumentDay))
                    OutsourceDetailModel.OutsourceDetailInputOutputModels.Add(item);
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task GetLastFicheAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetLastOutsourceFiches(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                OutsourceDetailModel.LastFiches.Clear();
                foreach (var item in result.Data)
                    OutsourceDetailModel.LastFiches.Add(Mapping.Mapper.Map<OutsourceFiche>(item));
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

    private async Task InputQuantityTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(OutsourceInputTransactionView)}", new Dictionary<string, object>
            {
                [nameof(OutsourceDetailModel)] = OutsourceDetailModel
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OutputQuantityTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(OutsourceOutputTransactionView)}", new Dictionary<string, object>
            {
                [nameof(OutsourceDetailModel)] = OutsourceDetailModel
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AllFicheTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(OutsourceDetailAllFichesView)}", new Dictionary<string, object> { {
                nameof(OutsourceDetailModel), OutsourceDetailModel
                }
                });
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetLastTransactionAsync(OutsourceFiche outsourceFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetLastOutsourceTransactions(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, outsourceFiche.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                OutsourceDetailModel.LastTransactions.Clear();

                foreach (var item in result.Data)
                    OutsourceDetailModel.LastTransactions.Add(Mapping.Mapper.Map<OutsourceTransaction>(item));
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

    private async Task ItemTappedAsync(OutsourceFiche outsourceFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await GetLastTransactionAsync(outsourceFiche);
            CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

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
}