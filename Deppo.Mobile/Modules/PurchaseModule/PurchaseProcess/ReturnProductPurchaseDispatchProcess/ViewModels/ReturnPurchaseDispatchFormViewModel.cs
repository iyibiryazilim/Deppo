using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
[QueryProperty(name: nameof(PurchaseFicheModel), queryId: nameof(PurchaseFicheModel))]
[QueryProperty(name: nameof(SelectedPurchaseTransactions), queryId: nameof(SelectedPurchaseTransactions))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]

public partial class ReturnPurchaseDispatchFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IPurchaseReturnDispatchTransactionService _purchaseReturnDispatchTransactionService;
	private readonly IUserDialogs _userDialogs;
	private readonly ICarrierService _carrierService;
	private readonly IDriverService _driverService;
	private readonly IServiceProvider _serviceProvider;


	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	PurchaseSupplier purchaseSupplier = null!;

	[ObservableProperty]
	PurchaseFicheModel purchaseFicheModel = null!;

	[ObservableProperty]
	public ObservableCollection<PurchaseTransactionModel> selectedPurchaseTransactions;
	[ObservableProperty]
	public ObservableCollection<ReturnPurchaseBasketModel> items = null!;


	[ObservableProperty]
	Carrier? selectedCarrier;
	[ObservableProperty]
	Driver? selectedDriver;

	public ObservableCollection<Carrier> Carriers { get; } = new();
	public ObservableCollection<Driver> Drivers { get; } = new();


	[ObservableProperty]
	string documentNumber = string.Empty;

	[ObservableProperty]
	DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	string description = string.Empty;

	[ObservableProperty]
	string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	string specialCode = string.Empty;

    public ReturnPurchaseDispatchFormViewModel(IHttpClientService httpClientService, IPurchaseReturnDispatchTransactionService purchaseReturnDispatchTransactionService, IUserDialogs userDialogs, ICarrierService carrierService, IDriverService driverService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _purchaseReturnDispatchTransactionService = purchaseReturnDispatchTransactionService;
        _userDialogs = userDialogs;
        Items = new();

        Title = "Satınalma İade İrsaliyesi";

        SaveCommand = new Command(async () => await SaveAsync());
        LoadPageCommand = new Command(async () => await LoadPageAsync());
        LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
        LoadDriversCommand = new Command(async () => await LoadDriversAsync());
        BackCommand = new Command(async () => await BackAsync());
        _carrierService = carrierService;
        _driverService = driverService;
        _serviceProvider = serviceProvider;
    }
    public Page CurrentPage { get; set; }

	public Command SaveCommand { get; }
	public Command LoadPageCommand { get; }
	public Command LoadCarriersCommand { get; }
	public Command LoadDriversCommand { get; }

	public Command BackCommand { get; }

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
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public static string GetEnumDescription(Enum value)
	{
		FieldInfo fi = value.GetType().GetField(value.ToString());

		DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

		if (attributes != null && attributes.Any())
		{
			return attributes.First().Description;
		}

		return value.ToString();
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

            var confirm = await _userDialogs.ConfirmAsync("Satınalma İade İrsaliyesi oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!confirm)
                return;


            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var dto = new PurchaseReturnDispatchTransactionInsert
			{
				Code = "",
				CurrentCode = PurchaseSupplier.Code,
				DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
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
				var purchaseReturnDispatchTransactionLineDto = new PurchaseReturnDispatchTransactionLineDto
				{
					ProductCode = item.ItemCode,
					WarehouseNumber = (short?)WarehouseModel.Number,
					Quantity = item.Quantity,
					DispatchReferenceId = item.DispatchReferenceId,
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
						Quantity = detail.RemainingQuantity,
						SubUnitsetCode = item.SubUnitsetCode,
						DestinationStockLocationCode = string.Empty,
						ConversionFactor = 1,
						OtherConversionFactor = 1,
					};

					purchaseReturnDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
				}

				dto.Lines.Add(purchaseReturnDispatchTransactionLineDto);
			}
			Console.WriteLine(dto);

			var result = await _purchaseReturnDispatchTransactionService.InsertPurchaseReturnDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

			ResultModel resultModel = new();
			if (result.IsSuccess)
			{
				resultModel.Message = "Başarılı";
				resultModel.Code = result.Data.Code;
				resultModel.PageTitle = Title;
				resultModel.PageCountToBack = 7;

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await ClearFormAsync();

                var basketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseDispatchBasketViewModel>();
			    basketViewModel.Items.Clear();
				basketViewModel.SelectedPurchaseTransactions.Clear();
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

            DocumentNumber = string.Empty;
            SpecialCode = string.Empty;
            DocumentTrackingNumber = string.Empty;
            Description = string.Empty;
            SelectedCarrier = null;
            SelectedDriver = null;

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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
            SelectedCarrier = null;
            SelectedDriver = null;

        }
        catch (Exception ex)
		{

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
	}
}
