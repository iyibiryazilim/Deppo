using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

[QueryProperty(name: nameof(TransferBasketModel), queryId:nameof(TransferBasketModel))]
public partial class TransferFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ITransferTransactionService _transferTransactionService;
	
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	TransferBasketModel transferBasketModel = null!;

	[ObservableProperty]
	DateTime ficheDate = DateTime.Now;

	[ObservableProperty]
	string documentNumber = string.Empty;

	[ObservableProperty]
	string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	string specialCode = string.Empty;

	[ObservableProperty]
	string description = string.Empty;



	public TransferFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ITransferTransactionService transferTransactionService)
	{
		_httpClientService = httpClientService;
		_transferTransactionService = transferTransactionService;
		_userDialogs = userDialogs;

		Title = "Ambar Transfer Formu";
		//LoadPageCommand = new Command(async () => await LoadPageAsync());
		//ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		SaveCommand = new Command(async () => await SaveAsync());
	}

	public Page CurrentPage { get; set; }

	public Command LoadPageCommand { get; }
	public Command BackCommand { get; }
	public Command SaveCommand { get; }
	public Command ShowBasketItemCommand { get; }

	//private async Task ShowBasketItemAsync()
	//{
	//	if (IsBusy)
	//		return;

	//	try
	//	{
	//		IsBusy = true;

	//		CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
	//	}
	//	catch (System.Exception)
	//	{

	//		throw;
	//	}
	//	finally
	//	{
	//		IsBusy = false;
	//	}
	//}

	//private async Task LoadPageAsync()
	//{
	//	if (IsBusy)
	//		return;

	//	try
	//	{
	//		IsBusy = true;

	//		CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;


	//	}
	//	catch (System.Exception)
	//	{

	//		throw;
	//	}
	//	finally
	//	{
	//		IsBusy = false;
	//	}
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

			var transferTransactionInsertDto = new TransferTransactionInsert
			{
				SpeCode = SpecialCode,
				CurrentCode = string.Empty,
				Code = string.Empty,
				DocTrackingNumber = DocumentTrackingNumber,
				DoCode = DocumentNumber,
				TransactionDate = FicheDate,
				FirmNumber = _httpClientService.FirmNumber,
				WarehouseNumber = TransferBasketModel.OutWarehouse.Number,
				DestinationWarehouseNumber = TransferBasketModel.InWarehouse.Number,
				Description = Description,
			};

			foreach (var item in TransferBasketModel.InProducts)
			{
				var transferTransactionLineDto = new TransferTransactionLineDto
				{
					ProductCode = item.Code,
					WarehouseNumber = TransferBasketModel.OutWarehouse.Number,
					DestinationWarehouseNumber = TransferBasketModel.InWarehouse.Number,
					Quantity = item.OutputQuantity,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
					SubUnitsetCode = item.SubUnitsetCode,
				};

				//foreach (var detail in item.Locations)
				//{
				//	var seriLotTransactionDto = new SeriLotTransactionDto
				//	{
				//		StockLocationCode = detail.LocationCode,
				//		Quantity = detail.In,
				//		ConversionFactor = 1,
				//		OtherConversionFactor = 1,
				//		DestinationStockLocationCode = string.Empty,
				//	};

				//	transferTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
				//}

				transferTransactionInsertDto.Lines.Add(transferTransactionLineDto);
			}

			var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);
			Console.WriteLine(result);
			ResultModel resultModel = new();

			if (result.IsSuccess)
			{
				resultModel.Message = "Başarılı";
				resultModel.Code = result.Data.Code;
				resultModel.PageTitle = "Ambar Transferi";
				resultModel.PageCountToBack = 7;

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

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
				resultModel.PageTitle = "Ambar Transferi";
				resultModel.PageCountToBack = 1;
				resultModel.ErrorMessage = result.Message;

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
		}
	}
}
