using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesOrderProcessCustomerListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ICustomerService _customerService;
	private readonly IUserDialogs _userDialogs;
	public OutputProductSalesOrderProcessCustomerListViewModel(IHttpClientService httpClientService, ICustomerService customerService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_customerService = customerService;
		_userDialogs = userDialogs;

		Title = "Müşteri seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<CustomerModel>(ItemTappedAsync);
		NextViewCommand = new Command(async () => await NextViewAsync());
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<CustomerModel> Items { get; } = new();

	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	CustomerModel? selectedCustomerModel;
	#endregion

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _customerService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<CustomerModel>(item);
					Items.Add(obj);
				}
			}

			_userDialogs.Loading().Hide();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
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

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _customerService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, skip: Items.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<CustomerModel>(item);
					Items.Add(obj);
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ItemTappedAsync(CustomerModel item)
	{
		try
		{
			IsBusy = true;

			if (SelectedCustomerModel == item)
			{
				SelectedCustomerModel.IsSelected = false;
				SelectedCustomerModel = null;
			}
			else
			{
				if (SelectedCustomerModel is not null)
				{
					SelectedCustomerModel.IsSelected = false;
				}
				SelectedCustomerModel = item;
				SelectedCustomerModel.IsSelected = true;
			}

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedCustomerModel is not null)
			{
				await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessProductListView)}", new Dictionary<string, object>
				{
					[nameof(WarehouseModel)] = WarehouseModel,
					[nameof(CustomerModel)] = SelectedCustomerModel
				});
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
}
