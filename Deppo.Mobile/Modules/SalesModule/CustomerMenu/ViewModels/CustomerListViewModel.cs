using System;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

public partial class CustomerListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerService _customerService;
    private readonly IUserDialogs _userDialogs;

    public CustomerListViewModel(
        IHttpClientService httpClientService,
        ICustomerService customerService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _customerService = customerService;
        _userDialogs = userDialogs;

        Title = "Müþteriler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        PerformSearchCommand = new Command<SearchBar>(async (searchBar) => await PerformSearchAsync(searchBar));
        ItemTappedCommand = new Command<Customer>(async (customer) => await ItemTappedAsync(customer));
    }

    public ObservableCollection<Customer> Items { get; } = new();
    public Command<Customer> ItemTappedCommand { get; }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<SearchBar> PerformSearchCommand { get; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();

            _userDialogs.Loading("Loading Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _customerService.GetObjects(httpClient, string.Empty, string.Empty, null, 0, 20, _httpClientService.FirmNumber);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(item);

                _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: result.Message, title: "Load Items");
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.Loading("Refreshing Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _customerService.GetObjects(httpClient, string.Empty, string.Empty, null, Items.Count, 20, _httpClientService.FirmNumber);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(item);

                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: result.Message, title: "Load Items");
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PerformSearchAsync(SearchBar searchBar)
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                await LoadItemsAsync();
                searchBar.Unfocus();
                return;
            }
            else
            {
                if (searchBar.Text.Length >= 3)
                {
                    IsBusy = true;
                    using (_userDialogs.Loading("Searching.."))
                    {
                        var httpClient = _httpClientService.GetOrCreateHttpClient();

                        var result = await _customerService.GetObjects(httpClient, searchBar.Text, string.Empty, null, 0, 999999, _httpClientService.FirmNumber);
                        if (!result.IsSuccess)
                        {
                            _userDialogs.Alert(result.Message, "Hata");
                            return;
                        }

                        Items.Clear();
                        foreach (var item in result.Data)
                            Items.Add(item);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ItemTappedAsync(Customer customer)
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