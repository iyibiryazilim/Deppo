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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

[QueryProperty(name: nameof(CustomerDetailModel), queryId: nameof(CustomerDetailModel))]
public partial class CustomerDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerService _customerService;
    private readonly IUserDialogs _userDialogs;
    private readonly ICustomQueryService _customQueryService;

    [ObservableProperty]
    private CustomerDetailModel customerDetailModel = null!;

    public CustomerDetailViewModel(IHttpClientService httpClientService,
    ICustomerService customerService,
    IUserDialogs userDialogs,
    ICustomQueryService customQueryService)
    {
        _httpClientService = httpClientService;
        _customerService = customerService;
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

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);

            // Querylerde yer alan firma numarasi dinamik olarak alinacak
            await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync(httpClient));
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            Console.WriteLine(CustomerDetailModel);
            IsBusy = false;
        }
    }

    private async Task GetInputOutputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var query = @$"SELECT
                    [InputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_001_02_STLINE WHERE IOCODE IN(1, 2) AND CLIENTREF = {CustomerDetailModel.Customer.ReferenceId}),
                    [OutputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_001_02_STLINE WHERE IOCODE IN(3, 4) AND CLIENTREF = {CustomerDetailModel.Customer.ReferenceId})";

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

    private async Task GetLastTransactionsAsync(HttpClient httpclient)
    {
        try
        {
            var query = @$"SELECT Top 5

        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
        [WarehouseName] = CAPIWHOUSE.NAME,
		[CustomerReferenceId] = CLCARD.LOGICALREF,
		[CustomerCode] = CLCARD.CODE,
		[CustomerName] = CLCARD.DEFINITION_
        FROM LG_001_02_STLINE AS STLINE
        LEFT JOIN LG_001_02_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_001_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_001_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_001_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_001_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 1
		WHERE STLINE.IOCODE IN (1,2,3,4) AND STFICHE.TRCODE IN (1,2,3,7,6,8)  AND STLINE.STFICHEREF <> 0 AND STLINE.USREF <> 0 AND STLINE.UOMREF <> 0 AND  CLCARD.LOGICALREF = {CustomerDetailModel.Customer.ReferenceId}  ORDER BY STLINE.DATE_ DESC";

            var result = await _customQueryService.GetObjectsAsync(httpclient, query);

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