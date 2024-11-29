using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.InCountingTransaction;
using Deppo.Core.DTOs.OutCountingTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.WastageTransaction;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(WarehouseCountingWarehouseModel), nameof(WarehouseCountingWarehouseModel))]
[QueryProperty(nameof(WarehouseCountingBasketModel), nameof(WarehouseCountingBasketModel))]
public partial class WarehouseCountingFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IInCountingTransactionService _inCountingTransactionService;
    private readonly IOutCountingTransactionService _outCountingTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;



    [ObservableProperty]
    LocationModel? locationModel;

    [ObservableProperty]
    WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;

    [ObservableProperty]
    ObservableCollection<WarehouseCountingBasketModel> warehouseCountingBasketModel = null!;

    [ObservableProperty]
    ObservableCollection<WarehouseCountingBasketModel> inputItems = new();

    [ObservableProperty]
    ObservableCollection<WarehouseCountingBasketModel> outputItems = new();


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


	public WarehouseCountingFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IOutCountingTransactionService outCountingTransactionService, IInCountingTransactionService inCountingTransactionService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_inCountingTransactionService = inCountingTransactionService;
		_outCountingTransactionService = outCountingTransactionService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		Title = "Ambar Sayım Formu";

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

            var confirm = await _userDialogs.ConfirmAsync("Fiş oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!confirm)
                return;

            CategorizeItems();

            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            if (OutputItems.Any())
            {
                await ProcessOutItems(httpClient);
                
            }
            else
            {
                await ProcessInputItems(httpClient);
            }

            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CategorizeItems()
    {
        InputItems.Clear();
        OutputItems.Clear();

        foreach (var item in WarehouseCountingBasketModel)
        {
            if (item.OutputQuantity > item.StockQuantity)
                InputItems.Add(item);
            else
                OutputItems.Add(item);
        }
    }

    private async Task ProcessOutItems(HttpClient httpClient)
    {
        var result = await OutCountingTransactionInsert(httpClient);
        if (!result.IsSuccess)
        {
            await NavigateToFailurePage(result);
        }
        else
        {
            string codeString = result.Code;

            if (InputItems.Any())
            {
                var inputResult = await InCountingTransactionInsert(httpClient);
                if (inputResult.IsSuccess)
                {
                    codeString += " - " + inputResult.Code;
                    result.Code = codeString;
                    await NavigateToSuccessPage(result);
                }
            }
            else
            {
                await NavigateToSuccessPage(result);
            }
        }
    }

    private async Task ProcessInputItems(HttpClient httpClient)
    {
        var inputResult = await InCountingTransactionInsert(httpClient);
        if (inputResult.IsSuccess)
        {
            await NavigateToSuccessPage(inputResult);
        }
    }

    private async Task NavigateToSuccessPage(ResultModel result)
    {

		await ClearFormAsync();
		await ClearDataAsync();

		if (_userDialogs.IsHudShowing)
			_userDialogs.HideHud();

		await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
        {
            [nameof(ResultModel)] = result
        });
    }

    private async Task NavigateToFailurePage(ResultModel result)
    {
		if (_userDialogs.IsHudShowing)
			_userDialogs.HideHud();

		await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
        {
            [nameof(ResultModel)] = result
        });
    }

    private void HandleError(Exception ex)
    {
        if (_userDialogs.IsHudShowing)
            _userDialogs.HideHud();

        _userDialogs.Alert(ex.Message, "Hata", "Tamam");
    }


    private async Task<ResultModel> OutCountingTransactionInsert(HttpClient httpClient)
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
            WarehouseNumber = WarehouseCountingWarehouseModel.Number,

        };

        foreach (var item in OutputItems)
        {
            var outCountingTransactionLineDto = new OutCountingTransactionLineDto
            {
                ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
                WarehouseNumber = WarehouseCountingWarehouseModel.Number,
                Quantity = item.StockQuantity - item.OutputQuantity,
                ConversionFactor = item.StockQuantity - item.OutputQuantity,
                OtherConversionFactor = item.StockQuantity - item.OutputQuantity,
                SubUnitsetCode = item.SubUnitsetCode,
            };

            if (item.LocationTransactions.Count > 0)
            {
                foreach (var detail in item.LocationTransactions)
                {
                    var serilotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        InProductTransactionLineReferenceId = detail.TransactionReferenceId,
                        OutProductTransactionLineReferenceId = detail.ReferenceId,
                        Quantity = detail.OutputQuantity,
                        SubUnitsetCode = item.SubUnitsetCode,
                        DestinationStockLocationCode = string.Empty,
                        ConversionFactor = detail.OutputQuantity,
                        OtherConversionFactor = detail.OutputQuantity,
                    };

                    outCountingTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                }
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
            resultModel.IsSuccess = true;
            resultModel.PageCountToBack = LocationModel == null ? 4 : 5;

			return resultModel;
        }
        else
        {
            resultModel.Message = "Başarısız";
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 1;
            resultModel.IsSuccess = false;
            resultModel.ErrorMessage = result.Message;

			return resultModel;           
        }
    }

    private async Task<ResultModel> InCountingTransactionInsert(HttpClient httpClient)
    {
        var inCountingTransactionDto = new InCountingTransactionInsert
        {
            Code = "",
            CurrentCode = "",
            Description = Description,
            DoCode = DocumentNumber,
            DocTrackingNumber = DocumentTrackingNumber,
            TransactionDate = TransactionDate,
            FirmNumber = _httpClientService.FirmNumber,
            SpeCode = SpecialCode,
            WarehouseNumber = WarehouseCountingWarehouseModel.Number,

        };

        foreach (var item in InputItems)
        {
            var inCountingTransactionLineDto = new InCountingTransactionLineDto
            {
                ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
                WarehouseNumber = WarehouseCountingWarehouseModel.Number,
                Quantity = item.OutputQuantity - item.StockQuantity,
                ConversionFactor = item.OutputQuantity - item.StockQuantity,
                OtherConversionFactor = item.OutputQuantity - item.StockQuantity,
                SubUnitsetCode = item.SubUnitsetCode,
            };

            if (item.LocTracking == 1)
            {

                var serilotTransactionDto = new SeriLotTransactionDto
                {
                    StockLocationCode = LocationModel.Code,
                    InProductTransactionLineReferenceId = 0,
                    OutProductTransactionLineReferenceId = 0,
                    Quantity = item.OutputQuantity - item.StockQuantity,
                    SubUnitsetCode = item.SubUnitsetCode,
                    DestinationStockLocationCode = string.Empty,
                    ConversionFactor = item.OutputQuantity - item.StockQuantity,
                    OtherConversionFactor = item.OutputQuantity - item.StockQuantity,
                };
                inCountingTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
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
            resultModel.PageCountToBack = LocationModel == null ? 4 : 5;

            return resultModel;
        }
        else
        {
            resultModel.Message = "Başarısız";
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = 1;
            resultModel.IsSuccess = false;
            resultModel.ErrorMessage = result.Message;

            
            return resultModel;

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

            DocumentNumber = string.Empty;
            DocumentTrackingNumber = string.Empty;
            Description = string.Empty;
            SpecialCode = string.Empty;
            TransactionDate = DateTime.Now;

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
            var warehouseListViewModel = _serviceProvider.GetRequiredService<WarehouseCountingWarehouseListViewModel>();
            var locationViewModel = _serviceProvider.GetRequiredService<WarehouseCountingLocationListViewModel>();
            var basketListViewModel = _serviceProvider.GetRequiredService<WarehouseCountingBasketViewModel>();

            if (warehouseListViewModel is not null && warehouseListViewModel.SelectedWarehouse is not null)
            {
                warehouseListViewModel.SelectedWarehouse.IsSelected = false;
                warehouseListViewModel.SelectedWarehouse = null;
            }

			if (locationViewModel is not null && locationViewModel.SelectedLocation is not null)
			{
				locationViewModel.SelectedLocation.IsSelected = false;
				locationViewModel.SelectedLocation = null;
			}


			foreach (var item in basketListViewModel.Items.ToList())
            {
                item.LocationTransactions?.Clear();
            }

            basketListViewModel.Items.Clear();
            basketListViewModel.SelectedItems.Clear();
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }


    private async Task ClearFormAsync()
    {
        DocumentNumber = string.Empty;
        DocumentTrackingNumber = string.Empty;
        Description = string.Empty;
        SpecialCode = string.Empty;
        TransactionDate = DateTime.Now;
    }
}
