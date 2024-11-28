using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels.ActionViewModels;

[QueryProperty(name: nameof(CustomerDetailModel), queryId: nameof(CustomerDetailModel))]
public partial class CustomerDetailWaitingSalesOrderListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ICustomerDetailActionService _customerDetailActionService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	CustomerDetailModel customerDetailModel = null!;

	public ObservableCollection<WaitingSalesOrderModel> Items { get; } = new();

	public CustomerDetailWaitingSalesOrderListViewModel(IHttpClientService httpClientService, ICustomerDetailActionService customerDetailActionService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_customerDetailActionService = customerDetailActionService;
		_userDialogs = userDialogs;

		Title = "Bekleyen Satış Siparişleri";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		CloseCommand = new Command(async () => await CloseAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command CloseCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items");
			await Task.Delay(1000);
			Items.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _customerDetailActionService.GetWaitingSalesOrdersByCustomer(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
				search: string.Empty,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<WaitingSalesOrderModel>(item));
				}
			}

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadMoreItemsAsync()
	{
		if (Items.Count < 18)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading More Items");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _customerDetailActionService.GetWaitingSalesOrdersByCustomer(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
				search: string.Empty,
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<WaitingSalesOrderModel>(item));
				}
			}

			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task CloseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync("..");
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

}
