using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

[QueryProperty(name: nameof(ProductOrderModel), queryId: nameof(ProductOrderModel))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ProcurementByProductCustomerListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementByProductCustomerService _procurementByProductCustomerService;
    private readonly IShipAddressService _shipAddressService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    ProductOrderModel productOrderModel = null!;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

	public ObservableCollection<CustomerOrderModel> Items { get; } = new();

    [ObservableProperty]
    CustomerOrderModel selectedCustomer;
	public ObservableCollection<CustomerOrderModel> SelectedItems { get; } = new();

	public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

	[ObservableProperty]
	ShipAddressModel selectedShipAddressModel;


	public ProcurementByProductCustomerListViewModel(
        IHttpClientService httpClientService,
        IProcurementByProductCustomerService procurementByProductCustomerService,
        IShipAddressService shipAddressService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementByProductCustomerService = procurementByProductCustomerService;
        _shipAddressService = shipAddressService;
        _userDialogs = userDialogs;

        Title = "Müşteri Seçiniz";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<CustomerOrderModel>(async (x) => await ItemTappedAsync(x));
		ShipAddressTappedCommand = new Command<ShipAddressModel>(async (x) => await ShipAddressTappedAsync(x));
		ConfirmShipAddressCommand = new Command(async () => await ConfirmShipAddressAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}
    public Page CurrentPage { get; set; } = null!;
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<CustomerOrderModel> ItemTappedCommand { get; }
    public Command<ShipAddressModel> ShipAddressTappedCommand { get; }
    public Command ShipAddressCloseCommand { get; }
    public Command ConfirmShipAddressCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Items.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementByProductCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                productReferenceId: ProductOrderModel.ItemReferenceId,
                search: string.Empty,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customerOrder = Mapping.Mapper.Map<CustomerOrderModel>(item);
                        Items.Add(customerOrder);
                    }
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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;
        if (Items.Count < 18)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementByProductCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                productReferenceId: ProductOrderModel.ItemReferenceId,
				search: string.Empty,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customerOrder = Mapping.Mapper.Map<CustomerOrderModel>(item);
                        Items.Add(customerOrder);
                    }
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
    
    private async Task ItemTappedAsync(CustomerOrderModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            SelectedCustomer = item;

            if(!SelectedCustomer.IsSelected)
            {
                if(SelectedCustomer.ShipAddressCount > 0)
                {
                    await LoadShipAddressesAsync(item);
                    CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
				}
                else
                {
					SelectedCustomer.IsSelected = true;
                    SelectedItems.Add(SelectedCustomer);
                }
            }
            else
            {
				SelectedCustomer.IsSelected = false;
				SelectedCustomer.ShipAddress = null;
				SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.Code == SelectedCustomer.Code));
				SelectedCustomer = null;
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

    private async Task LoadShipAddressesAsync(CustomerOrderModel customer)
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
				currentReferenceId: customer.ReferenceId,
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
            if(_userDialogs.IsHudShowing)
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
				SelectedCustomer.ShipAddress = selectedShipAddress;
				SelectedCustomer.IsSelected = true;

                SelectedItems.Add(SelectedCustomer);

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

            if (SelectedItems.Count == 0)
            {
                _userDialogs.Alert("Lütfen en az bir müşteri seçiniz.", "Uyarı", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(ProcurementByProductProcurementWarehouseListView)}", new Dictionary<string, object>
            {
                ["SelectedCustomers"] = SelectedItems,
				[nameof(ProductOrderModel)] = ProductOrderModel,
				["OrderWarehouse"] = WarehouseModel
			});

            
        }
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync($"{ex.Message}", "Hata", "Tamam");
        }
        finally
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

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

            foreach (var item in SelectedItems)
            {
                item.DistributedQuantity = 0;
                item.ShipAddress = null;
			}
            SelectedItems.Clear();

            await Shell.Current.GoToAsync("..");
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

    public async Task ClearPageAsync()
    {
        await Task.Run(() =>
        {
            SelectedItems.Clear();
        });
	}

}
