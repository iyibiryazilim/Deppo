using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;

public partial class OutsourceListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IOutsourceService _outsourceService;
    private readonly IUserDialogs _userDialogs;

    public OutsourceListViewModel(IHttpClientService httpClientService, IOutsourceService outsourceService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _outsourceService = outsourceService;
        _userDialogs = userDialogs;

        Title = "Fason Cariler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());

        ItemTappedCommand = new Command<Outsource>(async (outsource) => await ItemTappedAsync(outsource));

        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
    }

    public Page CurrentPage { get; set; }
    public ObservableCollection<Outsource> Items { get; } = new();
    public Command<Outsource> ItemTappedCommand { get; }
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

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
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<Outsource>(item));

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
            var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<Outsource>(item));

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

            Items.Clear();
            _userDialogs.Loading("Searching Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<Outsource>(item));

                _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: result.Message, title: "Load Items");
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

    private async Task ItemTappedAsync(Outsource outsource)
    {
        if (outsource is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            OutsourceDetailModel outsourceDetailModel = new();
            outsourceDetailModel.Outsource = outsource;

            await Shell.Current.GoToAsync($"{nameof(OutsourceDetailView)}", new Dictionary<string, object>
            {
                [nameof(OutsourceDetailModel)] = outsourceDetailModel
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