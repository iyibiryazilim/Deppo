using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.QueryHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(Product), queryId: nameof(Product))]
public partial class ProductInputTransactionViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;

    public ProductInputTransactionViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
    {
        Title = "Malzeme Giriş Hareketleri";
        _httpClientService = httpClientService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        GoToBackCommand = new Command(async () => await GoToBackAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
    }

    #region Collections

    public ObservableCollection<ProductTransaction> Items { get; } = new();

    #endregion Collections

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command GoToBackCommand { get; }
    public Command LoadMoreItemsCommand { get; }

    #endregion Commands

    #region Properties

    [ObservableProperty]
    private Product product = null!;

    #endregion Properties

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            Items.Clear();

            var query = ProductQuery.InputTransactionListQuery(
                FirmNumber: _httpClientService.FirmNumber,
                PeriodNumber: _httpClientService.PeriodNumber,
                ProductReferenceId: Product.ReferenceId,
                ExternalDb: _httpClientService.ExternalDatabase
                );

            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _customQueryService.GetObjectsAsync(httpClient, query);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    Items.Add(Mapping.Mapper.Map<ProductTransaction>(item));
                }
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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Load More Items...");

            var query = ProductQuery.InputTransactionListQuery(
                FirmNumber: _httpClientService.FirmNumber,
                PeriodNumber: _httpClientService.PeriodNumber,
                ProductReferenceId: Product.ReferenceId,
                Sorting: "DESC",
                Skip: Items.Count,
                Take: 20,
                ExternalDb: _httpClientService.ExternalDatabase
            );

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _customQueryService.GetObjectsAsync(httpClient, query);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    Items.Add(Mapping.Mapper.Map<ProductTransaction>(item));
                }
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: result.Message, title: "Hata");
            }
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

    private async Task GoToBackAsync()
    {
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync("..");
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