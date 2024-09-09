using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesProcessCustomerListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISalesCustomerService _salesCustomerService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	public ObservableCollection<SalesCustomer> Items { get; } = new();

	public OutputProductSalesProcessCustomerListViewModel(IHttpClientService httpClientService, ISalesCustomerService salesCustomerService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_salesCustomerService = salesCustomerService;
		_userDialogs = userDialogs;

		Title = "Müşteriler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<SalesCustomer>(async (customer) => await ItemTappedAsync(customer));
		NextViewCommand = new Command(async () => await NextViewAsync());
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
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
			var result = await _salesCustomerService.GetObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: 0, take: 20, search: string.Empty);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
			}

			_userDialogs.HideHud();
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
			var result = await _salesCustomerService.GetObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: Items.Count, take: 20, search: string.Empty);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
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

	private async Task ItemTappedAsync(SalesCustomer item)
	{
		try
		{
			IsBusy = true;

			if (SalesCustomer == item)
			{
				SalesCustomer.IsSelected = false;
				SalesCustomer = null;
			}
			else
			{
				if (SalesCustomer is not null)
				{
					SalesCustomer.IsSelected = false;
				}
				SalesCustomer = item;
				SalesCustomer.IsSelected = true;
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SalesCustomer is not null)
			{
				//await Shell.Current.GoToAsync($"{nameof(OutputProductSalesProcessProductListView)}", new Dictionary<string, object>
				//{
				//	[nameof(SalesCustomer)] = SalesCustomer,
				//	[nameof(WarehouseModel)] = WarehouseModel,
				//});
			}
			else
			{
				_userDialogs.Alert("Lütfen bir müşteri seçiniz.", "Hata", "Tamam");
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
