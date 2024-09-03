using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(CustomerModel), queryId: nameof(CustomerModel))]
public partial class OutputProductSalesOrderProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	public OutputProductSalesOrderProcessProductListViewModel(IHttpClientService httpClientService, IWaitingSalesOrderService waitingSalesOrderService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_waitingSalesOrderService = waitingSalesOrderService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		Title = "Sipariş seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<WaitingSalesOrderModel> Items { get; } = new();

	public ObservableCollection<WaitingSalesOrderModel> SelectedItems { get; } = new();
	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	CustomerModel customerModel = null!;
	#endregion

	private async Task LoadItemsAsync()
	{
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Load Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _waitingSalesOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{

				}
			}

			_userDialogs.Loading().Hide();

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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _waitingSalesOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, skip: Items.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{

				}
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

	private async Task ItemTappedAsync(WaitingSalesOrderModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item is not null)
			{
				if (!item.IsSelected)
				{
					Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;

					var basketItem = new OutputSalesBasketModel
					{
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetName = item.UnitsetName,
						UnitsetCode = item.UnitsetCode,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetName = item.SubUnitsetName,
						SubUnitsetCode = item.SubUnitsetCode,
						
					};

					//SelectedItems.Add(basketItem);
				} else
				{
					var selectedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
					if (selectedItem is not null)
					{
						SelectedItems.Remove(selectedItem);
						Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = false;
					}
				}
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
		try
		{
			IsBusy = true;

			if (SelectedItems.Count > 0)
			{
				var nextViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();

				foreach (var item in SelectedItems)
				{


				}
				await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessBasketListView)}");
			}
			else
			{
				_userDialogs.Alert("Lütfen en az bir ürün seçiniz.", "Uyarı", "Tamam");
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
			{
				_userDialogs.HideHud();
			}

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
