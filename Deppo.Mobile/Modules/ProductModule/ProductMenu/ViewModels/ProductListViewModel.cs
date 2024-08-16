using System;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

public class ProductListViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;

    public ProductListViewModel(IProductService productService, IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _productService = productService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Malzemeler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        PerformSearchCommand = new Command<SearchBar>(async (searchBar) => await PerformSearchAsync(searchBar));
    }

    public ObservableCollection<Product> Items { get; } = new();

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
            var result = await _productService.GetObjects(httpClient, string.Empty, string.Empty, null, 0, 20, 1);
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
            var result = await _productService.GetObjects(httpClient, string.Empty, string.Empty, null, Items.Count, 20, 1);
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

                        var result = await _productService.GetObjects(httpClient, searchBar.Text, string.Empty, null, 1, 999999, 1);
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
}
