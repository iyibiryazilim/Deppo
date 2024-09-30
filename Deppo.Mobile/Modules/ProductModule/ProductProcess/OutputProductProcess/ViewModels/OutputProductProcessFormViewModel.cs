using Android.Net.Wifi;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.OutCountingTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.WastageTransaction;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class OutputProductProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IConsumableTransactionService _consumableTransactionService;
    private readonly IWastageTransactionService _wastageTransactionService;
    private readonly IOutCountingTransactionService _outCountingTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
	OutputProductProcessType outputProductProcessType;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	ObservableCollection<OutputProductBasketModel> items = null!;


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


    public OutputProductProcessFormViewModel(IHttpClientService httpClientService, IConsumableTransactionService consumableTransactionService, IUserDialogs userDialogs, IWastageTransactionService wastageTransactionService, IOutCountingTransactionService outCountingTransactionService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _consumableTransactionService = consumableTransactionService;
        _wastageTransactionService = wastageTransactionService;
        _outCountingTransactionService = outCountingTransactionService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;

        Items = new();
        SaveCommand = new Command(async () => await SaveAsync());
        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        BackCommand = new Command(async () => await BackAsync());
    }
    public Page CurrentPage { get; set; }

    public Command SaveCommand { get; }
	public Command LoadPageCommand { get; }
	public Command ShowBasketItemCommand { get; }
    public Command BackCommand { get; }

	private async Task LoadPageAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			Title = GetEnumDescription(OutputProductProcessType);
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

            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);
         
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            switch(OutputProductProcessType)
            {
                case OutputProductProcessType.ConsumableProcess:
                    await ConsumableTransactionInsertAsync(httpClient);
                    break;
                case OutputProductProcessType.WasteProcess:
                    await WastageTransactionInsertAsync(httpClient);
                    break;
                case OutputProductProcessType.UnderCountProcess:
                    await OutCountingTransactionInsert(httpClient);
                    break;
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

    private async Task ConsumableTransactionInsertAsync(HttpClient httpClient)
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
            WarehouseNumber = WarehouseModel.Number,

        };

        foreach (var item in Items)
        {
            var consumableTransactionLineDto = new ConsumableTransactionLineDto
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

                consumableTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
            }

            consumableTransactionDto.Lines.Add(consumableTransactionLineDto);
        }

        var result = await _consumableTransactionService.InsertConsumableTransaction(httpClient, consumableTransactionDto, _httpClientService.FirmNumber);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 4;

            await ClearFormAsync();

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
            resultModel.PageCountToBack = 1;
            await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
            {
                [nameof(ResultModel)] = resultModel
            });
        }
    }

    private async Task WastageTransactionInsertAsync(HttpClient httpClient)
    {
        var wastageTransactionDto = new WastageTransactionInsert
        {
            Code = "",
            CurrentCode = "",
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
            var wastageTransactionLineDto = new WastageTransactionLineDto
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

                wastageTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
            }

            wastageTransactionDto.Lines.Add(wastageTransactionLineDto);
        }

        var result = await _wastageTransactionService.InsertWastageTransaction(httpClient, wastageTransactionDto, _httpClientService.FirmNumber);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 4;

            await ClearFormAsync();
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
            resultModel.PageCountToBack = 1;
            await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
            {
                [nameof(ResultModel)] = resultModel
            });
        }
    }

    private async Task OutCountingTransactionInsert(HttpClient httpClient)
    {
        var outCountingTransactionDto = new OutCountingTransactionInsert
        {
            Code = "",
            CurrentCode = "",
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
            var outCountingTransactionLineDto = new OutCountingTransactionLineDto
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

                outCountingTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
            }

            outCountingTransactionDto.Lines.Add(outCountingTransactionLineDto);
        }

        var result = await _outCountingTransactionService.InsertOutCountingTransaction(httpClient, outCountingTransactionDto, _httpClientService.FirmNumber);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 4;

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await ClearFormAsync();
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

			var confirm = await _userDialogs.ConfirmAsync("Form verileriniz silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
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
			DocumentNumber = string.Empty;
			TransactionDate = DateTime.Now;
			Description = string.Empty;
			DocumentTrackingNumber = string.Empty;
			SpecialCode = string.Empty;

            var basketViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
            var productListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessProductListViewModel>();
            var warehouseListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessWarehouseListViewModel>();

            foreach (var item in productListViewModel.Items)
                item.IsSelected = false;
            
            productListViewModel.Items.Clear();
            productListViewModel.SelectedProducts.Clear();

            foreach (var item in basketViewModel.Items)
                item.Details.Clear();
           
            basketViewModel.Items.Clear();

            foreach (var item in warehouseListViewModel.Items)
                item.IsSelected = false;
            
            warehouseListViewModel.SelectedWarehouseModel = null;
        }
        catch (Exception ex)
        {
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
    }
}
