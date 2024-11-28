using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.ViewModels;

public partial class WaitingSalesOrderListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWaitingSalesOrderService _waitingSalesOrderService;
    private readonly ICustomerService _customerService;
    private readonly IUserDialogs _userDialogs;
    private readonly IShipAddressService _shipAddressService;

    [ObservableProperty]
    CustomerModel? selectedCustomerModel;
    [ObservableProperty]
    ShipAddressModel? selectedShipAddressModel;


    public Page CurrentPage { get; set; }

    #region Collections

    public ObservableCollection<CustomerModel> Customers { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();
    public ObservableCollection<WaitingSalesOrderModel> Items { get; } = new();

	#endregion Collections

	public WaitingSalesOrderListViewModel(
		IHttpClientService httpClientService,
		IWaitingSalesOrderService waitingSalesOrderService,
		ICustomerService customerService,
		IUserDialogs userDialogs,
		IShipAddressService shipAddressService)
	{
		_httpClientService = httpClientService;
		_waitingSalesOrderService = waitingSalesOrderService;
		_customerService = customerService;
		_userDialogs = userDialogs;
		_shipAddressService = shipAddressService;

		Title = "Bekleyen Satış Siparişleri";

		LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
		LoadShipAddressesCommand = new Command(async () => await LoadShipAddressesAsync());
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		FilterTappedCommand = new Command(async () => await FilterTappedAsync());
	}

	#region Commands

	public Command LoadCustomersCommand { get; }
    public Command LoadShipAddressesCommand { get; }
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command FilterTappedCommand { get; }

    public Command ConfirmCommand { get; }

    #endregion Commands


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

    private async Task LoadCustomersAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

			_userDialogs.Loading("Loading Customers...");
            Customers.Clear();
            SelectedCustomerModel = null;
            await Task.Delay(500);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingSalesOrderService.GetCustomers(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: "",
                skip: 0,
                take: 99999
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Customers.Add(Mapping.Mapper.Map<CustomerModel>(item));

			}

            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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

	private async Task LoadShipAddressesAsync()
	{
        if (IsBusy)
            return;

		if (SelectedCustomerModel is null)
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
                currentReferenceId: SelectedCustomerModel.ReferenceId,
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

	private async Task LoadItemsAsync()
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
			if (SelectedShipAddressModel is null)
            {
				result = await _waitingSalesOrderService.GetObjectsByCustomer(
				  httpClient: httpClient,
				  firmNumber: _httpClientService.FirmNumber,
				  periodNumber: _httpClientService.PeriodNumber,
				  customerReferenceId: SelectedCustomerModel.ReferenceId,
				  search: "",
				  skip: 0,
				  take: 20
			   );
			}
            else
            {
			   result = await _waitingSalesOrderService.GetObjectsByCustomerAndShipInfo(
			       httpClient: httpClient,
			       firmNumber: _httpClientService.FirmNumber,
			       periodNumber: _httpClientService.PeriodNumber,
			       customerReferenceId: SelectedCustomerModel.ReferenceId,
			       shipInfoReferenceId: SelectedShipAddressModel is not null ? SelectedShipAddressModel.ReferenceId : 0,
			       search: "",
			       skip: 0,
			       take: 20
		        );
			}

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

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
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
				result = await _waitingSalesOrderService.GetObjectsByCustomer(
				  httpClient: httpClient,
				  firmNumber: _httpClientService.FirmNumber,
				  periodNumber: _httpClientService.PeriodNumber,
				  customerReferenceId: SelectedCustomerModel.ReferenceId,
				  search: "",
				  skip: Items.Count,
				  take: 20
			   );
			}
			else
			{
				result = await _waitingSalesOrderService.GetObjectsByCustomerAndShipInfo(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					customerReferenceId: SelectedCustomerModel.ReferenceId,
					shipInfoReferenceId: SelectedShipAddressModel is not null ? SelectedShipAddressModel.ReferenceId : 0,
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
                    Items.Add(Mapping.Mapper.Map<WaitingSalesOrderModel>(item));
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

    private async Task ConfirmAsync()
    {
       if(SelectedCustomerModel is null)
       {
			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;
	   } 
       else
       {
			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			Title = string.Format("{0} Bekleyen Siparişleri", SelectedCustomerModel.Name);
			await LoadItemsAsync();
	    }
    }
}