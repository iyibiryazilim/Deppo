using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.InCountingTransaction;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(InputProductProcessType), queryId: nameof(InputProductProcessType))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class InputProductProcessFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductionTransactionService _productionTransactionService;
    private readonly IInCountingTransactionService _inCountingTransactionService;
    private readonly IUserDialogs _userDialogs;


    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private InputProductProcessType inputProductProcessType;

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

    [ObservableProperty]
    ObservableCollection<InputProductBasketModel> items = null!;


	public InputProductProcessFormViewModel(IHttpClientService httpClientService, IProductionTransactionService productionTransactionService, IUserDialogs userDialogs, IInCountingTransactionService inCountingTransactionService)
	{
		_httpClientService = httpClientService;
		_productionTransactionService = productionTransactionService;
		_inCountingTransactionService = inCountingTransactionService;
		_userDialogs = userDialogs;
		Items = new();

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		SaveCommand = new Command(async () => await SaveAsync());
	}

	public Page CurrentPage { get; set; }

    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }

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

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

            Title = GetEnumDescription(InputProductProcessType);
            CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;


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

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Fiş oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			_userDialogs.Loading("İşlem tamamlanıyor");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			switch(InputProductProcessType)
			{
				case InputProductProcessType.ProductionInputProcess:
					await ProductionTransactionInsertAsync(httpClient);
					break;
				case InputProductProcessType.OverCountProcess:
					await InCountingTransactionInsertAsync(httpClient);
					break;
			}

			if(_userDialogs.IsHudShowing)
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

    private async Task ProductionTransactionInsertAsync(HttpClient httpClient)
    {
        var productionTransactionDto = new ProductionTransactionInsert
		{
			SpeCode = SpecialCode,
			CurrentCode = string.Empty,
			Code = string.Empty,
			DocTrackingNumber = DocumentTrackingNumber,
			DoCode = DocumentNumber,
			TransactionDate = FicheDate,
			FirmNumber = _httpClientService.FirmNumber,
			WarehouseNumber = WarehouseModel.Number,
			Description = Description,
		};

		foreach (var item in Items)
		{
			var productionTransactionLineDto = new ProductionTransactionLineDto
			{
				ProductCode = item.ItemCode,
				WarehouseNumber = WarehouseModel.Number,
				Quantity = item.Quantity,
				ConversionFactor = 1,
				OtherConversionFactor = 1,
				SubUnitsetCode = item.SubUnitsetCode,
			};

			foreach (var detail in item.Details)
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

		ResultModel resultModel = new();
		if (result.IsSuccess)
		{
			resultModel.Message = "Başarılı";
			resultModel.Code = result.Data.Code;
			resultModel.PageTitle = Title;
            resultModel.IsSuccess = true;
			resultModel.PageCountToBack = 4;

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
			resultModel.PageTitle = Title;
            resultModel.ErrorMessage = result.Message;
            resultModel.IsSuccess = false;
			resultModel.PageCountToBack = 1;
			await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
			{
				[nameof(ResultModel)] = resultModel
			});
		}
	}

    private async Task InCountingTransactionInsertAsync(HttpClient httpClient)
    {
        var inCountingTransactionDto = new InCountingTransactionInsert
        {
			SpeCode = SpecialCode,
			CurrentCode = string.Empty,
			Code = string.Empty,
			DocTrackingNumber = DocumentTrackingNumber,
			DoCode = DocumentNumber,
			TransactionDate = FicheDate,
			FirmNumber = _httpClientService.FirmNumber,
			WarehouseNumber = WarehouseModel.Number,
			Description = Description,
		};

		foreach (var item in Items)
		{
			var inCountingTransactionLineDto = new InCountingTransactionLineDto
			{
				ProductCode = item.ItemCode,
				WarehouseNumber = WarehouseModel.Number,
				Quantity = item.Quantity,
				ConversionFactor = 1,
				OtherConversionFactor = 1,
				SubUnitsetCode = item.SubUnitsetCode,
			};

			foreach (var detail in item.Details)
			{
				var seriLotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = detail.LocationCode,
					Quantity = detail.Quantity,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
					DestinationStockLocationCode = string.Empty,
				};

				inCountingTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
			}

			inCountingTransactionDto.Lines.Add(inCountingTransactionLineDto);
		}

        var result = await _inCountingTransactionService.InsertInCountingTransaction(httpClient, inCountingTransactionDto, _httpClientService.FirmNumber);

		ResultModel resultModel = new();
		if (result.IsSuccess)
		{
			resultModel.Message = "Başarılı";
			resultModel.Code = result.Data.Code;
			resultModel.PageTitle = Title;
			resultModel.IsSuccess = true;
			resultModel.PageCountToBack = 4;

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
			resultModel.PageTitle = Title;
			resultModel.ErrorMessage = result.Message;
			resultModel.IsSuccess = false;
			resultModel.PageCountToBack = 1;
			await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
			{
				[nameof(ResultModel)] = resultModel
			});
		}
	}
}
