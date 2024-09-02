using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Remoting;

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
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command NextViewCommand { get; }
	public Command ItemTappedCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<CustomerModel> Items { get; } = new();

	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;
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

			if(result.IsSuccess)
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


}
