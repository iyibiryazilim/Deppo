using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;
using DevExpress.Maui.Controls;
using Org.Apache.Http.Client;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels
{
	[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
	[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
	[QueryProperty(nameof(SelectedProducts), nameof(SelectedProducts))]
	[QueryProperty(nameof(SelectedOrderWarehouseModel), nameof(SelectedOrderWarehouseModel))]

	public partial class ProcurementByLocationCustomerListViewModel : BaseViewModel
	{
		private readonly IHttpClientService _httpClientService;
		private readonly IUserDialogs _userDialogs;
		private readonly IProcurementByLocationCustomerService _procurementByLocationCustomerService;
		private readonly IShipAddressService _shipAddressService;
		public ProcurementByLocationCustomerListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProcurementByLocationCustomerService procurementByLocationCustomerService, IShipAddressService shipAddressService)
		{
			_httpClientService = httpClientService;
			_userDialogs = userDialogs;
			_procurementByLocationCustomerService = procurementByLocationCustomerService;
			_shipAddressService = shipAddressService;

			Title = "Müşteri Seçimi";

			LoadItemsCommand = new Command(async () => await LoadItemsAsync());
			ItemTappedCommand = new Command<ProcurementCustomerBasketModel>(async (x) => await ItemTappedAsync(x));
			NextViewCommand = new Command(async () => await NextViewAsync());
			ShipAddressTappedCommand = new Command<ShipAddressModel>(async (x) => await ShipAddressTappedAsync(x));
			ConfirmShipAddressCommand = new Command(async () => await ConfirmShipAddressAsync());
			CloseShowProductCommand = new Command(async () => await CloseShowProductAsync());
			SwipeItemCommand = new Command<ProcurementCustomerBasketModel>(async (x) => await SwipeItemAsync(x));
		}

		[ObservableProperty]
		private ProcurementCustomerBasketModel selectedItem;

		[ObservableProperty]
		private WarehouseModel warehouseModel = null!;

		[ObservableProperty]
		private LocationModel locationModel = null!;

		[ObservableProperty]
		private WarehouseModel selectedOrderWarehouseModel = null!;

		[ObservableProperty]
		private ObservableCollection<ProcurementByLocationProduct> selectedProducts;

		public ObservableCollection<ProcurementCustomerModel> Customers { get; } = new();

		public ObservableCollection<ProcurementCustomerBasketModel> Items { get; } = new();
		public ObservableCollection<ProcurementCustomerBasketModel> SelectedItems { get; } = new();

		public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

		[ObservableProperty]
		ShipAddressModel selectedShipAddressModel;

		public Command LoadItemsCommand { get; }
		public Command ItemTappedCommand { get; }
		public Command NextViewCommand { get; }
		public Command<ShipAddressModel> ShipAddressTappedCommand { get; }
		public Command ShipAddressCloseCommand { get; }
		public Command ConfirmShipAddressCommand { get; }
		public Command CloseShowProductCommand { get; }
		public Command SwipeItemCommand { get; }
		public Page CurrentPage { get; set; } = null!;



		private async Task LoadItemsAsync()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				_userDialogs.ShowLoading("Loading...");
				Customers.Clear();
				Items.Clear();
				await Task.Delay(1000);
				var httpClient = _httpClientService.GetOrCreateHttpClient();

				string ids = string.Empty;
				foreach (var item in SelectedProducts)
				{
					ids += item.ItemReferenceId + ",";
				}
				ids = ids.Remove(ids.Length - 1);

				var result = await _procurementByLocationCustomerService.GetCustomers(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, ids, string.Empty, 0, 9999999);
				foreach (var customer in result.Data)
				{
					Customers.Add(Mapping.Mapper.Map<ProcurementCustomerModel>(customer));
				}
				var groupedCustomers = Customers.GroupBy(x => x.CustomerReferenceId);
				foreach (var group in groupedCustomers)
				{
					var customerBasketModel = new ProcurementCustomerBasketModel();
					customerBasketModel.ProcurementCustomerModel = group.First();
					foreach (var item in group)
					{
						var product = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ProductReferenceId);
						if (product is not null)
							customerBasketModel.ProcurementByLocationProducts.Add(product);

					}
					Items.Add(customerBasketModel);	

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

		private async Task ItemTappedAsync(ProcurementCustomerBasketModel item)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				SelectedItem = item;

				if (!SelectedItem.IsSelected)
				{
					if (SelectedItem.ProcurementCustomerModel?.ShipAddressCount > 0)
					{
						await LoadShipAddressesAsync(item);
						CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
					}
					else
					{
						SelectedItem.IsSelected = true;
						SelectedItems.Add(SelectedItem);
					}
				}
				else
				{
					SelectedItem.IsSelected = false;
					SelectedItem.ProcurementCustomerModel.ShipAddress = null;
					SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.ProcurementCustomerModel.CustomerReferenceId == SelectedItem.ProcurementCustomerModel.CustomerReferenceId));
					SelectedItem = null;
				}


			}
			catch (System.Exception ex)
			{
				_userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
			}
			finally
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				IsBusy = false;
			}
		}

		private async Task SwipeItemAsync(ProcurementCustomerBasketModel item)
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;

				if (item is not null)
				{
					SelectedItem = item;
					CurrentPage.FindByName<BottomSheet>("showProductsbottomsheet").State = BottomSheetState.HalfExpanded;
				}
			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task CloseShowProductAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;


				CurrentPage.FindByName<BottomSheet>("showProductsbottomsheet").State = BottomSheetState.Hidden;

			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			}
			finally
			{
				IsBusy = false;
			}
		}


		private async Task LoadShipAddressesAsync(ProcurementCustomerBasketModel customer)
		{
			try
			{
				ShipAddresses.Clear();
				_userDialogs.Loading("Loading Ship Addresses...");
				await Task.Delay(1000);

				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var result = await _shipAddressService.GetObjectsByOrder(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					currentReferenceId: customer.ProcurementCustomerModel.CustomerReferenceId,
					search: "",
					skip: 0,
					take: 99999
				);

				if (result.IsSuccess)
				{
					if (result.Data is null)
						return;

					foreach (var item in result.Data)
						ShipAddresses.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
				}

				_userDialogs.HideHud();
			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await _userDialogs.AlertAsync(ex.Message, "Hata");
			}
		}

		private async Task ShipAddressTappedAsync(ShipAddressModel shipAddressModel)
		{
			if (IsBusy)
				return;
			try
			{
				IsBusy = true;

				SelectedShipAddressModel = shipAddressModel;
				if (shipAddressModel.IsSelected)
				{
					SelectedShipAddressModel.IsSelected = false;
					SelectedShipAddressModel = null;
				}
				else
				{
					ShipAddresses.ToList().ForEach(x => x.IsSelected = false);
					SelectedShipAddressModel = shipAddressModel;
					SelectedShipAddressModel.IsSelected = true;
				}
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

		private async Task ConfirmShipAddressAsync()
		{
			if (IsBusy)
				return;
			try
			{
				IsBusy = true;

				var selectedShipAddress = ShipAddresses.FirstOrDefault(x => x.IsSelected);
				if (selectedShipAddress is not null)
				{
					SelectedItem.ProcurementCustomerModel.ShipAddress = selectedShipAddress;
					SelectedItem.IsSelected = true;

					SelectedItems.Add(SelectedItem);

					CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.Hidden;
				}
				else
				{
					_userDialogs.Alert("Lütfen bir adres seçiniz.", "Hata", "Tamam");
				}
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


		private async Task NextViewAsync()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationBasketView)}", new Dictionary<string, object>
				{
					[nameof(WarehouseModel)] = WarehouseModel,
					[nameof(LocationModel)] = LocationModel,
					[nameof(SelectedItems)] = SelectedItems,
					[nameof(SelectedOrderWarehouseModel)] = SelectedOrderWarehouseModel
				});

			}
			catch (Exception ex)
			{
				_userDialogs.Alert(ex.Message);
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

				SelectedItems.Clear();
				
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
}
