using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;

public partial class SalesPanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISalesPanelService _salesPanelService;

    [ObservableProperty]
    private SalesPanelModel salesPanelModel = new();

    [ObservableProperty]
    private SalesFiche selectedSalesFiche;

    public SalesPanelViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, ISalesPanelService salesPanelService)
    {
        _salesPanelService = salesPanelService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Satış Paneli";
        LoadItemsCommand = new Command(async () => await LoadItemAsync());
        ItemTappedCommand = new Command<SalesFiche>(async (salesFiche) => await ItemTappedAsync(salesFiche));
        WaitingOrderTappedCommand = new Command(async () => await WaitingOrderTappedAsync());
        ShippedOrderTappedCommand = new Command(async () => await ShippedOrderTappedAsync());
        CustomerTappedCommand = new Command<Customer>(async (customer) => await CustomerTappedAsync(customer));
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
    }

    public Page CurrentPage { get; set; }
    public Command LoadItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command WaitingOrderTappedCommand { get; }
    public Command ShippedOrderTappedCommand { get; }
    public Command CustomerTappedCommand { get; }
    public Command AllFicheTappedCommand { get; }

    private async Task LoadItemAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            CancellationTokenSource cancellationTokenSource = new();

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);

            await Task.WhenAll(TotalOrderCountsAsync(), WaitingProductCountAsync(), ShippedOrderCountsAsync()).ContinueWith((task) =>
            {
                SalesPanelModel.WaitingOrderCount = SalesPanelModel.WaitingOrderCount;
                SalesPanelModel.WaitingOrderCountRate = (double)((double)SalesPanelModel.WaitingOrderCount / (double)SalesPanelModel.AmountTotal);
                SalesPanelModel.ShippedQuantityTotalRate = (double)((double)SalesPanelModel.ShippedQuantityTotal / (double)SalesPanelModel.AmountTotal);
            });

            await Task.WhenAll(GetLastTransactionByCustomerAsync(), GetLastFicheAsync());

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
            Console.WriteLine(SalesPanelModel);
            IsBusy = false;
        }
    }

    private async Task ItemTappedAsync(SalesFiche salesFiche)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(500);
            await GetLastTransactionAsync(salesFiche);
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

    private async Task GetLastTransactionByCustomerAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.GetLastTransactionByCustomer(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                SalesPanelModel.LastCustomer.Clear();
                foreach (var item in result.Data)
                    SalesPanelModel.LastCustomer.Add(Mapping.Mapper.Map<Customer>(item));
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

    private async Task GetLastTransactionAsync(SalesFiche salesFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.GetLastTransaction(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, salesFiche.ReferenceId);

            SelectedSalesFiche = salesFiche;

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                SalesPanelModel.LastCustomerTransaction.Clear();

                foreach (var item in result.Data)
                    SalesPanelModel.LastCustomerTransaction.Add(Mapping.Mapper.Map<CustomerTransaction>(item));
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

    private async Task TotalOrderCountsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.TotalOrderCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<SalesPanelModel>(item));
                    SalesPanelModel.AmountTotal = value.AmountTotal;
                }

                var jsonData = result.Data.ToString();
                var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);
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

    private async Task ShippedOrderCountsAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.ShippedOrderCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<SalesPanelModel>(item));
                    SalesPanelModel.ShippedQuantityTotal = value.ShippedQuantityTotal;
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

    private async Task WaitingProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.WaitingProductCount(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var value = (Mapping.Mapper.Map<SalesPanelModel>(item));
                    SalesPanelModel.WaitingOrderCount = value.WaitingOrderCount;
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

    private async Task GetLastFicheAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _salesPanelService.GetLastFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                SalesPanelModel.LastSalesFiche.Clear();
                foreach (var item in result.Data)
                    SalesPanelModel.LastSalesFiche.Add(Mapping.Mapper.Map<SalesFiche>(item));
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

    private async Task WaitingOrderTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(SalesPanelWaitingProductListView)}");
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

    private async Task ShippedOrderTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(SalesPanelShippedProductListView)}");
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

    private async Task AllFicheTappedAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(SalesPanelAllFicheListView)}");
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

    private async Task CustomerTappedAsync(Customer customer)
    {
        if (customer == null)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            CustomerDetailModel customerDetailModel = new();
            customerDetailModel.Customer = customer;

            await Shell.Current.GoToAsync($"{nameof(CustomerDetailView)}", new Dictionary<string, object> { {
                nameof(CustomerDetailModel), customerDetailModel
                }
                });
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }
}