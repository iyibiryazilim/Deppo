using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
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

	public InputOutsourceTransferV2FormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_productionTransactionService = productionTransactionService;
		_consumableTransactionService = consumableTransactionService;

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
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Kaydetmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			_userDialogs.Loading("Kaydediliyor...");
			await Task.Delay(500);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			// TODO: Insert işlemi
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

	private async Task<DataResult<ResponseModel>> ProductionTransactionInsertAsync(HttpClient httpClient)
	{
		var productionTransactionDto = new ProductionTransactionInsert
		{
			SpeCode = SpecialCode,
			CurrentCode = "",
			Code = string.Empty,
			DocTrackingNumber = DocumentTrackingNumber,
			DoCode = DocumentNumber,
			TransactionDate = FicheDate,
			FirmNumber = _httpClientService.FirmNumber,
			WarehouseNumber = InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
			Description = Description,
		};

		var productionTransactionLineDto = new ProductionTransactionLineDto
		{
			ProductCode = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant ?  InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode : InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode,
			VariantCode = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.IsVariant ? InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode
	: string.Empty,
			WarehouseNumber = InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number,
			Quantity = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity,
			SubUnitsetCode = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.SubUnitsetCode,
			ConversionFactor = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity,
			OtherConversionFactor = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity,
		};

        foreach (var detail in InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.Details)
        {
			var seriLotTransactionDto = new SeriLotTransactionDto
			{
				StockLocationCode = detail.LocationCode,
				Quantity = detail.Quantity,
				ConversionFactor = detail.Quantity,
				OtherConversionFactor = detail.Quantity,
				DestinationStockLocationCode = string.Empty,
			};

			productionTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
        }

		productionTransactionDto.Lines.Add(productionTransactionLineDto);

		var result = await _productionTransactionService.InsertProductionTransaction(httpClient, productionTransactionDto, _httpClientService.FirmNumber);

		return result;
    }


	private async Task ConsumableTransactionInsert(HttpClient httpClient, ObservableCollection<InputOutsourceTransferSubProductModel> subProducts)
	{
		var consumableTrasactionDto = new ConsumableTransactionInsert
		{
			Code = string.Empty,
			CurrentCode = string.Empty,
			Description = Description,
			DoCode = DocumentNumber,
			DocTrackingNumber = DocumentTrackingNumber,
			TransactionDate = FicheDate,
			FirmNumber = _httpClientService.FirmNumber,
			SpeCode = SpecialCode,
			// TODO: SET Related WarehouseNumber
		};

        foreach (var subProduct in subProducts)
        {
			var consumableTransactionLineDto = new ConsumableTransactionLineDto
			{
				ProductCode = subProduct.IsVariant ? subProduct.ProductCode : subProduct.ProductCode,
				VariantCode = subProduct.IsVariant ? subProduct.ProductCode : string.Empty,
				WarehouseNumber = subProduct.WarehouseNumber,
				Quantity = subProduct.OutputQuantity,
				ConversionFactor = subProduct.OutputQuantity,
				OtherConversionFactor = subProduct.OutputQuantity,
				SubUnitsetCode = subProduct.SubUnitsetCode,
				UnitPrice = 0,
				VatRate = 0,
			};

            foreach (var detail in subProduct.Details)
            {
				var seriLotTransactionDto = new SeriLotTransactionDto
				{
					ConversionFactor = detail.Quantity,
					OtherConversionFactor = detail.Quantity,
					StockLocationCode = detail.LocationCode,
					Quantity = detail.Quantity,
					SubUnitsetCode = subProduct.SubUnitsetCode,
					DestinationStockLocationCode = string.Empty, // TODO: set real value
				};

				consumableTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
			}

			consumableTrasactionDto.Lines.Add(consumableTransactionLineDto);
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
