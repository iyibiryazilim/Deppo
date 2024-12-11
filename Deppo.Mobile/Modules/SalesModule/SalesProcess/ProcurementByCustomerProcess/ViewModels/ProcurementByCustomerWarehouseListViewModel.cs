using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;
using DevExpress.Utils.Serializing;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

public partial class ProcurementByCustomerWarehouseListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly IWarehouseCountingService _warehouseCountingService;
    private readonly IUserDialogs _userDialogs;
    private readonly ILocationService _locationService;

    [ObservableProperty]
    private WarehouseModel? selectedWarehouseModel;


    [ObservableProperty]
    LocationModel? selectedLocationModel;

	[ObservableProperty]
	public SearchBar searchText;

	public ProcurementByCustomerWarehouseListViewModel(
		IHttpClientService httpClientService,
		IWarehouseService warehouseService,
		IUserDialogs userDialogs,
		IWarehouseCountingService warehouseCountingService,
		ILocationService locationService)
	{
		_httpClientService = httpClientService;
		_warehouseService = warehouseService;
		_userDialogs = userDialogs;
		_warehouseCountingService = warehouseCountingService;
		_locationService = locationService;

		Title = "Sipariş Ambarı Seçiniz";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		LoadMoreLocationsCommand = new Command(async () => await LoadMoreLocationsAsync());
		LocationTappedCommand = new Command<LocationModel>(async (locationModel) => await LocationTappedAsync(locationModel));
		ConfirmLocationsCommand = new Command(async () => await ConfirmLocationsAsync());

		LocationsPerformSearchCommand = new Command(async () => await LocationsPerformSearchAsync());
		LocationsPerformEmptySearchCommand = new Command(async () => await LocationsPerformEmptySearchAsync());
	}

	public ObservableCollection<WarehouseModel> Items { get; } = new();
    public ObservableCollection<LocationModel> Locations { get; } = new();

	public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<WarehouseModel> ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }
    public Command LoadMoreLocationsCommand { get; }
	public Command<LocationModel> LocationTappedCommand { get; }
    public Command ConfirmLocationsCommand { get; }
    public Command LocationsPerformSearchCommand { get; }
    public Command LocationsPerformEmptySearchCommand { get; }

	private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseCountingService.GetWarehouses(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: string.Empty,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var warehouse = Mapping.Mapper.Map<WarehouseModel>(item);
                        Items.Add(warehouse);
                    }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

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
        if (Items.Count < 18)
            return;
        try
        {
            IsBusy = true;

			_userDialogs.ShowLoading("Yükleniyor...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseCountingService.GetWarehouses(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: string.Empty,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {	
					foreach (var item in result.Data)
					{
						var warehouse = Mapping.Mapper.Map<WarehouseModel>(item);
						Items.Add(warehouse);
					}
				}
                    
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void ItemTappedAsync(WarehouseModel warehouse)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;


            if(!warehouse.IsSelected)
            {
                if(warehouse.LocationCount > 0)
                {
                    SelectedWarehouseModel = warehouse;
                    await LoadLocationsAsync(warehouse);
                    CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    warehouse.IsSelected = true;
                    SelectedWarehouseModel = warehouse;
				}
            }
            else
            {
                warehouse.IsSelected = false;
                SelectedWarehouseModel = null;
            }


            //if (warehouse == SelectedWarehouseModel)
            //{
            //    SelectedWarehouseModel.IsSelected = false;
            //    SelectedWarehouseModel = null;
            //}
            //else
            //{
            //    if (SelectedWarehouseModel != null)
            //    {
            //        SelectedWarehouseModel.IsSelected = false;
            //    }

            //    SelectedWarehouseModel = warehouse;
            //    SelectedWarehouseModel.IsSelected = true;
            //}

        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadLocationsAsync(WarehouseModel warehouse)
    {
        if (warehouse is null)
            return;
        try
        {
            Locations.Clear();
            _userDialogs.Loading("Loading Locations...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: warehouse.Number,
                search: SearchText.Text,
                skip: 0,
                take: 20
            );

            if(result.IsSuccess)
            {
                if (result.Data is null)
                    return;
            }

            foreach (var location in result.Data)
            {
				var obj = Mapping.Mapper.Map<LocationModel>(location);
				obj.IsSelected = SelectedLocationModel != null && SelectedLocationModel?.ReferenceId == obj.ReferenceId ? SelectedLocationModel.IsSelected : false;
				Locations.Add(obj);
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
    }

    private async Task LoadMoreLocationsAsync()
    {
        if (IsBusy)
            return;
        if (Locations.Count < 18)
            return;
        try
        {
            IsBusy = true;

			_userDialogs.Loading("Loading More Locations...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: SelectedWarehouseModel.Number,
				search: SearchText.Text,
				skip: Locations.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
			}

			foreach (var location in result.Data)
			{
				var obj = Mapping.Mapper.Map<LocationModel>(location);
				obj.IsSelected = SelectedLocationModel != null && SelectedLocationModel?.ReferenceId == obj.ReferenceId ? SelectedLocationModel.IsSelected : false;
				Locations.Add(obj);
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

	private async Task LocationTappedAsync(LocationModel locationModel)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

            SelectedLocationModel = locationModel;
            if (locationModel.IsSelected)
            {
				SelectedLocationModel.IsSelected = false;
				SelectedLocationModel = null;
            }
            else
            {
                Locations.ToList().ForEach(x => x.IsSelected = false);
				SelectedLocationModel = locationModel;
				SelectedLocationModel.IsSelected = true;
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

	private async Task ConfirmLocationsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var selectedLocation = Locations.FirstOrDefault(x => x.IsSelected);
			if (selectedLocation is not null)
			{
				//SelectedWarehouseModel.LocationReferenceId = selectedLocation.ReferenceId;
				SelectedWarehouseModel.LocationCode = selectedLocation.Code;

				Items.ToList().ForEach(x => x.IsSelected = false);

				SelectedWarehouseModel.IsSelected = true;

				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
			}
			else
			{
				_userDialogs.Alert("Lütfen bir raf seçiniz.", "Hata", "Tamam");
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

	private async Task LocationsPerformSearchAsync()
	{
		try
		{
			if (string.IsNullOrEmpty(SearchText.Text))
			{
				await LoadLocationsAsync(SelectedWarehouseModel);
				SearchText.Unfocus();
				return;
			}

			IsBusy = true;

			Locations.Clear();
			_userDialogs.Loading("Searching...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
			   httpClient: httpClient,
			   firmNumber: _httpClientService.FirmNumber,
			   periodNumber: _httpClientService.PeriodNumber,
			   warehouseNumber: SelectedWarehouseModel.Number,
			   search: SearchText.Text,
			   skip: 0,
			   take: 20
		   );

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<LocationModel>(item);
						obj.IsSelected = SelectedLocationModel != null && SelectedLocationModel?.ReferenceId == obj.ReferenceId ? SelectedLocationModel.IsSelected : false;
						Locations.Add(obj);
					}
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LocationsPerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(SearchText.Text))
		{
			await LocationsPerformSearchAsync();
		}
	}

	private async Task NextViewAsync()
    {
        if (IsBusy)
            return;
        if(SelectedWarehouseModel is null)
			return;
		try
        {
            IsBusy = true;

            if(SelectedWarehouseModel.LocationCount > 0 && SelectedWarehouseModel.LocationCode == string.Empty)
            {
                await _userDialogs.AlertAsync("Lütfen devam etmek için raf seçiniz", "Uyarı", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(ProcurementByCustomerListView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = SelectedWarehouseModel
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

            if (SelectedWarehouseModel is not null)
			{
				SelectedWarehouseModel.IsSelected = false;
				SelectedWarehouseModel = null;
			}

            if (SelectedLocationModel is not null)
            {
                SelectedLocationModel.IsSelected = false;
				SelectedLocationModel = null;
			}

            Items.ForEach(x => x.IsSelected = false);
			await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
}