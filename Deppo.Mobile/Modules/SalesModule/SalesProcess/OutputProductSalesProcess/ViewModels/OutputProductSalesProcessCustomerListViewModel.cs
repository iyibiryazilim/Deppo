using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesProcessCustomerListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISalesCustomerService _salesCustomerService;
	private readonly ISalesCustomerProductService _salesCustomerProductService;
	private readonly IShipAddressService _shipAddressService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	SalesCustomer? swipedSalesCustomer;  // Müşteriye ait ürün listesini göstermek için swipe edilen müşteriyi tutar.

	public ObservableCollection<SalesCustomer> Items { get; } = new();

	public ObservableCollection<SalesCustomerProduct> SalesCustomerProducts { get; } = new();

	public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

	public OutputProductSalesProcessCustomerListViewModel(IHttpClientService httpClientService, ISalesCustomerService salesCustomerService, ISalesCustomerProductService salesCustomerProductService, IShipAddressService shipAddressService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_salesCustomerService = salesCustomerService;
		_salesCustomerProductService = salesCustomerProductService;
		_shipAddressService = shipAddressService;
		_userDialogs = userDialogs;

		Title = "Müşteriler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<SalesCustomer>(async (customer) => await ItemTappedAsync(customer));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());

		ShowProductsCommand = new Command<SalesCustomer>(async (customer) => await ShowProductsAsync(customer));
		LoadMoreProductsCommand = new Command(async () => await LoadMoreProductsAsync());

		LoadMoreShipAddressesCommand = new Command(async () => await LoadMoreShipAddressesAsync());
		ShipAddressTappedCommand = new Command<ShipAddressModel>(async (shipAddress) => await ShipAddressTappedAsync(shipAddress));
		
	}

	public Page CurrentPage { get; set; } = null!;


	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	public Command ShowProductsCommand { get; }
	public Command LoadMoreProductsCommand { get; }

	public Command LoadMoreShipAddressesCommand { get; }
	public Command ShipAddressTappedCommand { get; }
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

				// Müşterinin sevk adresi varsa sevk adres bottomSheet aç
				if (SalesCustomer.ShipAddressCount > 0)
				{
					await LoadShipAddressesAsync(SalesCustomer);
					CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
				} 

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

	private async Task ShowProductsAsync(SalesCustomer customer)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await LoadProductsAsync(customer);
			CurrentPage.FindByName<BottomSheet>("customerProductsBottomSheet").State = BottomSheetState.HalfExpanded;


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

	private async Task LoadProductsAsync(SalesCustomer customer)
	{
		try
		{
			SwipedSalesCustomer = customer;
			SalesCustomerProducts.Clear();
			_userDialogs.Loading("Loading Products...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _salesCustomerProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: SwipedSalesCustomer.ReferenceId,
				warehouseNumber: WarehouseModel.Number,
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
					SalesCustomerProducts.Add(Mapping.Mapper.Map<SalesCustomerProduct>(item));
				}
			}

			_userDialogs.HideHud();


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

	private async Task LoadMoreProductsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _salesCustomerProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: SwipedSalesCustomer.ReferenceId,
				warehouseNumber: WarehouseModel.Number,
				search: string.Empty,
				skip: SalesCustomerProducts.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var order in result.Data)
				{
					SalesCustomerProducts.Add(Mapping.Mapper.Map<SalesCustomerProduct>(order));
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

	private async Task LoadShipAddressesAsync(SalesCustomer customer)
	{
		try
		{
			ShipAddresses.Clear();

			_userDialogs.Loading("Loading Ship Addresses...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _shipAddressService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				currentReferenceId: SalesCustomer.ReferenceId,
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
					ShipAddresses.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
				}
			}


			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadMoreShipAddressesAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _shipAddressService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				currentReferenceId: SalesCustomer.ReferenceId,
				search: string.Empty,
				skip: ShipAddresses.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					ShipAddresses.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
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

	private async Task ShipAddressTappedAsync(ShipAddressModel shipAddressModel)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			ShipAddresses.ToList().ForEach(x => x.IsSelected = false);
			ShipAddresses.FirstOrDefault(x => x.ReferenceId == shipAddressModel.ReferenceId).IsSelected = true;

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
	
	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SalesCustomer is not null)
			{
				await Shell.Current.GoToAsync($"{nameof(OutputProductSalesProcessBasketListView)}", new Dictionary<string, object>
				{
					[nameof(SalesCustomer)] = SalesCustomer,
					[nameof(WarehouseModel)] = WarehouseModel,
				});
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(SalesCustomer is not null)
			{
				SalesCustomer.IsSelected = false;
				SalesCustomer = null;
			}
			
			await Shell.Current.GoToAsync("..");
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
