using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class OutputProductSalesOrderProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IShipAddressService _shipAddressService;
	private readonly ICarrierService _carrierService;
	private readonly IDriverService _driverService;
	private readonly IRetailSalesDispatchTransactionService _retailSalesDispatchTransactionService;
	private readonly IWholeSalesDispatchTransactionService _wholeSalesDispatchTransactionService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	ObservableCollection<OutputSalesBasketModel> items = null!;

	public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();
	public ObservableCollection<Carrier> Carriers { get; } = new();
	public ObservableCollection<Driver> Drivers { get; } = new();

	[ObservableProperty]
	ShipAddressModel? selectedShipAddress;
	[ObservableProperty]
	Carrier? selectedCarrier;
	[ObservableProperty]
	Driver? selectedDriver;

	[ObservableProperty]
	DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	string documentNumber = string.Empty;

	[ObservableProperty]
	string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	string specialCode = string.Empty;

	[ObservableProperty]
	string description = string.Empty;

	[ObservableProperty]
	string cargoTrackingNumber = string.Empty;

	public OutputProductSalesOrderProcessFormViewModel(IHttpClientService httpClientService, IShipAddressService shipAddressService, ICarrierService carrierService, IDriverService driverService, IUserDialogs userDialogs, IRetailSalesDispatchTransactionService retailSalesDispatchTransactionService, IWholeSalesDispatchTransactionService wholeSalesDispatchTransactionService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_shipAddressService = shipAddressService;
		_carrierService = carrierService;
		_driverService = driverService;
		_userDialogs = userDialogs;
		_retailSalesDispatchTransactionService = retailSalesDispatchTransactionService;
		_wholeSalesDispatchTransactionService = wholeSalesDispatchTransactionService;
		_serviceProvider = serviceProvider;


		Title = "Sevk İşlemi";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		SaveCommand = new Command(async () => await SaveAsync());
		SelectWholeCommand = new Command(async () => await SelectWholeAsync());
		SelectRetailCommand = new Command(async () => await SelectRetailAsync());
		BackCommand = new Command(async () => await BackAsync());

		LoadShipAddressesCommand = new Command<SalesCustomer>(async (x) => await LoadShipAddressesAsync(x));
		LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
		LoadDriversCommand = new Command(async () => await LoadDriversAsync());
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadPageCommand { get; }
	public Command BackCommand { get; }
	public Command SaveCommand { get; }
	public Command ShowBasketItemCommand { get; }
	public Command<SalesCustomer> LoadShipAddressesCommand { get; }
	public Command LoadCarriersCommand { get; }
	public Command LoadDriversCommand { get; }

	public Command SelectWholeCommand { get; }
	public Command SelectRetailCommand { get; }


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
			if (_userDialogs.IsHudShowing)
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
	private async Task LoadShipAddressesAsync(SalesCustomer salesCustomer)
	{
		if (salesCustomer is null)
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

	private async Task LoadCarriersAsync()
	{
		if (IsBusy)
			return;
		try
		{
			Carriers.Clear();
			SelectedCarrier = null;
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _carrierService.GetObjects(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber

			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Carriers.Add(Mapping.Mapper.Map<Carrier>(item));
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
	}

	private async Task LoadDriversAsync()
	{
		if (IsBusy)
			return;
		try
		{
			Drivers.Clear();
			SelectedDriver = null;
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _driverService.GetObjects(
					httpClient: httpClient
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Drivers.Add(Mapping.Mapper.Map<Driver>(item));
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
	}

	private async Task SaveAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await OpenInsertOptionsAsync();

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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
	}

	private async Task SelectWholeAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirmInsert = await _userDialogs.ConfirmAsync("Siparişe Bağlı Toptan Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirmInsert)
			{
				await CloseInsertOptionsAsync();
				return;
			}


			_userDialogs.Loading("İşlem tamamlanıyor...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await WholeSalesDispatchTransactionInsertAsync(httpClient);

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

	private async Task SelectRetailAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirmInsert = await _userDialogs.ConfirmAsync("Siparişe Bağlı Perakende Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirmInsert)
			{
				await CloseInsertOptionsAsync();
				return;
			}

			_userDialogs.Loading("İşlem tamamlanıyor...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await RetailSalesDispatchTransactionInsertAsync(httpClient);

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


	private async Task WholeSalesDispatchTransactionInsertAsync(HttpClient httpClient)
	{
		var dto = new WholeSalesDispatchTransactionInsert
		{
			Code = "",
			CurrentCode = SalesCustomer != null ? SalesCustomer.Code : "",
			DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
			ShipInfoCode = SelectedShipAddress != null ? SelectedShipAddress.Code : "",
			DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
			CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
			IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
			Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
			IsEDispatch = 1,
			DispatchType = 0,
			DispatchStatus = 1,
			EDispatchProfileId = 1,
			Description = Description,
			DoCode = DocumentNumber,
			DocTrackingNumber = DocumentTrackingNumber,
			TransactionDate = TransactionDate,
			FirmNumber = _httpClientService.FirmNumber,
			SpeCode = SpecialCode,
			WarehouseNumber = WarehouseModel.Number,

		};

		foreach (var item in Items)
		{
			var wholeSalesDispatchTransactionLineDto = new WholeSalesDispatchTransactionLineInsert
			{
				ProductCode = item.ItemCode,
				WarehouseNumber = (short?)WarehouseModel.Number,
				Quantity = item.OutputQuantity,
				ConversionFactor = 1,
				OtherConversionFactor = 1,
				SubUnitsetCode = item.SubUnitsetCode,
			};

			foreach (var detail in item.Details)
			{
				var serilotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = detail.LocationCode,
					InProductTransactionLineReferenceId = detail.TransactionReferenceId,
					OutProductTransactionLineReferenceId = detail.ReferenceId,
					Quantity = detail.Quantity,
					SubUnitsetCode = item.SubUnitsetCode,
					DestinationStockLocationCode = string.Empty,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
				};

				wholeSalesDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
			}

			dto.Lines.Add(wholeSalesDispatchTransactionLineDto);
		}
		Console.WriteLine(dto);

		var result = await _wholeSalesDispatchTransactionService.InsertWholeSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

		ResultModel resultModel = new();
		if (result.IsSuccess)
		{
			resultModel.Message = "Başarılı";
			resultModel.Code = result.Data.Code;
			resultModel.PageTitle = Title;
			resultModel.PageCountToBack = 6;

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await ClearFormAsync();

			var basketViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
			basketViewModel.Items.Clear();
			basketViewModel.SelectedLocationTransactions.Clear();
			basketViewModel.SelectedSeriLotTransactions.Clear();

			await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
			{
				[nameof(ResultModel)] = resultModel
			});
		}
		else
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			resultModel.Message = "Başarısız";
			resultModel.PageTitle = Title;
			resultModel.ErrorMessage = result.Message;
			resultModel.PageCountToBack = 1;
			await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
			{
				[nameof(ResultModel)] = resultModel
			});
		}
	}

	private async Task RetailSalesDispatchTransactionInsertAsync(HttpClient httpClient)
	{
		var dto = new RetailSalesDispatchTransactionInsert
		{
			Code = "",
			CurrentCode = SalesCustomer != null ? SalesCustomer.Code : "",
			DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
			DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
			CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
			IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
			Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
			ShipInfoCode = SalesCustomer != null ? SalesCustomer.ShipAddressCode : "",
			IsEDispatch = 1,
			DispatchType = 0,
			DispatchStatus = 1,
			EDispatchProfileId = 1,
			Description = Description,
			DoCode = DocumentNumber,
			DocTrackingNumber = DocumentTrackingNumber,
			TransactionDate = TransactionDate,
			FirmNumber = _httpClientService.FirmNumber,
			SpeCode = SpecialCode,
			WarehouseNumber = WarehouseModel.Number,

		};

		foreach (var item in Items)
		{
			var retailSalesDispatchTransactionLineDto = new RetailSalesDispatchTransactionLineInsert
			{
				ProductCode = item.ItemCode,
				WarehouseNumber = (short?)WarehouseModel.Number,
				Quantity = item.OutputQuantity,
				ConversionFactor = 1,
				OtherConversionFactor = 1,
				SubUnitsetCode = item.SubUnitsetCode,
			};

			foreach (var detail in item.Details)
			{
				var serilotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = detail.LocationCode,
					InProductTransactionLineReferenceId = detail.TransactionReferenceId,
					OutProductTransactionLineReferenceId = detail.ReferenceId,
					Quantity = detail.Quantity,
					SubUnitsetCode = item.SubUnitsetCode,
					DestinationStockLocationCode = string.Empty,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
				};

				retailSalesDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
			}

			dto.Lines.Add(retailSalesDispatchTransactionLineDto);
		}
		Console.WriteLine(dto);

		var result = await _retailSalesDispatchTransactionService.InsertRetailSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

		ResultModel resultModel = new();
		if (result.IsSuccess)
		{
			resultModel.Message = "Başarılı";
			resultModel.Code = result.Data.Code;
			resultModel.PageTitle = Title;
			resultModel.PageCountToBack = 6;

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await ClearFormAsync();

			var basketViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
			basketViewModel.Items.Clear();
			basketViewModel.SelectedLocationTransactions.Clear();
			basketViewModel.SelectedSeriLotTransactions.Clear();

			await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
			{
				[nameof(ResultModel)] = resultModel
			});
		}
		else
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			resultModel.Message = "Başarısız";
			resultModel.PageTitle = Title;
			resultModel.ErrorMessage = result.Message;
			resultModel.PageCountToBack = 1;
			await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
			{
				[nameof(ResultModel)] = resultModel
			});
		}
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var result = await _userDialogs.ConfirmAsync("Form verileri silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!result)
				return;

			await ClearFormAsync();

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

	private async Task ClearFormAsync()
	{
		try
		{
			CargoTrackingNumber = string.Empty;
			DocumentNumber = string.Empty;
			SpecialCode = string.Empty;
			DocumentTrackingNumber = string.Empty;
			Description = string.Empty;
			CargoTrackingNumber = string.Empty;
			SelectedShipAddress = null;
			SelectedCarrier = null;
			SelectedDriver = null;

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task CloseInsertOptionsAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("insertOptionsBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task OpenInsertOptionsAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("insertOptionsBottomSheet").State = BottomSheetState.HalfExpanded;
		});
	}
}
