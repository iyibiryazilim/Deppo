using Android.App;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.DTOs.RetailSalesReturnDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.WholeSalesReturnDispatchTransaction;
using Deppo.Core.Models;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
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
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
[QueryProperty(name: nameof(SalesFicheModel), queryId: nameof(SalesFicheModel))]
public partial class ReturnSalesDispatchFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IRetailSalesReturnDispatchTransactionService _retailService;
	private readonly IWholeSalesReturnDispatchTransactionService _wholeService;
	private readonly IShipAddressService _shipAddressService;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private DateTime ficheDate = DateTime.Now;

	[ObservableProperty]
	private string documentNumber = string.Empty;

	[ObservableProperty]
	private string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	private string specialCode = string.Empty;

	[ObservableProperty]
	private string description = string.Empty;

	[ObservableProperty]
	private SalesCustomer salesCustomer;

	[ObservableProperty]
	private ObservableCollection<ReturnSalesBasketModel> items = null!;

	[ObservableProperty]
	private SalesFicheModel salesFicheModel = null!;

	[ObservableProperty]
	private string ficheNo = string.Empty;

	[ObservableProperty]
	private ShipAddressModel? selectedShipAddress;

	public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

	public ReturnSalesDispatchFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IRetailSalesReturnDispatchTransactionService retailSalesDispatchTransactionService, IWholeSalesReturnDispatchTransactionService wholeSalesReturnDispatchTransactionService, IShipAddressService shipAddressService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		Items = new();

		Title = "Satış İade İşlemi";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		LoadShipAddressesCommand = new Command<SalesCustomer>(async (x) => await LoadShipAddressesAsync(x));
		BackCommand = new Command(async () => await BackAsync());

		SaveCommand = new Command(async () => await SaveAsync());

		_retailService = retailSalesDispatchTransactionService;
		_wholeService = wholeSalesReturnDispatchTransactionService;
		_shipAddressService = shipAddressService;
		_serviceProvider = serviceProvider;
	}

	public Page CurrentPage { get; set; }

	public Command LoadPageCommand { get; }
	public Command BackCommand { get; }
	public Command SaveCommand { get; }
	public Command ShowBasketItemCommand { get; }
	public Command<SalesCustomer> LoadShipAddressesCommand { get; }

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
		catch (System.Exception)
		{
			throw;
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

			// Title = GetEnumDescription(InputProductProcessType);
			CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
		}
		catch (System.Exception)
		{
			throw;
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void OpenBottomSheetAsync()
	{
		// BottomSheet'i aç
		CurrentPage.FindByName<BottomSheet>("selectConfirmBottomSheet").State = BottomSheetState.HalfExpanded;
	}

	//private async Task SelectTransactionTypeAsync(SalesReturnEnumType selectedType)
	//{
	//    // BottomSheet'i kapat
	//    CurrentPage.FindByName<BottomSheet>("selectConfirmBottomSheet").State = BottomSheetState.Hidden;

	//    // SaveCommand'i tetikle
	//    await SaveAsync();
	//}

	//public static string GetEnumDescription(Enum value)
	//{
	//    FieldInfo fi = value.GetType().GetField(value.ToString());

	//    DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

	//    if (attributes != null && attributes.Any())
	//    {
	//        return attributes.First().Description;
	//    }

	//    return value.ToString();
	//}

	private async Task SaveAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("İşlem Tamamlanıyor...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			DataResult<ResponseModel> result = new();

			if (SalesFicheModel.FicheType == 7)
			{
				RetailSalesReturnDispatchTransactionInsert dto = new()
				{
					ShipInfoCode = SelectedShipAddress?.Code ?? string.Empty,
					SpeCode = SpecialCode,
					CurrentCode = SalesCustomer.Code,
					Code = FicheNo,
					DocTrackingNumber = DocumentTrackingNumber,
					DoCode = DocumentNumber,
					TransactionDate = FicheDate,
					FirmNumber = _httpClientService.FirmNumber,
					WarehouseNumber = WarehouseModel.Number,
					Description = Description
				};

				foreach (var item in Items)
				{


					var salesReturnDispatchTransactionLineInsert = new RetailSalesReturnDispatchTransactionLineInsert
					{
						ProductCode = item.ItemCode,
						WarehouseNumber = (short)WarehouseModel.Number,
						Quantity = item.InputQuantity,
						ConversionFactor = item.InputQuantity * item.ConversionFactor,
						OtherConversionFactor = item.InputQuantity * item.OtherConversionFactor,
						SubUnitsetCode = item.SubUnitsetCode,
						DispatchReferenceId = item.DispatchReferenceId,
						Description = Description,
						SpeCode = SpecialCode,
					};

					dto.Lines.Add(salesReturnDispatchTransactionLineInsert);

					foreach (var detail in item.Details)
					{
						var seriLotTransactionDto = new SeriLotTransactionDto
						{
							StockLocationCode = detail.LocationCode,
							SubUnitsetCode = item.SubUnitsetCode,
							Quantity = detail.Quantity ,
							ConversionFactor = detail.Quantity * item.ConversionFactor,
							OtherConversionFactor = detail.Quantity * item.OtherConversionFactor,
							DestinationStockLocationCode = string.Empty,
						};

						salesReturnDispatchTransactionLineInsert.SeriLotTransactions.Add(seriLotTransactionDto);
					}
				}

				result = await _retailService.InsertRetailSalesReturnDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, dto);
			}
			if (SalesFicheModel.FicheType == 8)
			{
				WholeSalesReturnDispatchTransactionInsert dto = new()
				{
					ShipInfoCode = SelectedShipAddress?.Code ?? string.Empty,
					SpeCode = SpecialCode,
					CurrentCode = SalesCustomer.Code,
					Code = FicheNo,
					DocTrackingNumber = DocumentTrackingNumber,
					DoCode = DocumentNumber,
					TransactionDate = FicheDate,
					FirmNumber = _httpClientService.FirmNumber,
					WarehouseNumber = WarehouseModel.Number,
					Description = Description
				};

				foreach (var item in Items)
				{


					var wholeSalesDispatchTransactionLineInsert = new WholeSalesReturnTransactionLineInsert
					{
						ProductCode = item.ItemCode,
						WarehouseNumber = (short)WarehouseModel.Number,
						Quantity = item.InputQuantity,
						ConversionFactor = item.InputQuantity * item.ConversionFactor,
						OtherConversionFactor = item.InputQuantity * item.OtherConversionFactor,
						SubUnitsetCode = item.SubUnitsetCode,
						DispatchReferenceId = item.DispatchReferenceId,
						Description = Description,
						SpeCode = SpecialCode,
					};

					dto.Lines.Add(wholeSalesDispatchTransactionLineInsert);

					foreach (var detail in item.Details)
					{
						var seriLotTransactionDto = new SeriLotTransactionDto
						{
							StockLocationCode = detail.LocationCode,
							SubUnitsetCode = item.SubUnitsetCode,
							Quantity = detail.Quantity,
							ConversionFactor = detail.Quantity * item.ConversionFactor,
							OtherConversionFactor = detail.Quantity * item.OtherConversionFactor,
							DestinationStockLocationCode = string.Empty,
						};

						wholeSalesDispatchTransactionLineInsert.SeriLotTransactions.Add(seriLotTransactionDto);
					}
				}

				result = await _wholeService.InsertWholeSalesReturnDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, dto);
			}
			Console.WriteLine(result);
			ResultModel resultModel = new();
			if (result.IsSuccess)
			{
				resultModel.Message = "Başarılı" + result.Message;
				resultModel.Code = result.Data.Code;
				resultModel.PageTitle = Title;
				resultModel.PageCountToBack = 7;

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = resultModel
				});


				var basketModel = _serviceProvider.GetRequiredService<ReturnSalesDispatchBasketViewModel>();
				var locationBasketModel = _serviceProvider.GetRequiredService<ReturnSalesDispatchBasketLocationListViewModel>();

				locationBasketModel.SelectedItems.Clear();

				foreach (var item in basketModel.Items)
				{
					item.Details.Clear();
				}
				basketModel.Items.Clear();

				FicheNo = string.Empty;
				DocumentNumber = string.Empty;
				DocumentTrackingNumber = string.Empty;
				SpecialCode = string.Empty;
				Description = string.Empty;
				SelectedShipAddress = null;
				Items.Clear();

			}
			else
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				resultModel.Message = "Başarısız";
				resultModel.ErrorMessage = result.Message;
				resultModel.PageTitle = Title;
				resultModel.PageCountToBack = 4;
				await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = resultModel
				});
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally { IsBusy = false; }
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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
			DocumentNumber = string.Empty;
			SpecialCode = string.Empty;
			DocumentTrackingNumber = string.Empty;
			Description = string.Empty;
			SelectedShipAddress = null;
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}