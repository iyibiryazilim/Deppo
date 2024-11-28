using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using DevExpress.Data.Async.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Maui.Controls;
using Deppo.Mobile.Core.ActionModels.SupplierActionModels;
using System.Collections.ObjectModel;
using Deppo.Mobile.Core.ActionModels.CustomerActionModels;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views.ActionViews;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views.ActionViews;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

[QueryProperty(name: nameof(CustomerDetailModel), queryId: nameof(CustomerDetailModel))]
public partial class CustomerDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerService _customerService;
    private readonly ICustomerTransactionService _customerTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly ICustomQueryService _customQueryService;
    private readonly ICustomerDetailService _customerDetailService;

    [ObservableProperty]
    private CustomerDetailModel customerDetailModel = null!;

    [ObservableProperty]
    private SalesFiche selectedSalesFiche;

    public ObservableCollection<CustomerDetailActionModel> CustomerActionModels { get; } = new();

    public CustomerDetailViewModel(IHttpClientService httpClientService,
    ICustomerService customerService,
    ICustomerTransactionService customerTransactionService,
    IUserDialogs userDialogs,
    ICustomQueryService customQueryService, ICustomerDetailService customerDetailService)
    {
        _httpClientService = httpClientService;
        _customerService = customerService;
        _customerTransactionService = customerTransactionService;
        _userDialogs = userDialogs;
        _customQueryService = customQueryService;
        _customerDetailService = customerDetailService;

        Title = "Müşteri Detayı";
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());
        ItemTappedCommand = new Command<SalesFiche>(async (salesFiche) => await ItemTappedAsync(salesFiche));
        AllFicheTappedCommand = new Command(async () => await AllFicheTappedAsync());
        GetLastTransactionCommand = new Command<SalesFiche>(async (salesFiche) => await GetLastTransactionAsync(salesFiche));

        ActionModelProcessTappedCommand = new Command(async () => await ActionModelProcessTappedAsync());
        ActionModelsTappedCommand = new Command<CustomerDetailActionModel>(async (model) => await ActionModelsTappedAsync(model));

        GoToWaitingOrderCommand = new Command(async () => await GoToWaitingOrderAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }

    public Command OutputQuantityTappedCommand { get; }

    public Command AllFicheTappedCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command GetLastTransactionCommand { get; }

    public Command ActionModelProcessTappedCommand { get; }
    public Command ActionModelsTappedCommand { get; }

    public Command GoToWaitingOrderCommand { get; }

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            await Task.WhenAll(GetInputQuantityAsync(httpClient), GetOutputQuantityAsync(httpClient), CustomerInputOutputQuantitiesAsync(httpClient), GetLastFicheAsync(),GetWaitingProductReferenceCountAsync(httpClient));

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
            var result = await _customerDetailService.GetInputQuantity(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                customerReferenceId: CustomerDetailModel.Customer.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<CustomerDetailModel>(result.Data);
                CustomerDetailModel.InputQuantity = obj.InputQuantity;
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
            var result = await _customerDetailService.GetOutputQuantity(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                customerReferenceId: CustomerDetailModel.Customer.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<CustomerDetailModel>(result.Data);
                CustomerDetailModel.OutputQuantity = obj.OutputQuantity;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }

    private async Task CustomerInputOutputQuantitiesAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _customerDetailService.CustomerInputOutputQuantities(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                dateTime: DateTime.Now,
                customerReferenceId: CustomerDetailModel.Customer.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                List<CustomerDetailInputOutputModel> cacheItems = new();

                foreach (var item in result.Data)
                {
                    var value = Mapping.Mapper.Map<CustomerDetailInputOutputModel>(item);
                    cacheItems.Add(value);
                }

                CustomerDetailModel.CustomerDetailInputOutputModels.Clear();
                foreach (var item in cacheItems.OrderBy(x => x.ArgumentDay))
                    CustomerDetailModel.CustomerDetailInputOutputModels.Add(item);
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
            var result = await _customerDetailService.GetLastFichesByCustomer(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, CustomerDetailModel.Customer.ReferenceId);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                CustomerDetailModel.LastFiches.Clear();
                foreach (var item in result.Data)
                    CustomerDetailModel.LastFiches.Add(Mapping.Mapper.Map<SalesFiche>(item));
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
            await Shell.Current.GoToAsync($"{nameof(CustomerInputTransactionView)}", new Dictionary<string, object>
            {
                [nameof(CustomerDetailModel)] = CustomerDetailModel
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
            await Shell.Current.GoToAsync($"{nameof(CustomerOutputTransactionView)}", new Dictionary<string, object>
            {
                [nameof(CustomerDetailModel)] = CustomerDetailModel
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

            await Shell.Current.GoToAsync($"{nameof(CustomerDetailAllFichesView)}", new Dictionary<string, object> { {
                nameof(CustomerDetailModel), CustomerDetailModel
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

    private async Task GetLastTransactionAsync(SalesFiche salesFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _customerDetailService.GetLastTransaction(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, salesFiche.ReferenceId);
            SelectedSalesFiche = salesFiche;

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                CustomerDetailModel.LastTransactions.Clear();

                foreach (var item in result.Data)
                    CustomerDetailModel.LastTransactions.Add(Mapping.Mapper.Map<CustomerTransaction>(item));
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

    private async Task ActionModelProcessTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.Loading("Yükleniyor");
            await LoadActionModelAsync();

            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.HalfExpanded;

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadActionModelAsync()
    {
        try
        {
            CustomerActionModels.Clear();

            CustomerActionModels.Add(new CustomerDetailActionModel
            {
                LineNumber = 1,
                ActionName = "Satılabilen Ürünler",
                ActionUrl = $"{nameof(CustomerDetailApprovedProductListView)}",
                Icon = "",
                IsSelected = false
            });

            CustomerActionModels.Add(new CustomerDetailActionModel
            {
                LineNumber = 2,
                ActionName = "Bekleyen Satış Siparişleri",
                ActionUrl = $"{nameof(CustomerDetailWaitingSalesOrderListView)}",
                Icon = "",
                IsSelected = false
            });

            CustomerActionModels.Add(new CustomerDetailActionModel
            {
                LineNumber = 3,
                ActionName = "Sevk Adresleri",
                ActionUrl = $"{nameof(CustomerDetailShipAddressListView)}",
                Icon = "",
                IsSelected = false
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task ActionModelsTappedAsync(CustomerDetailActionModel model)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("processBottomSheet").State = BottomSheetState.Hidden;
            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{model.ActionUrl}", new Dictionary<string, object>
            {
                [nameof(CustomerDetailModel)] = CustomerDetailModel
            });
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToWaitingOrderAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(CustomerDetailWaitingSalesOrderListView)}", new Dictionary<string, object>
            {
                [nameof(CustomerDetailModel)] = CustomerDetailModel,
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

    private async Task GetWaitingProductReferenceCountAsync(HttpClient httpClient)
    {
        try
        {
            var result = await _customerDetailService.GetWaitingProductReferenceCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                customerReferenceId: CustomerDetailModel.Customer.ReferenceId
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                var obj = Mapping.Mapper.Map<CustomerDetailModel>(result.Data);
                CustomerDetailModel.WaitingProductReferenceCount = obj.WaitingProductReferenceCount;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
        }
    }
}