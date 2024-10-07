using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Newtonsoft.Json;
using System.Dynamic;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

[QueryProperty(name: nameof(WarehouseDetailModel), queryId: nameof(WarehouseDetailModel))]
public partial class WarehouseDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IWarehouseDetailService _warehouseDetailService;
    private readonly IUserDialogs _userDialogs;

    public WarehouseDetailViewModel(IHttpClientService httpClientService, IWarehouseService warehouseService, ICustomQueryService customQueryService, IUserDialogs userDialogs, IWarehouseDetailService warehouseDetailService)
    {
        Title = "Ambar Detayı";
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;
        _warehouseDetailService = warehouseDetailService;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
    }

    [ObservableProperty]
    private WarehouseDetailModel warehouseDetailModel = null!;

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }
    public Command OutputQuantityTappedCommand { get; }
    public Command AllFicheTappedCommand { get; }

    #endregion Commands

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            _userDialogs.Loading("Loading Items...");

            await Task.Delay(1000);
            await Task.WhenAll(GetInputQuantityAsync(), GetLastTransactionsAsync(), GetOutputQuantityAsync());

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

    private async Task GetInputQuantityAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseDetailService.GetInputQuantity(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseDetailModel.Warehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<WarehouseDetailModel>(item);
                    WarehouseDetailModel.InputQuantity = value.InputQuantity;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetOutputQuantityAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseDetailService.GetOutputQuantity(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseDetailModel.Warehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<WarehouseDetailModel>(item);
                    WarehouseDetailModel.OutputQuantity = value.OutputQuantity;
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetLastTransactionsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseDetailService.GetLastFiches(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, WarehouseDetailModel.Warehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                WarehouseDetailModel.Transactions.Clear();
                foreach (var item in result.Data)
                    WarehouseDetailModel.Transactions.Add(Mapping.Mapper.Map<WarehouseFiche>(item));
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
            await Shell.Current.GoToAsync($"{nameof(WarehouseInputTransactionView)}", new Dictionary<string, object>
            {
                ["Warehouse"] = WarehouseDetailModel.Warehouse
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
            await Shell.Current.GoToAsync($"{nameof(WarehouseOutputTransactionView)}", new Dictionary<string, object>
            {
                ["Warehouse"] = WarehouseDetailModel.Warehouse
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

            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(WarehouseDetailAllFicheListView)}", new Dictionary<string, object> { {
                nameof(WarehouseDetailModel), WarehouseDetailModel
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
}