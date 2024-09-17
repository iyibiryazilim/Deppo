using Android.Net.Wifi.Aware;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class OutputProductSalesProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISalesCustomerService _salesCustomerService;
	private readonly IShipAddressService _shipAddressService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	ObservableCollection<OutputSalesBasketModel> items = null!;

	public ObservableCollection<SalesCustomer> Customers { get; } = new();
	public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

	[ObservableProperty]
	SalesCustomer? selectedCustomer;
	[ObservableProperty]
	ShipAddressModel? selectedShipAddress;


	[ObservableProperty]
	DateTime ficheDate = DateTime.Now;

	[ObservableProperty]
	string documentNumber = string.Empty;

	[ObservableProperty]
	string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	string specialCode = string.Empty;

	[ObservableProperty]
	string description = string.Empty;

	public OutputProductSalesProcessFormViewModel(IHttpClientService httpClientService, ISalesCustomerService salesCustomerService, IShipAddressService shipAddressService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_salesCustomerService = salesCustomerService;
		_shipAddressService = shipAddressService;
		_userDialogs = userDialogs;

		Title = "Sevk İşlemi";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());

		LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
		LoadShipAddressesCommand = new Command<SalesCustomer>(async (x) => await LoadShipAddressesAsync(x));
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadPageCommand { get; }
	public Command BackCommand { get; }
	public Command SaveCommand { get; }
	public Command ShowBasketItemCommand { get; }

	public Command LoadCustomersCommand { get; }
	public Command<SalesCustomer> LoadShipAddressesCommand { get; }

	private async Task ShowBasketItemAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadPageAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadCustomersAsync()
	{
		if (IsBusy)
			return;
		try
		{
			Customers.Clear();
			SelectedCustomer = null;

			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _salesCustomerService.GetObjectsAsync(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					warehouseNumber: WarehouseModel.Number,
					skip: 0,
					take: 9999999,
					search: ""
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
					Customers.Add(Mapping.Mapper.Map<SalesCustomer>(item));
                }
			}
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadShipAddressesAsync(SalesCustomer salesCustomer)
	{
		if(salesCustomer is null)
		{
			await _userDialogs.AlertAsync("Lütfen müşteri seçiniz.", "Uyarı", "Tamam");
			return;
		}
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			ShipAddresses.Clear();
			SelectedShipAddress = null;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _shipAddressService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				currentReferenceId: salesCustomer.ReferenceId,
				skip: 0,
				take: 9999999,
				search: ""
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Customers.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
				}
			}
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");

		}
		finally
		{
			IsBusy = false;
		}
	}
}
