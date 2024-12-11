using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
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

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;


[QueryProperty(name: nameof(InputOutsourceTransferV2BasketModel), queryId: nameof(InputOutsourceTransferV2BasketModel))]
public partial class InputOutsourceTransferV2FormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	private readonly IProductionTransactionService _productionTransactionService;
	private readonly IConsumableTransactionService _consumableTransactionService;
	private readonly ILocationTransactionService _locationTransactionService;

	[ObservableProperty]
	InputOutsourceTransferV2BasketModel? inputOutsourceTransferV2BasketModel;


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


	private ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();


	public InputOutsourceTransferV2FormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService, ILocationTransactionService locationTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_productionTransactionService = productionTransactionService;
		_consumableTransactionService = consumableTransactionService;
		_locationTransactionService = locationTransactionService;

		Title = "Fason Kabul Formu";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		SaveCommand = new Command(async () => await SaveAsync());
		BackCommand = new Command(async () => await BackAsync());
	}
	public Page CurrentPage { get; set; } = null!;
	public Command ShowBasketItemCommand { get; }
	public Command LoadPageCommand { get; }
	public Command SaveCommand { get; }
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

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

	private async Task SaveAsync()
	{
		if (IsBusy)
			return;
		try
		{
			var confirm = await _userDialogs.ConfirmAsync("Kaydetmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			IsBusy = true;

			_userDialogs.Loading("Kaydediliyor...");
			await Task.Delay(500);
			ResultModel resultModel = new ResultModel();

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var consumableTransactionDtoResult = await CreateConsumableTransactionInsertDTO(httpClient, InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts);
			var consumableTransactionResult = await _consumableTransactionService.InsertConsumableTransaction(httpClient, consumableTransactionDtoResult, _httpClientService.FirmNumber);

			if(consumableTransactionResult.IsSuccess)
			{
				var productionTransactionDtoResult = await CreateProductionTransactionInsertDTO(httpClient);
				var productionTransactionResult = await _productionTransactionService.InsertProductionTransaction(httpClient, productionTransactionDtoResult, _httpClientService.FirmNumber);

				if(productionTransactionResult.IsSuccess)
				{
					resultModel.IsSuccess = true;
					resultModel.PageTitle = "Fason Mal Kabul İşlemi";
					resultModel.Code = $@"Sarf Fişi: {consumableTransactionResult.Data.Code}\nÜretimden Giriş Fişi: {productionTransactionResult.Data.Code}";
					resultModel.PageCountToBack = 6;
					resultModel.Message = "Başarılı";

					await ClearFormAsync();
					await ClearDataAsync();

					await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});

					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

				}
				else
				{
					resultModel.IsSuccess = false;
					resultModel.PageCountToBack = 6;
					resultModel.Message = @$"Sarf Fişi Başarılı: {consumableTransactionResult.Data.Code}\nÜretimden Giriş Fişi Başarısız";
					resultModel.PageTitle = "Fason Kabul İşlemi";
					resultModel.ErrorMessage = productionTransactionResult.Message;

					await ClearFormAsync();
					await ClearDataAsync();

					await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});

					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();
				}

			}
			else
			{
				resultModel.IsSuccess = false;
				resultModel.PageCountToBack = 1;
				resultModel.Message = "Başarısız";
				resultModel.PageTitle = "Fason Kabul İşlemi";
				resultModel.ErrorMessage = consumableTransactionResult.Message;


				await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = resultModel
				});

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

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

	private async Task<ProductionTransactionInsert> CreateProductionTransactionInsertDTO(HttpClient httpClient)
	{
		var productionTransactionInsertDto = new ProductionTransactionInsert
		{
			SpeCode = SpecialCode,
			CurrentCode = string.Empty,
			Code = string.Empty,
			DocTrackingNumber = DocumentTrackingNumber,
			DoCode = DocumentNumber,
			Description = Description,
			FirmNumber = _httpClientService.FirmNumber,
			TransactionDate = FicheDate,
			WarehouseNumber = InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
		};

		var productionTransactionLineDto = new ProductionTransactionLineDto
		{
			ProductCode = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode : InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode,
			VariantCode = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode : string.Empty,
			WarehouseNumber = InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
			SubUnitsetCode = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.SubUnitsetCode,
		    Quantity = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity,
			ConversionFactor = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ConversionFactor,
			OtherConversionFactor = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.OtherConversionFactor,
		};

		foreach(var detail in InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.Details)
		{
			var seriLotTransactionDto = new SeriLotTransactionDto
			{
				StockLocationCode = detail.LocationCode,
				Quantity = detail.Quantity,
				ConversionFactor = detail.Quantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ConversionFactor,
				OtherConversionFactor = detail.Quantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.OtherConversionFactor,
				DestinationStockLocationCode = string.Empty,
			};

			productionTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
		}

		productionTransactionInsertDto.Lines.Add(productionTransactionLineDto);

		return productionTransactionInsertDto;
	}


	private async Task<ConsumableTransactionInsert> CreateConsumableTransactionInsertDTO(HttpClient httpClient, ObservableCollection<InputOutsourceTransferSubProductModel> subProducts)
	{
		var consumableTransactionDto = new ConsumableTransactionInsert
		{
			Code = string.Empty,
			CurrentCode = string.Empty,
			Description = Description,
			DoCode = DocumentNumber,
			DocTrackingNumber = DocumentTrackingNumber,
			TransactionDate = FicheDate,
			FirmNumber = _httpClientService.FirmNumber,
			SpeCode = SpecialCode,
			WarehouseNumber = InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number
		};

        foreach (var subProduct in subProducts)
        {
			var tempSubProductQuantity = subProduct.OutputQuantity;
			var consumableTransactionLineDto = new ConsumableTransactionLineDto
			{
				ProductCode = subProduct.IsVariant ? subProduct.ProductCode : subProduct.ProductCode,
				VariantCode = subProduct.IsVariant ? subProduct.ProductCode : string.Empty,
				WarehouseNumber = subProduct.WarehouseNumber,
				Quantity = subProduct.OutputQuantity,
				ConversionFactor = subProduct.ConversionFactor * subProduct.OutputQuantity,
				OtherConversionFactor = subProduct.OtherConversionFactor * subProduct.OutputQuantity,
				SubUnitsetCode = subProduct.SubUnitsetCode,
			};

            foreach (var detail in subProduct.Details)
            {
				var tempDetailQuantity = detail.Quantity;
				await LoadLocationTransactionAsync(httpClient, subProduct, detail);
				LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

				foreach(var locationTransaction in LocationTransactions)
				{
					var tempLocationTransactionRemainingQuantity = locationTransaction.RemainingQuantity;
					while(tempLocationTransactionRemainingQuantity > 0 && tempSubProductQuantity > 0 && tempDetailQuantity > 0)
					{
						var seriLotTransactionDto = new SeriLotTransactionDto
						{
							StockLocationCode = detail.LocationCode,
							InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
							OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
							Quantity = tempDetailQuantity > tempLocationTransactionRemainingQuantity ? tempLocationTransactionRemainingQuantity : tempDetailQuantity,
							SubUnitsetCode = subProduct.SubUnitsetCode,
							DestinationStockLocationCode = string.Empty,
							ConversionFactor = tempDetailQuantity > tempLocationTransactionRemainingQuantity ? tempLocationTransactionRemainingQuantity : tempDetailQuantity * subProduct.ConversionFactor,
							OtherConversionFactor = tempDetailQuantity > tempLocationTransactionRemainingQuantity ? tempLocationTransactionRemainingQuantity : tempDetailQuantity * subProduct.OtherConversionFactor,
						};

						consumableTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
						tempSubProductQuantity -= (double)seriLotTransactionDto.Quantity;
						tempDetailQuantity -= (double)seriLotTransactionDto.Quantity;
						tempLocationTransactionRemainingQuantity -= (double)seriLotTransactionDto.Quantity;
					}
				}
			}

			consumableTransactionDto.Lines.Add(consumableTransactionLineDto);
		}

		return consumableTransactionDto;
    }

	private async Task LoadLocationTransactionAsync(HttpClient httpClient, InputOutsourceTransferSubProductModel product, InputOutsourceTransferSubProductDetailModel detail)
	{
		try
		{
			LocationTransactions.Clear();
			var result = await _locationTransactionService.GetInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: product.ProductReferenceId,
				warehouseNumber: product.WarehouseNumber,
				skip: 0,
				take: 99999,
				search: "",
				locationRef: detail.LocationReferenceId
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
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Form verileriniz silinecektir. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			await ClearFormAsync();
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

	private async Task ClearDataAsync()
	{
		try
		{
			var warehouseListViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2WarehouseListViewModel>();
			var supplierListViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2SupplierListViewModel>();
			var productListViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2ProductListViewModel>();
			var locationListViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2MainProductLocationListViewModel>();
			var basketViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2BasketViewModel>();

			if(warehouseListViewModel is not null && warehouseListViewModel.SelectedWarehouseModel is not null)
			{
				warehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
				warehouseListViewModel.SelectedWarehouseModel = null;
			}

			if(supplierListViewModel is not null && supplierListViewModel.SelectedOutsourceModel is not null)
			{
				supplierListViewModel.SelectedOutsourceModel.IsSelected = false;
				supplierListViewModel.SelectedOutsourceModel = null;
			}

			if(productListViewModel is not null && productListViewModel.SelectedOutsourceProductModel is not null)
			{
				productListViewModel.SelectedOutsourceProductModel.IsSelected = false;
				productListViewModel.SelectedOutsourceProductModel = null;
			}

			if(locationListViewModel is not null)
			{
				locationListViewModel.SelectedItems.Clear();
			}

			if(basketViewModel is not null && basketViewModel.InputOutsourceTransferV2BasketModel is not null)
			{
				basketViewModel.InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.Details.Clear();
				basketViewModel.InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel = null;

				foreach (var item in basketViewModel.InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
				{
					item.Details.Clear();
				}

				basketViewModel.InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts.Clear();

				basketViewModel.SelectedSubProductModel = null;
				basketViewModel.Locations.Clear();
				
				basketViewModel.InputOutsourceTransferV2BasketModel = null;
			}
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task ClearFormAsync()
	{
		try
		{
			FicheDate = DateTime.Now;
			DocumentNumber = string.Empty;
			DocumentTrackingNumber = string.Empty;
			SpecialCode = string.Empty;
			Description = string.Empty;
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
