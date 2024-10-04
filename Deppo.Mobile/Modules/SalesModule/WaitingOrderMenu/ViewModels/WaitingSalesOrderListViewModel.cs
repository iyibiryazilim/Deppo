using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.ViewModels;

public partial class WaitingSalesOrderListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWaitingSalesOrderService _waitingSalesOrderService;
    private readonly ICustomerService _customerService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private CustomerModel selectedCustomerModel = null!;

    [ObservableProperty]
    private CustomerModel customerModel = null!;

    public Page CurrentPage { get; set; }

    #region Collections

    public ObservableCollection<CustomerModel> Customers { get; } = new();
    public ObservableCollection<WaitingSalesOrder> Items { get; } = new();

    #endregion Collections

    public WaitingSalesOrderListViewModel(
        IHttpClientService httpClientService,
        IWaitingSalesOrderService waitingSalesOrderService,
        ICustomerService customerService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _waitingSalesOrderService = waitingSalesOrderService;
        _customerService = customerService;
        _userDialogs = userDialogs;

        Title = "Bekleyen Satış Siparişleri";

        LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
        LoadItemsCommand = new Command<CustomerModel>(async (customer) => await LoadItemsAsync(customer));
        ItemTappedCommand = new Command<CustomerModel>(ItemTappedAsync);
        CustomerConfirmCommand = new Command(async () => await CustomerConfirmAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
    }

    #region Commands

    public Command LoadCustomersCommand { get; }
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command CustomerConfirmCommand { get; }

    #endregion Commands

    private async Task LoadCustomersAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            Customers.Clear();

            SelectedCustomerModel = null;
            _userDialogs.Loading("Loading Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _waitingSalesOrderService.GetCustomers(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, "", 0, 999999);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Customers.Add(Mapping.Mapper.Map<CustomerModel>(item));

                CurrentPage.FindByName<BottomSheet>("customerBottomSheet").State = BottomSheetState.HalfExpanded;

                _userDialogs.Loading().Hide();
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

    private async Task LoadItemsAsync(Customer customer)
    {
        if (customer is null)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);
            Items.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingSalesOrderService.GetObjectsByCustomer(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, customerReferenceId: customer.ReferenceId, "", 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    Items.Add(Mapping.Mapper.Map<WaitingSalesOrder>(item));
                }
            }

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

    private async Task LoadMoreItemsAsync()
    {
        if (SelectedCustomerModel is null)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingSalesOrderService.GetObjectsByCustomer(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, customerReferenceId: SelectedCustomerModel.ReferenceId, "", Items.Count, 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    Items.Add(Mapping.Mapper.Map<WaitingSalesOrder>(item));
                }
            }

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

    private async void ItemTappedAsync(CustomerModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Customers.ToList().ForEach(x => x.IsSelected = false);

            var selectedItem = Customers.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
            if (selectedItem != null)
                selectedItem.IsSelected = true;

            SelectedCustomerModel = item;
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

    private async Task CustomerConfirmAsync()
    {
        CurrentPage.FindByName<BottomSheet>("customerBottomSheet").State = BottomSheetState.Hidden;

        Title = string.Format("{0} Bekleyen Siparişleri", SelectedCustomerModel.Name);
        await LoadItemsAsync(SelectedCustomerModel);
    }
}