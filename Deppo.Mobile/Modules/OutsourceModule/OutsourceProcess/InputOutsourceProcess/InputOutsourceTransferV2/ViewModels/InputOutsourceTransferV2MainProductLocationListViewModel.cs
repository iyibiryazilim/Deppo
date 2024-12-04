using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;


[QueryProperty(name: nameof(InputOutsourceTransferV2BasketModel), queryId: nameof(InputOutsourceTransferV2BasketModel))]
public partial class InputOutsourceTransferV2MainProductLocationListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationService _locationService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	InputOutsourceTransferV2BasketModel? inputOutsourceTransferV2BasketModel;

	public ObservableCollection<LocationModel> Items { get; } = new();
	public ObservableCollection<LocationModel> SelectedSearchItems { get; } = new();

	public ObservableCollection<LocationModel> SelectedItems { get; } = new();

	[ObservableProperty]
	LocationModel? selectedItem;

	[ObservableProperty]
	public SearchBar searchText;

	public InputOutsourceTransferV2MainProductLocationListViewModel(IHttpClientService httpClientService, ILocationService locationService, IUserDialogs userDialogs, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_locationService = locationService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		CancelCommand = new Command(async () => await CancelAsync());

		IncreaseCommand = new Command<LocationModel>(async (x) => await IncreaseAsync(x));
		DecreaseCommand = new Command<LocationModel>(async (x) => await DecreaseAsync(x));
		QuantityTappedCommand = new Command<LocationModel>(async (x) => await QuantityTappedAsync(x));
		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ShowLocationsCommand = new Command(async () => await ShowLocationsAsync());
		LocationsPerformSearchCommand = new Command(async () => await LocationsPerformSearchAsync());
		LocationsPerformEmptySearchCommand = new Command(async () => await LocationsPerformEmptySearchAsync());
		CloseLocationsCommand = new Command(async () => await CloseLocationsAsync());
		ItemTappedCommand = new Command<LocationModel>(async (locationModel) => await ItemTappedAsync(locationModel));
		ConfirmLocationsCommand = new Command(async () => await ConfirmLocationsAsync());
		CancelCommand = new Command(async () => await CancelAsync());
	}

	public Page CurrentPage { get; set; } = null!;
	public Command<LocationModel> QuantityTappedCommand { get; }
	public Command<LocationModel> IncreaseCommand { get; }
	public Command<LocationModel> DecreaseCommand { get; }
	public Command<Entry> PerformSearchCommand { get; }
	public Command ConfirmCommand { get; }
	public Command CancelCommand { get; }

	#region Location BottomSheet Commands
	public Command ShowLocationsCommand { get; }
	public Command CloseLocationsCommand { get; }
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command<LocationModel> ItemTappedCommand { get; }
	public Command LocationsPerformSearchCommand { get; }
	public Command LocationsPerformEmptySearchCommand { get; }
	public Command ConfirmLocationsCommand { get; }
	#endregion

	private async Task IncreaseAsync(LocationModel locationModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedItem = locationModel;

			
			locationModel.InputQuantity += 1;
			
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

	private async Task DecreaseAsync(LocationModel locationModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (locationModel.InputQuantity > 0)
			{
				SelectedItem = locationModel;

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

	private async Task QuantityTappedAsync(LocationModel locationModel)
	{
		if (locationModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: locationModel.Code,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: locationModel.InputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			locationModel.InputQuantity = quantity;
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
				warehouseNumber: InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
				productReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId,
				variantReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : 0,
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


	private async Task CancelAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");

			SelectedItems.Clear();
			await Task.Delay(500);

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		if (InputOutsourceTransferV2BasketModel is null)
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
				warehouseNumber: InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
				productReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId,
				variantReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : 0,
				search: SearchText.Text,
				skip: 0,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<LocationModel>(item);
						var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.Code == obj.Code);
						obj.IsSelected = matchedItem != null ? matchedItem.IsSelected : false;
						Items.Add(obj);
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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
		if (InputOutsourceTransferV2BasketModel is null)
			return;
		if (Items.Count < 18)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
				productReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId,
				variantReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : 0,
				search: SearchText.Text,
				skip: Items.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<LocationModel>(item);
						var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.Code == obj.Code);
						obj.IsSelected = matchedItem != null ? matchedItem.IsSelected : false;
						Items.Add(obj);
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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}
	private async Task ItemTappedAsync(LocationModel locationModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (locationModel.IsSelected)
			{

				SelectedSearchItems.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = false;
				SelectedSearchItems.Remove(Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId));
				Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = false;
			}
			else
			{
				Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = true;
				SelectedSearchItems.Add(Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId));
				SelectedSearchItems.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = true;

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
	private async Task ConfirmLocationsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			await Task.Delay(500);
			foreach (var item in Items.Where(x => x.IsSelected))
			{
				if (!SelectedItems.Any(x => x.Code == item.Code))
					SelectedItems.Add(item);
			}

			SelectedSearchItems.Clear();

			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;

			if(_userDialogs.IsHudShowing)
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

	private async Task LocationsPerformSearchAsync()
	{
		try
		{
			if (string.IsNullOrEmpty(SearchText.Text))
			{
				await LoadItemsAsync();
				SearchText.Unfocus();
				return;
			}

			IsBusy = true;

			Items.Clear();
			_userDialogs.Loading("Searching...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
				productReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId,
				variantReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant == true ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId : 0,
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
						var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.Code == obj.Code);
						obj.IsSelected = matchedItem != null ? matchedItem.IsSelected : false;
						Items.Add(obj);
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

			if (_userDialogs.IsHudShowing)
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
}
