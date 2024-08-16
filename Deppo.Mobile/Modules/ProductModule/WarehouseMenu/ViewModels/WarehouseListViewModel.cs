using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

public partial class WarehouseListViewModel : BaseViewModel
{
    private IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
	private readonly IUserDialogs _userDialogs;

    public WarehouseListViewModel(IWarehouseService warehouseService, IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Ambar Listesi";
		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		RefreshPageCommand = new Command(async () => await RefreshPageAsync());
		PerformSearchCommand = new Command<SearchBar>(async (searchBar) => await PerformSearchAsync(searchBar));
    }

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command<SearchBar> PerformSearchCommand { get; }
	public Command RefreshPageCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<Warehouse> Items { get; } = new();
	#endregion

	#region Properties
	[ObservableProperty]
	string searchText = string.Empty;
	[ObservableProperty]
	int page = 0;
	#endregion

	public async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			Items.Clear();
			//string? token = await SecureStorage.GetAsync("token");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseService.GetObjects(httpClient,search: SearchText, orderBy: null, page: 0, pageSize: 20, firmNumber: 1);
			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var item in result.Data)
					Items.Add(item);
			}
			else
			{
				_userDialogs.Alert(message: result.Message, title: "Load Items");
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(message: ex.Message, title: "Load Items");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task LoadMoreItemsAsync()
	{
		if (Items.Count < (20 - 2))
			return;

		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			Page += 1;

			var result = await _warehouseService.GetObjects(httpClient, SearchText, null, page: Page, 20, 1);
			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;
				if(!result.Data.Any())
				{
					Page -= 1;
					return;
				}

				foreach (var item in result.Data)
					Items.Add(item);
			}
			else
			{
				_userDialogs.Alert(message: result.Message, title: "Load Items");
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(message: ex.Message, title: "Load Items Error");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task RefreshPageAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			IsRefreshing = true;
			IsRefreshing = false;

			Items.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseService.GetObjects(httpClient, search: SearchText, null, 0, 20, 1);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var item in result.Data)
					Items.Add(item);
			}
			else
			{
				_userDialogs.Alert(message: result.Message, title: "Hata");
			}
            
        }
		catch (Exception ex)
		{
			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
		finally
		{
			IsBusy = false;
			IsRefreshing = false;
		}
	}

	async Task PerformSearchAsync(SearchBar searchBar)
	{
		if (IsBusy)
			return;
		try
		{
			if(string.IsNullOrWhiteSpace(searchBar.Text))
			{
				SearchText = string.Empty;
				await LoadItemsAsync();
				searchBar.Unfocus();
				return;
			}
			else
			{
				if(searchBar.Text.Length >= 3)
				{
					IsBusy = true;
					
					var httpClient = _httpClientService.GetOrCreateHttpClient();
					SearchText = searchBar.Text;
					var result = await _warehouseService.GetObjects(httpClient, SearchText, null, 0, 20, 1);
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
		catch(Exception ex)
		{
			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
