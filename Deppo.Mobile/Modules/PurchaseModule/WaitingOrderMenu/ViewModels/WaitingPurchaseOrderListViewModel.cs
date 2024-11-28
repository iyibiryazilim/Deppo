using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.ViewModels;

public partial class WaitingPurchaseOrderListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISupplierService _supplierService;
    private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;
    private readonly IShipAddressService _shipAddressService;

	public ObservableCollection<WaitingPurchaseOrderModel> Items { get; } = new();
    public ObservableCollection<SupplierModel> Suppliers { get; } = new();
	public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    [ObservableProperty]
    private SupplierModel selectedSupplierModel;

	[ObservableProperty]
	ShipAddressModel? selectedShipAddressModel;

	public WaitingPurchaseOrderListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWaitingPurchaseOrderService waitingPurchaseOrderService, ISupplierService supplierService, IShipAddressService shipAddressService)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_waitingPurchaseOrderService = waitingPurchaseOrderService;
		_supplierService = supplierService;
		_shipAddressService = shipAddressService;

		Title = "Bekleyen Satın Alma Siparişleri";

		ConfirmCommand = new Command(async () => await ConfirmAsync());
		FilterTappedCommand = new Command(async () => await FilterTappedAsync());
		LoadSuppliersCommand = new Command(async () => await LoadSupplierAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		LoadShipAddressesCommand = new Command(async () => await LoadShipAddressesAsync());
	}
	public Page CurrentPage { get; set; }

	public Command LoadItemsCommand { get; }
    public Command LoadSuppliersCommand { get; }
    public Command LoadShipAddressesCommand { get; }
	public Command LoadMoreItemsCommand { get; }
  
    public Command FilterTappedCommand { get; }
	public Command ConfirmCommand { get; }

	private async Task FilterTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.HalfExpanded;
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

	public async Task LoadSupplierAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Suppliers...");
            Suppliers.Clear();
            SelectedSupplierModel = null;
            await Task.Delay(500);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingPurchaseOrderService.GetSuppliers(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                search: string.Empty, 
                skip: 0, 
                take: 999999);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Suppliers.Add(Mapping.Mapper.Map<SupplierModel>(item));
            }

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

	private async Task LoadShipAddressesAsync()
	{
		if (IsBusy)
			return;

		if (SelectedSupplierModel is null)
		{
			await _userDialogs.AlertAsync("Lütfen müşteri seçiniz", "Uyarı", "Tamam");
			return;
		}

		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Ship Addresses...");
			ShipAddresses.Clear();
			SelectedShipAddressModel = null;
			await Task.Delay(500);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _shipAddressService.GetObjectsByOrder(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				currentReferenceId: SelectedSupplierModel.ReferenceId,
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

			if (_userDialogs.IsHudShowing)
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

	public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

			_userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			DataResult<IEnumerable<dynamic>> result;
			if(SelectedShipAddressModel is null)
			{
				result = await _waitingPurchaseOrderService.GetObjectsBySupplier(
				   httpClient: httpClient,
				   firmNumber: _httpClientService.FirmNumber,
				   periodNumber: _httpClientService.PeriodNumber,
				   supplierReferenceId: SelectedSupplierModel.ReferenceId,
				   search: "",
				   skip: 0,
				   take: 20
				);
			}
			else
			{
				result = await _waitingPurchaseOrderService.GetObjectsBySupplierAndShipInfo(
				   httpClient: httpClient,
				   firmNumber: _httpClientService.FirmNumber,
				   periodNumber: _httpClientService.PeriodNumber,
				   supplierReferenceId: SelectedSupplierModel.ReferenceId,
				   shipInfoReferenceId: SelectedShipAddressModel.ReferenceId,
				   search: "",
				   skip: 0,
				   take: 20
				);
			}
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item));
                }
            }

			if (_userDialogs.IsHudShowing)
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

            _userDialogs.Loading("Loading More Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();

			DataResult<IEnumerable<dynamic>> result;
			if (SelectedShipAddressModel is null)
			{
				result = await _waitingPurchaseOrderService.GetObjectsBySupplier(
				   httpClient: httpClient,
				   firmNumber: _httpClientService.FirmNumber,
				   periodNumber: _httpClientService.PeriodNumber,
				   supplierReferenceId: SelectedSupplierModel.ReferenceId,
				   search: "",
				   skip: Items.Count,
				   take: 20
				);
			}
			else
			{
				result = await _waitingPurchaseOrderService.GetObjectsBySupplierAndShipInfo(
				   httpClient: httpClient,
				   firmNumber: _httpClientService.FirmNumber,
				   periodNumber: _httpClientService.PeriodNumber,
				   supplierReferenceId: SelectedSupplierModel.ReferenceId,
				   shipInfoReferenceId: SelectedShipAddressModel.ReferenceId,
				   search: "",
				   skip: Items.Count,
				   take: 20
				);
			}

			if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    Items.Add(Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item));
                }
            }

			if (_userDialogs.IsHudShowing)
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


    private async Task ConfirmAsync()
    {
		if (SelectedSupplierModel is null)
		{
			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;
		}
		else
		{
			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			Title = string.Format("{0} Bekleyen Siparişleri", SelectedSupplierModel.Name);
			await LoadItemsAsync();
		}
    }
}