using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using DevExpress.Data.Async.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

[QueryProperty(name: nameof(CustomerDetailModel), queryId: nameof(CustomerDetailModel))]
public partial class CustomerDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerService _customerService;
    private readonly ICustomerTransactionService _customerTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly ICustomQueryService _customQueryService;

    [ObservableProperty]
    private CustomerDetailModel customerDetailModel = null!;

    public CustomerDetailViewModel(IHttpClientService httpClientService,
    ICustomerService customerService,
	ICustomerTransactionService customerTransactionService,
	IUserDialogs userDialogs,
    ICustomQueryService customQueryService)
    {
        _httpClientService = httpClientService;
        _customerService = customerService;
        _customerTransactionService = customerTransactionService;
        _userDialogs = userDialogs;
        _customQueryService = customQueryService;

        Title = "Müşteri Detayı";
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }

    public Command OutputQuantityTappedCommand { get; }

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

			_userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();

            await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync(httpClient));

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

    private async Task GetInputOutputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var query = @$"SELECT
                    [InputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_{_httpClientService.FirmNumber.ToString().PadLeft(3, '0')}_{_httpClientService.PeriodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(1, 2) AND CLIENTREF = {CustomerDetailModel.Customer.ReferenceId}),
                    [OutputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_{_httpClientService.FirmNumber.ToString().PadLeft(3, '0')}_{_httpClientService.PeriodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(3, 4) AND CLIENTREF = {CustomerDetailModel.Customer.ReferenceId})";

            var result = await _customQueryService.GetObjectAsync(httpClient, query);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;
                var obj = Mapping.Mapper.Map<CustomerDetailModel>(result.Data);
                CustomerDetailModel.InputQuantity = obj.InputQuantity;
                CustomerDetailModel.OutputQuantity = obj.OutputQuantity;
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetLastTransactionsAsync(HttpClient httpClient)
    {
        try
        {

            var result = await _customerTransactionService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
                search: "",
                skip: 0,
                take: 5
            );

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    CustomerDetailModel.LastTransactions.Add(Mapping.Mapper.Map<CustomerTransaction>(item));
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

    private async Task InputQuantityTappedAsync()
    {
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(CustomerInputTransactionView)}", new Dictionary<string, object>
            {
                ["Customer"] = CustomerDetailModel.Customer
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
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(CustomerOutputTransactionView)}", new Dictionary<string, object>
            {
                ["Customer"] = CustomerDetailModel.Customer
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
}