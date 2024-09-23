using System;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

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
        PerformSearchCommand = new Command<SearchBar>(async (searchBar) => await PerformSearchAsync(searchBar));
        ItemTappedCommand = new Command<Outsource>(async (outsource) => await ItemTappedAsync(outsource));
    }

    public Page CurrentPage { get; set; }
    public ObservableCollection<Outsource> Items { get; } = new();
    public Command<Outsource> ItemTappedCommand { get; }
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
            var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
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
            var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
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

                        var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
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

    private async Task ItemTappedAsync(Outsource outsource)
    {
        if (outsource == null)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            
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
