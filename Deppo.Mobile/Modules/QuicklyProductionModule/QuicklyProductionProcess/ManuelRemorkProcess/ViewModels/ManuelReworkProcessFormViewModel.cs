using Android.Views.Accessibility;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IProductionTransactionService _productionTransactionService;
	private readonly IConsumableTransactionService _consumableTransactionService;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;


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

	public ManuelReworkProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_productionTransactionService = productionTransactionService;
		_consumableTransactionService = consumableTransactionService;

		Title = "Form Sayfası";

		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		LoadPageCommand = new Command(async () => await LoadPageAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Page CurrentPage { get; set; } = null!;
	public Command ShowBasketItemCommand { get; }
	public Command LoadPageCommand { get; }

	public Command SaveCommand { get; }
	public Command BackCommand { get; }

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

			_userDialogs.Loading("Loading");
			CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

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

	private async Task SaveAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Kaydetmek istediğinize emin misiniz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			_userDialogs.Loading("İşlem Tamamlanıyor");
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			ResultModel resultModel = new(); // for Success and Failure page
			var productionTransactionInsertResult = new DataResult<ResponseModel>();  // for ProductionTransaction insert result
			var consumableTransactionResult = new DataResult<ResponseModel>(); // for ConsumableTransaction insert result

			var groupByWarehouseList = ReworkBasketModel.ReworkInProducts.GroupBy(x => x.InWarehouseModel.Number).ToList();

			int groupByWarehouseListCount = groupByWarehouseList.Count;
			int successCount = 0;

			resultModel.Code = "Üretimden Giriş Numaraları: ";
			foreach(var groupByWarehouse in groupByWarehouseList)
			{
				productionTransactionInsertResult = await ProductionTransactionInsert(httpClient, groupByWarehouse.ToList());
				if(productionTransactionInsertResult.IsSuccess)
				{
					successCount += 1;
					resultModel.Code += productionTransactionInsertResult.Data.Code + ", ";
				}
			}

			if(successCount == groupByWarehouseListCount)
			{
				 consumableTransactionResult = await ConsumableTransactionInsert(httpClient, ReworkBasketModel);

				if(consumableTransactionResult.IsSuccess)
				{
					resultModel.Code += "Sarf Fişi Numarası: " + consumableTransactionResult.Data.Code;
					resultModel.Message = "İşlem Başarılı";
					resultModel.PageTitle = "Sarf Fişi ve Üretimden Giriş fişleri";
					resultModel.PageCountToBack = 5;

					await ClearFormAsync();

                    // Basket modeli tümüyle temizlenmeli detaylarıyla beraber
                    foreach (var item in ReworkBasketModel.ReworkInProducts)
                    {
						item.Details.Clear();
                    }
					ReworkBasketModel.ReworkInProducts.Clear();
					ReworkBasketModel.ReworkOutProductModel.Details.Clear();
					await ClearFormAsync();

					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

					await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});
				}
				else if (!consumableTransactionResult.IsSuccess)
				{
					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

					resultModel.Code = productionTransactionInsertResult.Data.Code;
					resultModel.Message = "Sarf fişi başarısız, Üretimden Giriş fişleri başarılı";
					resultModel.PageTitle = "";
					resultModel.PageCountToBack = 1;
					await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});
				}
			}
			else
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				resultModel.PageCountToBack = 1;
				resultModel.Message = "Başarısız";
				resultModel.ErrorMessage = $@"{productionTransactionInsertResult.Message} \n {consumableTransactionResult.Message}";

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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}


	private async Task<DataResult<ResponseModel>> ConsumableTransactionInsert(HttpClient httpClient, ReworkBasketModel reworkBasketModel)
	{
		var consumableTransactionDto = new ConsumableTransactionInsert
		{
			Code = "",
			CurrentCode = "",
			Description = Description,
			DoCode = DocumentNumber,
			DocTrackingNumber = DocumentTrackingNumber,
			TransactionDate = TransactionDate,
			FirmNumber = _httpClientService.FirmNumber,
			SpeCode = SpecialCode,
			WarehouseNumber = reworkBasketModel.OutWarehouseModel.Number,
		};

		var consumableTransactionLineDto = new ConsumableTransactionLineDto
		{
			ProductCode = reworkBasketModel.ReworkOutProductModel.Code,
			WarehouseNumber = reworkBasketModel.OutWarehouseModel.Number,
			Quantity = reworkBasketModel.ReworkOutProductModel.OutputQuantity,
			ConversionFactor = 1,
			OtherConversionFactor = 1,
			SubUnitsetCode = reworkBasketModel.ReworkOutProductModel.SubUnitsetCode,
		};

		foreach (var detail in reworkBasketModel.ReworkOutProductModel.Details)
		{
			var seriLotTransactionDto = new SeriLotTransactionDto
			{
				Quantity = detail.Quantity,
				SubUnitsetCode = reworkBasketModel.ReworkOutProductModel.SubUnitsetCode,
				ConversionFactor = 1,
				OtherConversionFactor = 1,
				DestinationStockLocationCode = string.Empty,
				StockLocationCode = detail.LocationCode,
				InProductTransactionLineReferenceId = detail.TransactionReferenceId,
				OutProductTransactionLineReferenceId = detail.ReferenceId
			};

			consumableTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
		}

		consumableTransactionDto.Lines.Add(consumableTransactionLineDto);

		var result = await _consumableTransactionService.InsertConsumableTransaction(httpClient, consumableTransactionDto, _httpClientService.FirmNumber);
		return result;
	}

	private async Task<DataResult<ResponseModel>> ProductionTransactionInsert(HttpClient httpClient, List<ReworkInProductModel> reworkInProductModels)
	{
		
		var productionTransactionDto = new ProductionTransactionInsert
		{
			SpeCode = SpecialCode,
			CurrentCode = string.Empty,
			Code = string.Empty,
			DocTrackingNumber = DocumentTrackingNumber,
			DoCode = DocumentNumber,
			TransactionDate = TransactionDate,
			FirmNumber = _httpClientService.FirmNumber,
			WarehouseNumber = reworkInProductModels.FirstOrDefault().InWarehouseModel.Number,
			Description = Description,
		};


		foreach (var reworkInProductModel in reworkInProductModels)
		{
			var productionTransactionLineDto = new ProductionTransactionLineDto
			{
				ProductCode = reworkInProductModel.Code,
				WarehouseNumber = reworkInProductModel.InWarehouseModel.Number,
				Quantity = reworkInProductModel.InputQuantity,
				ConversionFactor = 1,
				OtherConversionFactor = 1,
				SubUnitsetCode = reworkInProductModel.SubUnitsetCode,
				UnitPrice = 0,
				VatRate = 0,
			};

			foreach (var detail in reworkInProductModel.Details)
			{
				var seriLotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = detail.LocationCode,
					Quantity = detail.Quantity,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
					DestinationStockLocationCode = string.Empty,
				};

				productionTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
			}
			productionTransactionDto.Lines.Add(productionTransactionLineDto);
		}
		
		var result = await _productionTransactionService.InsertProductionTransaction(httpClient, productionTransactionDto, _httpClientService.FirmNumber);
		return result;
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
			TransactionDate = DateTime.Now;
			Description = string.Empty;
			DocumentTrackingNumber = string.Empty;
			SpecialCode = string.Empty;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
