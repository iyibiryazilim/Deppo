using Android.Views.Accessibility;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IProductionTransactionService _productionTransactionService;
	private readonly IConsumableTransactionService _consumableTransactionService;
	private readonly IServiceProvider _serviceProvider;
	private readonly ILocationTransactionService _locationTransactionService;

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

	public ManuelReworkProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_productionTransactionService = productionTransactionService;
		_consumableTransactionService = consumableTransactionService;
		_serviceProvider = serviceProvider;
		_locationTransactionService = locationTransactionService;

		Title = "Form Sayfası";

		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		LoadPageCommand = new Command(async () => await LoadPageAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();
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

	public async Task LoadLocationTransaction(ReworkOutProductModel reworkOutProductModel, ReworkOutProductDetailModel detail)
	{
		try
		{

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			LocationTransactions.Clear();
			var result = await _locationTransactionService.GetInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: reworkOutProductModel.IsVariant ? reworkOutProductModel.MainItemReferenceId : reworkOutProductModel.ReferenceId,
				variantReferenceId: reworkOutProductModel.IsVariant ? reworkOutProductModel.ReferenceId : 0,
				warehouseNumber: reworkOutProductModel.WarehouseNumber,
				locationRef: detail.LocationReferenceId,
				skip: 0,
				take: 999999,
				search: ""
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
				}

			}

			_userDialogs.Loading().Hide();
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
					if(ReworkBasketModel is not null)
					{
						ReworkBasketModel.ReworkInProducts?.Clear();
						ReworkBasketModel.ReworkOutProductModel?.Details?.Clear();
					}

					var outWarehouseListViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessOutWarehouseListViewModel>() ?? throw new InvalidOperationException("Service not found");
					var warehouseTotalListViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessWarehouseTotalListViewModel>() ?? throw new InvalidOperationException("Service not found"); 
					var inWarehouseListViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessInWarehouseListViewModel>() ?? throw new InvalidOperationException("Service not found");
					var allProductListViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessAllProductListViewModel>() ?? throw new InvalidOperationException("Service not found");
					var basketLocationListViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessBasketLocationListViewModel>() ?? throw new InvalidOperationException("Service not found");
					var basketViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessBasketViewModel>() ?? throw new InvalidOperationException("Service not found");

					await Task.WhenAll(ClearFormAsync(), outWarehouseListViewModel.ClearPageAsync(), warehouseTotalListViewModel.ClearPageAsync(), inWarehouseListViewModel.ClearPageAsync(), allProductListViewModel.ClearPageAsync(), basketLocationListViewModel.ClearPageAsync(), basketViewModel.ClearPageAsync());


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
			ProductCode = reworkBasketModel.ReworkOutProductModel.IsVariant ? reworkBasketModel.ReworkOutProductModel.MainItemCode : reworkBasketModel.ReworkOutProductModel.Code,
			VariantCode = reworkBasketModel.ReworkOutProductModel.IsVariant ? reworkBasketModel.ReworkOutProductModel.Code : string.Empty,
			WarehouseNumber = reworkBasketModel.OutWarehouseModel.Number,
			Quantity = reworkBasketModel.ReworkOutProductModel.OutputQuantity,
			ConversionFactor = 1,
			OtherConversionFactor = 1,
			SubUnitsetCode = reworkBasketModel.ReworkOutProductModel.SubUnitsetCode,
		};
		var tempItemQuantity = reworkBasketModel.ReworkOutProductModel.OutputQuantity;
		foreach (var detail in reworkBasketModel.ReworkOutProductModel.Details)
		{
			var tempDetailQuantity = detail.Quantity;
			await LoadLocationTransaction(reworkBasketModel.ReworkOutProductModel, detail);
			LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

			foreach (var locationTransaction in LocationTransactions)
			{
				var tempLocationTransactionQuantity = locationTransaction.RemainingQuantity;

				while (tempLocationTransactionQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity > 0)
				{
					var serilotTransactionDto = new SeriLotTransactionDto
					{
						StockLocationCode = detail.LocationCode,
						InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
						OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
						Quantity = tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity,
						SubUnitsetCode = reworkBasketModel.ReworkOutProductModel.SubUnitsetCode,
						DestinationStockLocationCode = string.Empty,
						ConversionFactor = 1,
						OtherConversionFactor = 1,
					};
					consumableTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
					tempLocationTransactionQuantity -= (double)serilotTransactionDto.Quantity;
					tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
					tempItemQuantity -= (double)serilotTransactionDto.Quantity;
				}
			}

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
				ProductCode = reworkInProductModel.IsVariant ? reworkInProductModel.MainItemCode : reworkInProductModel.Code,
				VariantCode = reworkInProductModel.IsVariant ? reworkInProductModel.Code : string.Empty,
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
