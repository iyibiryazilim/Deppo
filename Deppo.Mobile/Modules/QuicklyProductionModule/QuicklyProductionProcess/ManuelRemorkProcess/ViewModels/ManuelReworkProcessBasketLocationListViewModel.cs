using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MessageHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;


[QueryProperty(name: nameof(SelectedReworkInProductModel), queryId: nameof(SelectedReworkInProductModel))]
public partial class ManuelReworkProcessBasketLocationListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationService _locationService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	ReworkInProductModel selectedReworkInProductModel = null!;

	public ObservableCollection<LocationModel> Items { get; } = new();
	public ObservableCollection<LocationModel> SelectedItems { get; } = new();

	[ObservableProperty]
	LocationModel? selectedItem;

	public ManuelReworkProcessBasketLocationListViewModel(IHttpClientService httpClientService, ILocationService locationService, IUserDialogs userDialogs, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_locationService = locationService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		IncreaseCommand = new Command<LocationModel>(async (x) => await IncreaseAsync(x));
		DecreaseCommand = new Command<LocationModel>(async (x) => await DecreaseAsync(x));
		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		CancelCommand = new Command(async () => await CancelAsync());

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ShowLocationsCommand = new Command(async () => await ShowLocationsAsync());
		CloseLocationsCommand = new Command(async () => await CloseLocationsAsync());
		ItemTappedCommand = new Command<LocationModel>(async (x) => await ItemTappedAsync(x));
		ConfirmLocationsCommand = new Command(async () => await ConfirmLocationsAsync());
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadSelectedItemsCommand { get; }  
	public Command<LocationModel> IncreaseCommand { get; }
	public Command<LocationModel> DecreaseCommand { get; }
	public Command<Entry> PerformSearchCommand { get; } 
	public Command ConfirmCommand { get; }
	public Command CancelCommand { get; }


	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ShowLocationsCommand { get; }
	public Command CloseLocationsCommand { get; }
	public Command<LocationModel> ItemTappedCommand { get; }
	public Command<SearchBar> LocationsPerformSearchCommand { get; } // method will be added
	public Command ConfirmLocationsCommand { get; }

	public async Task LoadSelectedItemsAsync()
	{
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			SelectedItems.Clear();
			await Task.Delay(1000);

			if(SelectedReworkInProductModel.Details.Count > 0)
			{
                foreach (var item in SelectedReworkInProductModel.Details)
                {
					SelectedItems.Add(new LocationModel
					{
						Code = item.LocationCode,
						Name = item.LocationName,
						ReferenceId = item.LocationReferenceId,
						InputQuantity = item.Quantity,
						WarehouseName = SelectedReworkInProductModel.InWarehouseModel.Name,
						WarehouseNumber = SelectedReworkInProductModel.InWarehouseModel.Number,
						WarehouseReferenceId = SelectedReworkInProductModel.InWarehouseModel.ReferenceId,
					});
                }
            }

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam"); ;
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task IncreaseAsync(LocationModel locationModel)
	{
		if (locationModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = locationModel;

			if(SelectedReworkInProductModel.TrackingType != 0)
			{
				// SeriLot logic comes here
			}
			else
			{
				locationModel.InputQuantity += 1;
			}
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

	private async Task DecreaseAsync(LocationModel locationModel)
	{
		if (locationModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (locationModel.InputQuantity <= 0)
				return;

			SelectedItem = locationModel;

			if (SelectedReworkInProductModel.TrackingType != 0)
			{
				// SeriLot logic comes here
			}
			else
			{
				locationModel.InputQuantity -= 1;
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

	private async Task PerformSearchAsync(Entry barcodeEntry)
	{
		if (IsBusy)
			return;

		try
		{
			if (string.IsNullOrEmpty(barcodeEntry.Text))
				return;

			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
						httpClient: httpClient,
						firmNumber: _httpClientService.FirmNumber,
						periodNumber: _httpClientService.PeriodNumber,
						warehouseNumber: SelectedReworkInProductModel.InWarehouseModel.Number,
						productReferenceId: SelectedReworkInProductModel.IsVariant ? SelectedReworkInProductModel.MainItemReferenceId : SelectedReworkInProductModel.ReferenceId,
						variantReferenceId: SelectedReworkInProductModel.IsVariant ? SelectedReworkInProductModel.ReferenceId : 0,
						search: barcodeEntry.Text,
						skip: 0,
						take: 1);

			if (result.IsSuccess)
			{
				if (!(result.Data.Count() > 0))
				{
					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

					_userDialogs.ShowToast(message: $"{barcodeEntry.Text} kodlu raf bulunamadı.");
					return;
				}

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<LocationModel>(item);
					if (SelectedItems.Where(x => x.Code == obj.Code).Any())
					{
						SelectedItems.Where(x => x.Code == obj.Code).FirstOrDefault().InputQuantity += 1;
					}
					else
					{
						SelectedItems.Add(obj);
					}
				}
			}

			if (_userDialogs.IsHudShowing)
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
			barcodeEntry.Text = string.Empty;
			barcodeEntry.Focus();
			IsBusy = false;
		}
	}

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var basketViewModel = _serviceProvider.GetService<ManuelReworkProcessBasketViewModel>();
			if(SelectedItems.Any(x => x.InputQuantity == 0))
			{
				foreach (var item in SelectedItems.Where(x=>x.InputQuantity <= 0))
				{
					SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId));
				}
			}

			_userDialogs.ShowLoading("Loading...");
			foreach (var item in SelectedItems.Where(x => x.InputQuantity > 0))
            {
				var selectedItemlocationDetail = SelectedReworkInProductModel.Details.FirstOrDefault(x => x.LocationCode == item.Code);

				if(selectedItemlocationDetail is not null)
				{
					selectedItemlocationDetail.Quantity = item.InputQuantity;
					//selectedItemlocationDetail.RemainingQuantity = item.InputQuantity;
				}
				else
				{
					SelectedReworkInProductModel.Details.Add(new ReworkInProductDetailModel
					{
						LocationReferenceId = item.ReferenceId,
						LocationCode = item.Code,
						LocationName = item.Name,
						RemainingQuantity = item.InputQuantity,
						Quantity = item.InputQuantity,
					});
				}
			}

			var totalInputQuantity = SelectedItems.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);
			SelectedReworkInProductModel.InputQuantity = totalInputQuantity;

            await Shell.Current.GoToAsync("..");
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

	private async Task CancelAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			await Task.Delay(500);
			await Shell.Current.GoToAsync("..");
			_userDialogs.HideHud();
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


	private async Task ShowLocationsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			await LoadItemsAsync();
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task CloseLocationsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			await Task.Delay(500);
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
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
			

			_userDialogs.ShowLoading("Loading...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: SelectedReworkInProductModel.InWarehouseModel.Number,
				productReferenceId: SelectedReworkInProductModel.IsVariant ? SelectedReworkInProductModel.MainItemReferenceId : SelectedReworkInProductModel.ReferenceId,
				variantReferenceId: SelectedReworkInProductModel.IsVariant ? SelectedReworkInProductModel.ReferenceId : 0,
				skip: 0,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
						Items.Add(Mapping.Mapper.Map<LocationModel>(item));
				}
			}

			_userDialogs.HideHud();
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

	private async Task LoadMoreItemsAsync()
	{
		if (Items.Count < 18)
			return;
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading More Items");

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: SelectedReworkInProductModel.InWarehouseModel.Number,
				productReferenceId: SelectedReworkInProductModel.IsVariant ? SelectedReworkInProductModel.MainItemReferenceId : SelectedReworkInProductModel.ReferenceId,
				variantReferenceId: SelectedReworkInProductModel.IsVariant ? SelectedReworkInProductModel.ReferenceId : 0,
				skip: Items.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
						Items.Add(Mapping.Mapper.Map<LocationModel>(item));
				}
			}

			_userDialogs.HideHud();
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

	private async Task ItemTappedAsync(LocationModel locationModel)
	{
		if (locationModel is null)
			return;
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (locationModel.IsSelected)
			{
				Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = false;
			}
			else
			{
				Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = true;
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

            if (Items.Where(x=> x.IsSelected).Count() == 0)
            {
				await _userDialogs.AlertAsync("Lütfen en az bir raf seçiniz", "Uyarı", "Tamam");
				return;
            }

            _userDialogs.ShowLoading("Loading...");
			await Task.Delay(500);
			foreach (var item in Items.Where(x => x.IsSelected))
			{
				if (!SelectedItems.Any(x => x.Code == item.Code))
					SelectedItems.Add(item);
			}

			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;

			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{

			throw;
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task ClearPageAsync()
	{
		try
		{
			await Task.Run(() =>
			{
				if(SelectedItem is not null)
				{
					SelectedItem.IsSelected = false;
					SelectedItem = null;
				}
				SelectedItems.Clear();
			});
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
