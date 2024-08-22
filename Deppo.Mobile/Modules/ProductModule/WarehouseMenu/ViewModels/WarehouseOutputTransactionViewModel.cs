using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.CompanyHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

[QueryProperty(name: nameof(Warehouse), queryId: nameof(Warehouse))]
public partial class WarehouseOutputTransactionViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWarehouseTransactionService _warehouseTransactionService;
	private readonly IUserDialogs _userDialogs;
	public WarehouseOutputTransactionViewModel(IHttpClientService httpClientService, IWarehouseTransactionService warehouseTransactionService, IUserDialogs userDialogs)
	{
		Title = "Ambar Çıkış Hareketleri";
		_httpClientService = httpClientService;
		_warehouseTransactionService = warehouseTransactionService;
		_userDialogs = userDialogs;

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		GoToBackCommand = new Command(async () => await GoToBackAsync());
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command<SearchBar> PerformSearchCommand { get; }
	public Command GoToBackCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<WarehouseTransaction> Items { get; } = new();
	#endregion

	#region Properties
	[ObservableProperty]
	string searchText = string.Empty;

	[ObservableProperty]
	Warehouse warehouse = null!;
	#endregion

	async Task LoadItemsAsync()
	{
		try
		{
			IsBusy = true;
			Items.Clear();

			_userDialogs.Loading("Loading Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await Task.Delay(1000);
			var result = await _warehouseTransactionService.GetOutputTransactionByWarehouseNumberAsync(httpClient, Warehouse.Number, SearchText, null, 0, 20, await CompanyHelper.GetCompanyNumberAsync());

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
					Items.Add(item);

				_userDialogs.Loading().Hide();
			}
			else
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(message: result.Message, title: "Hata");
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

	async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			_userDialogs.Loading("Load Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseTransactionService.GetOutputTransactionByWarehouseNumberAsync(httpClient, Warehouse.Number, SearchText, null, Items.Count, 20, await CompanyHelper.GetCompanyNumberAsync());

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Items.Add(item);

				if(_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();
			}
			else
			{
				if(_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(message: result.Message, title: "Hata");
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

	async Task GoToBackAsync()
	{
		if (IsBusy)
			return;
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
