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
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

[QueryProperty(name: nameof(WorkOrderReworkBasketModel), queryId: nameof(WorkOrderReworkBasketModel))]
public partial class WorkOrderReworkProcessFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductionTransactionService _productionTransactionService;
    private readonly IConsumableTransactionService _consumableTransactionService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	WorkOrderReworkBasketModel workOrderReworkBasketModel = null!;

	public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

	[ObservableProperty]
	private DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	private string documentNumber = string.Empty;

	[ObservableProperty]
	private string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	private string specialCode = string.Empty;

	[ObservableProperty]
	private string description = string.Empty;

	public WorkOrderReworkProcessFormViewModel(IHttpClientService httpClientService, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_productionTransactionService = productionTransactionService;
		_consumableTransactionService = consumableTransactionService;
		_userDialogs = userDialogs;
		_locationTransactionService = locationTransactionService;
		_serviceProvider = serviceProvider;

		Title = "İş Emrine Bağlı Rework Form Sayfası";

		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		LoadPageCommand = new Command(async () => await LoadPageAsync());
		SaveCommand = new Command(async () => await SaveAsync());
		BackCommand = new Command(async () => await BackAsync());

	}

	public Page CurrentPage { get; set; } = null!;
	public Command SaveCommand { get; }
	public Command BackCommand { get; }
	public Command ShowBasketItemCommand { get; }
	public Command LoadPageCommand { get; }

	public async Task ShowBasketItemAsync()
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

			var groupByWarehouseList = WorkOrderReworkBasketModel.WorkOrderReworkSubProducts.GroupBy(x => x.WarehouseModel.Number).ToList();

			int groupByWarehouseListCount = groupByWarehouseList.Count;
			int successCount = 0;

			resultModel.Code = "Üretimden Giriş Numaraları: ";
			foreach(var groupByWarehouse in groupByWarehouseList)
			{
				productionTransactionInsertResult = await ProductionTransactionInsert(httpClient, groupByWarehouse.ToList());
				if(productionTransactionInsertResult.IsSuccess)
				{
					successCount += 1;
					resultModel.Code += productionTransactionInsertResult.Data?.Code + ", ";
				}
			}

			if(successCount == groupByWarehouseListCount)
			{
				consumableTransactionResult = await ConsumableTransactionInsert(httpClient, WorkOrderReworkBasketModel);

				if(consumableTransactionResult.IsSuccess)
				{
					resultModel.Code += "Sarf Fişi Numarası: " + consumableTransactionResult.Data?.Code;
					resultModel.Message = "İşlem Başarılı";
					resultModel.PageTitle = "Sarf Fişi ve Üretimden Giriş fişleri";
					resultModel.PageCountToBack = 4;

					// Clear Form, basket model, and all related pages
					TransactionDate = DateTime.Now;
					DocumentNumber = string.Empty;
					DocumentTrackingNumber = string.Empty;
					SpecialCode = string.Empty;
					Description = string.Empty;

					var productListViewModel = _serviceProvider.GetRequiredService<WorkOrderReworkProcessProductListViewModel>();
					productListViewModel.SelectedItem = null;

					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

					await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});					
				}
				else if (!consumableTransactionResult.IsSuccess)
				{
					resultModel.ErrorMessage = consumableTransactionResult.Message;
					resultModel.Message = "Sarf fişi başarısız, Üretimden Giriş fişleri başarılı";
					resultModel.PageTitle = "";
					resultModel.PageCountToBack = 1;

					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();
					await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});
				}
			}
			else
			{
				resultModel.PageCountToBack = 1;
				resultModel.Message = "Başarısız";
				resultModel.ErrorMessage = $@"{productionTransactionInsertResult.Message} \n {consumableTransactionResult.Message}";

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = resultModel
				});
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
	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Form verileriniz silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!confirm)
            {
				return;
            }

            TransactionDate = DateTime.Now;
			DocumentNumber = string.Empty;
			DocumentTrackingNumber = string.Empty;
			SpecialCode = string.Empty;
			Description = string.Empty;
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

	private async Task<DataResult<ResponseModel>> ConsumableTransactionInsert(HttpClient httpClient, WorkOrderReworkBasketModel reworkBasketModel)
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
			WarehouseNumber = reworkBasketModel.WorkOrderReworkMainProductModel.WarehouseNumber,
		};

		var consumableTransactionLineDto = new ConsumableTransactionLineDto
		{
			ProductCode = reworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? reworkBasketModel.WorkOrderReworkMainProductModel.MainItemCode : reworkBasketModel.WorkOrderReworkMainProductModel.Code,
			VariantCode = reworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? reworkBasketModel.WorkOrderReworkMainProductModel.Code : string.Empty,
			WarehouseNumber = reworkBasketModel.WorkOrderReworkMainProductModel.WarehouseNumber,
			Quantity = reworkBasketModel.BOMQuantity,
			ConversionFactor = 1,
			OtherConversionFactor = 1,
			SubUnitsetCode = reworkBasketModel.WorkOrderReworkMainProductModel.SubUnitsetCode,
		};
		var tempItemQuantity = reworkBasketModel.BOMQuantity;
		foreach (var detail in reworkBasketModel.MainProductLocationTransactions)
		{
			var tempDetailQuantity = detail.RemainingQuantity;
			await LoadLocationTransaction(detail);
			LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

			foreach (var locationTransaction in LocationTransactions)
			{
				var tempLocationTransaction = locationTransaction.RemainingQuantity;
				while (tempLocationTransaction > 0 && tempDetailQuantity > 0 && tempItemQuantity>0)
				{
					var seriLotTransactionDto = new SeriLotTransactionDto
					{
						StockLocationCode = detail.LocationCode,
						DestinationStockLocationCode = string.Empty,
						InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
						OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
						ConversionFactor = 1,
						OtherConversionFactor = 1,
						SubUnitsetCode = reworkBasketModel.WorkOrderReworkMainProductModel.SubUnitsetCode,
						Quantity = tempDetailQuantity > tempLocationTransaction ? tempLocationTransaction : tempDetailQuantity,
					};
					consumableTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
					tempLocationTransaction -= (double)seriLotTransactionDto.Quantity;
					tempDetailQuantity -= (double)seriLotTransactionDto.Quantity;
					tempItemQuantity -= (double)seriLotTransactionDto.Quantity;
				}
			}
		}

		consumableTransactionDto.Lines.Add(consumableTransactionLineDto);

		var result = await _consumableTransactionService.InsertConsumableTransaction(httpClient, consumableTransactionDto, _httpClientService.FirmNumber);

		return result;
	}

	private async Task<DataResult<ResponseModel>> ProductionTransactionInsert(HttpClient httpClient, List<WorkOrderReworkSubProductModel> subProducts)
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
			WarehouseNumber = subProducts.FirstOrDefault().WarehouseModel.Number,
			Description = Description,
		};


		foreach (var subProduct in subProducts)
		{
			var productionTransactionLineDto = new ProductionTransactionLineDto
			{
				ProductCode = subProduct.ProductModel.IsVariant ? subProduct.ProductModel.MainProductCode : subProduct.ProductModel.Code,
				VariantCode = subProduct.ProductModel.IsVariant ? subProduct.ProductModel.Code : string.Empty,
				WarehouseNumber = subProduct.WarehouseModel.Number,
				Quantity = subProduct.SubBOMQuantity,
				ConversionFactor = 1,
				OtherConversionFactor = 1,
				SubUnitsetCode = subProduct.ProductModel.SubUnitsetCode,
				UnitPrice = 0,
				VatRate = 0,
			};

			foreach (var detail in subProduct.Locations)
			{
				var seriLotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = detail.Code,
					Quantity = detail.InputQuantity,
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

	private async Task LoadLocationTransaction(GroupLocationTransactionModel groupLocationTransactionModel)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			LocationTransactions.Clear();
			var result = await  _locationTransactionService.GetInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.MainItemReferenceId : WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.ReferenceId,
				variantReferenceId: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.ReferenceId : 0,
				warehouseNumber: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.WarehouseNumber,
				locationRef: groupLocationTransactionModel.LocationReferenceId,
				skip: 0,
				take: 9999,
				search: string.Empty
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
				}
			}
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

}
