using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

public partial class ProductListViewModel : BaseViewModel
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
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        ItemTappedCommand = new Command<Product>(async (product) => await ItemTappedAsync(product));
    }

    public ObservableCollection<Product> Items { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command<Product> ItemTappedCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Items.Clear();
            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<Product>(item));

                _userDialogs.HideHud();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

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
            _userDialogs.Loading("Loading More Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<Product>(item));

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

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

    private async Task PerformSearchAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                await LoadItemsAsync();
                SearchText.Unfocus();
                return;
            }
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (!result.IsSuccess)
            {
                _userDialogs.Alert(result.Message, "Hata");
                return;
            }

            Items.Clear();
            foreach (var item in result.Data)
            {
                var itm = Mapping.Mapper.Map<Product>(item);
                Items.Add(itm);
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

    private async Task PerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }

    private async Task ItemTappedAsync(Product product)
    {
        if (product == null)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            ProductDetailModel productDetailModel = new();
            productDetailModel.Product = product;

            await Shell.Current.GoToAsync($"{nameof(ProductDetailView)}", new Dictionary<string, object>
            {
                [nameof(ProductDetailModel)] = productDetailModel
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