using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.ActionModels.ProductActionModels;
using Deppo.Mobile.Core.ActionModels.WarehouseActionModels;
using Deppo.Mobile.Core.Models.AnalysisModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views.ActionViews;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views.ActionViews;
using DevExpress.Maui.Controls;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Dynamic;
using static DevExpress.Data.Filtering.Helpers.SubExprHelper.UiThreadRowStubSubExpressive;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

[QueryProperty(name: nameof(WarehouseDetailModel), queryId: nameof(WarehouseDetailModel))]
public partial class WarehouseDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IWarehouseDetailService _warehouseDetailService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseDetailModel warehouseDetailModel = null!;

    [ObservableProperty]
    private WarehouseFiche selectedWarehouseFiche = null!;

    public Page CurrentPage { get; set; }

    public ObservableCollection<WarehouseDetailActionModel> WarehouseActionModels { get; } = new();

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
        ItemTappedCommand = new Command<WarehouseFiche>(async (warehouseFiche) => await ItemTappedAsync(warehouseFiche));
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());

        ActionModelProcessTappedCommand = new Command(async () => await ActionModelProcessTappedAsync());
        ActionModelsTappedCommand = new Command<WarehouseDetailActionModel>(async (model) => await ActionModelsTappedAsync(model));

        GoToWarehouseTotalListCommand = new Command(async () => await GoToWarehouseTotalListAsync());
    }

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }
    public Command OutputQuantityTappedCommand { get; }
    public Command AllFicheTappedCommand { get; }

    public Command ItemTappedCommand { get; }

    public Command ActionModelProcessTappedCommand { get; }

    public Command ActionModelsTappedCommand { get; }

    public Command GoToWarehouseTotalListCommand { get; }

    #endregion Commands

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            _userDialogs.Loading("Loading Items...");

            await Task.Delay(1000);
            await Task.WhenAll(GetInputQuantityAsync(), GetLastTransactionFicheAsync(), GetOutputQuantityAsync(), GetProductInputOutputQuantitiesAsync(), WarehouseReferenceCountAsync(httpClient), WarehouseLastTransactionDateAsync());

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

    private async Task GetProductInputOutputQuantitiesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseDetailService.ProductInputOutputReferences(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, DateTime.Now, WarehouseDetailModel.Warehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                List<WarehouseDetailProductReferenceModel> cacheItems = new();

                cacheItems.Clear();
                foreach (var item in result.Data)
                    cacheItems.Add(Mapping.Mapper.Map<WarehouseDetailProductReferenceModel>(item));

                WarehouseDetailModel.ProductReferences.Clear();
                foreach (var item in cacheItems.OrderBy(x => x.ArgumentDay))
                    WarehouseDetailModel.ProductReferences.Add(item);
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

    private async Task ItemTappedAsync(WarehouseFiche warehouseFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await GetLastTransactionAsync(warehouseFiche);
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

    private async Task GetLastTransactionAsync(WarehouseFiche warehouseFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseDetailService.GetLastTransaction(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseFiche.ReferenceId, WarehouseDetailModel.Warehouse.Number);

            SelectedWarehouseFiche = warehouseFiche;

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                WarehouseDetailModel.LastTransactions.Clear();

                foreach (var item in result.Data)
                    WarehouseDetailModel.LastTransactions.Add(Mapping.Mapper.Map<WarehouseTransaction>(item));
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

    private async Task GetLastTransactionFicheAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseDetailService.GetLastFiches(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseDetailModel.Warehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                WarehouseDetailModel.LastFiches.Clear();
                foreach (var item in result.Data)
                    WarehouseDetailModel.LastFiches.Add(Mapping.Mapper.Map<WarehouseFiche>(item));
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

    private async Task LoadActionModelsAsync()
    {
        try
        {
            IsBusy = true;
            WarehouseActionModels.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            WarehouseActionModels.Add(new WarehouseDetailActionModel
            {
                ActionName = "Ambar Toplamları",
                ActionUrl = $"{nameof(WarehouseDetailWarehouseTotalListView)}",
                LineNumber = 1,
                Icon = "",
                IsSelected = false
            });

            WarehouseActionModels.Add(new WarehouseDetailActionModel
            {
                ActionName = "Raf Listesi",
                ActionUrl = $"{nameof(WarehouseDetailLocationListView)}",
                LineNumber = 2,
                Icon = "",
                IsSelected = false
            });

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

    private async Task ActionModelProcessTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await LoadActionModelsAsync();

            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.HalfExpanded;

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ActionModelsTappedAsync(WarehouseDetailActionModel model)
    {
        try
        {
            IsBusy = true;
            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.Hidden;
            await Task.Delay(100);

            await Shell.Current.GoToAsync($"{model.ActionUrl}", new Dictionary<string, object>
            {
                [nameof(WarehouseDetailModel)] = WarehouseDetailModel
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

    private async Task WarehouseReferenceCountAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _warehouseDetailService.WarehouseReferenceCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseDetailModel.Warehouse.Number
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<WarehouseDetailModel>(result.Data);
                WarehouseDetailModel.WarehouseReferenceCount = obj.WarehouseReferenceCount;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }

    private async Task GoToWarehouseTotalListAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(WarehouseDetailWarehouseTotalListView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseDetailModel)] = WarehouseDetailModel,
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

    private async Task WarehouseLastTransactionDateAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseDetailService.WarehouseLastTransactionDate(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseDetailModel.Warehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<WarehouseDetailModel>(result.Data);
                    WarehouseDetailModel.LastTransactionDate = obj.LastTransactionDate;
                    WarehouseDetailModel.LastTransactionTime = obj.LastTransactionTime;
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
}