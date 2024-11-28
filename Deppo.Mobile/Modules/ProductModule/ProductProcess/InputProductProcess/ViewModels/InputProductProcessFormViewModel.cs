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
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Services;
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
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserDialogs _userDialogs;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private InputProductProcessType inputProductProcessType;

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

    [ObservableProperty]
    private ObservableCollection<InputProductBasketModel> items = null!;

    public InputProductProcessFormViewModel(IHttpClientService httpClientService, IProductionTransactionService productionTransactionService, IUserDialogs userDialogs, IInCountingTransactionService inCountingTransactionService, IServiceProvider serviceProvider, ITransactionAuditHelperService transactionAuditHelperService, IHttpClientSysService httpClientSysService)
    {
        _httpClientService = httpClientService;
        _productionTransactionService = productionTransactionService;
        _inCountingTransactionService = inCountingTransactionService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _transactionAuditHelperService = transactionAuditHelperService;
        _httpClientSysService = httpClientSysService;

        Items = new();

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        BackCommand = new Command(async () => await BackAsync());
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

            //await _userDialogs.ConfirmAsync("Fiş oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır").ContinueWith((x) =>
            //{
            //    x.
            //});

			var confirm = await _userDialogs.ConfirmAsync("Fiş oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!confirm)
                return;
            
			
			_userDialogs.Loading("İşlem tamamlanıyor");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            switch (InputProductProcessType)
            {
                case InputProductProcessType.ProductionInputProcess:
                    await ProductionTransactionInsertAsync(httpClient);
                    break;

                case InputProductProcessType.OverCountProcess:
                    await InCountingTransactionInsertAsync(httpClient);
                    break;
            }

            if (_userDialogs.IsHudShowing)
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
                ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                VariantCode = item.IsVariant ? item.ItemCode : "",
                WarehouseNumber = WarehouseModel.Number,
                Quantity = item.Quantity,
                ConversionFactor = item.ConversionFactor * item.Quantity,
                OtherConversionFactor = item.OtherConversionFactor * item.Quantity,
                SubUnitsetCode = item.SubUnitsetCode,
            };

            foreach (var detail in item.Details)
            {
                var seriLotTransactionDto = new SeriLotTransactionDto
                {
                    StockLocationCode = detail.LocationCode,
                    Quantity = detail.Quantity,
                    ConversionFactor = detail.Quantity * item.ConversionFactor,
                    OtherConversionFactor = detail.Quantity * item.OtherConversionFactor,
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

            await _transactionAuditHelperService.InsertProducTransactionAuditAsync(
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ioType: 1,
                    transactionType: 13,
                    transactionDate: FicheDate,
                    transactionReferenceId: result.Data.ReferenceId,
                    transactionNumber: result.Data.Code,
                    documentNumber: DocumentNumber,
                    warehouseNumber: WarehouseModel.Number,
                    warehouseName: WarehouseModel.Name,
                    productReferenceCount: Items.Count

            );

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            var basketViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
			foreach (var item in basketViewModel.Items)
			{
				item.Details.Clear();
			}
			basketViewModel.Items.Clear();

			var viewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketLocationListViewModel>();
			viewModel.SelectedItems.Clear();
			viewModel.Items.Clear();

			await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
            {
                [nameof(ResultModel)] = resultModel
            });

            await ClearFormAsync();
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
                ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                VariantCode = item.IsVariant ? item.ItemCode : "",
                WarehouseNumber = WarehouseModel.Number,
                Quantity = item.Quantity,
				ConversionFactor = item.ConversionFactor * item.Quantity,
				OtherConversionFactor = item.OtherConversionFactor * item.Quantity,
				SubUnitsetCode = item.SubUnitsetCode,
            };

            foreach (var detail in item.Details)
            {
                var seriLotTransactionDto = new SeriLotTransactionDto
                {
                    StockLocationCode = detail.LocationCode,
                    Quantity = detail.Quantity,
					ConversionFactor = detail.Quantity * item.ConversionFactor,
					OtherConversionFactor = detail.Quantity * item.OtherConversionFactor,
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

            await _transactionAuditHelperService.InsertProducTransactionAuditAsync(
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ioType: 1,
                    transactionType: 51,
                    transactionDate: FicheDate,
                    transactionReferenceId: result.Data.ReferenceId,
                    transactionNumber: result.Data.Code,
                    documentNumber: DocumentNumber,
                    warehouseNumber: WarehouseModel.Number,
                    warehouseName: WarehouseModel.Name,
                    productReferenceCount: Items.Count

            );

			await ClearFormAsync();

			var basketViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
			foreach (var item in basketViewModel.Items)
			{
				item.Details.Clear();
			}
			basketViewModel.Items.Clear();

			var viewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketLocationListViewModel>();
            viewModel.SelectedItems.Clear();
            viewModel.Items.Clear();




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
            FicheDate = DateTime.Now;

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

    private async Task ClearFormAsync()
    {
        DocumentNumber = string.Empty;
        DocumentTrackingNumber = string.Empty;
        Description = string.Empty;
        SpecialCode = string.Empty;
        FicheDate = DateTime.Now;
    }
}