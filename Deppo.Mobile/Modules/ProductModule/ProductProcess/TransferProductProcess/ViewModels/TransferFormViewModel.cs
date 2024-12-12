using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Core.Internal;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

[QueryProperty(name: nameof(TransferBasketModel), queryId: nameof(TransferBasketModel))]
public partial class TransferFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ITransferTransactionService _transferTransactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserDialogs _userDialogs;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

    [ObservableProperty]
    private TransferBasketModel transferBasketModel = null!;

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

    public TransferFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ITransferTransactionService transferTransactionService, IServiceProvider serviceProvider, ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService, ITransactionAuditHelperService transactionAuditHelperService)
    {
        _httpClientService = httpClientService;
        _transferTransactionService = transferTransactionService;
        _serviceProvider = serviceProvider;
        _userDialogs = userDialogs;
        _transactionAuditService = transactionAuditService;
        _httpClientSysService = httpClientSysService;
        _transactionAuditHelperService = transactionAuditHelperService;

        Title = "Ambar Transfer Formu";
        //LoadPageCommand = new Command(async () => await LoadPageAsync());
        //ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        BackCommand = new Command(async () => await BackAsync());
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
			var confirm = await _userDialogs.ConfirmAsync("Kaydetmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

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

            foreach (var item in TransferBasketModel.OutProducts)
            {
                ObservableCollection<LocationModel> Locations = new();

                foreach (var inProduct in TransferBasketModel.InProducts)
                {
                    if (item.ReferenceId == inProduct.ReferenceId)
                    {
                        foreach (var location in inProduct.Locations)
                        {
                            Locations.Add(location);
                        }
                    }
                }

                var transferTransactionLineDto = new TransferTransactionLineDto
                {
                    ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                    VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
                    WarehouseNumber = TransferBasketModel.OutWarehouse.Number,
                    DestinationWarehouseNumber = TransferBasketModel.InWarehouse.Number,
                    Quantity = item.OutputQuantity,
                    ConversionFactor = 1,
                    OtherConversionFactor = 1,
                    SubUnitsetCode = item.SubUnitsetCode,
                };

                foreach (var locationTransaction in item.LocationTransactions)
                {
                    foreach (var location in Locations)
                    {
                        if (location.InputQuantity <= 0)
                            continue;

                        var seriLotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = locationTransaction.LocationCode,
                            Quantity = location.InputQuantity < locationTransaction.Quantity
                                       ? location.InputQuantity
                                       : locationTransaction.Quantity,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                            InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                            OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                            DestinationStockLocationCode = location.Code
                        };

                        locationTransaction.Quantity -= (double)seriLotTransactionDto.Quantity;
                        location.InputQuantity -= (double)seriLotTransactionDto.Quantity;

                        transferTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);

                        if (locationTransaction.Quantity <= 0)
                            break;
                    }
                }

                transferTransactionInsertDto.Lines.Add(transferTransactionLineDto);
            }

            var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);
            ResultModel resultModel = new();

            if (result.IsSuccess)
            {
                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = "Ambar Transferi";
                resultModel.PageCountToBack = 7;

                try
                {
                    await _transactionAuditHelperService.InsertProducTransactionAuditAsync(firmNumber: _httpClientService.FirmNumber,
                        periodNumber: _httpClientService.PeriodNumber,
                        ioType: 2,
                        transactionType: 25,
                        transactionDate: FicheDate,
                        transactionReferenceId: result.Data.ReferenceId,
                        transactionNumber: result.Data.Code,
                        documentNumber: DocumentNumber,
                        warehouseNumber: TransferBasketModel.OutWarehouse.Number,
                        warehouseName: TransferBasketModel.OutWarehouse.Name,
                        productReferenceCount: default
                      );
                }
                catch (Exception ex)
                {
                    _userDialogs.Alert(ex.Message, "Hata", "Tamam");
                }

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
                resultModel.Message = "Başarısız";
                resultModel.PageTitle = "Ambar Transferi";
                resultModel.PageCountToBack = 1;
                resultModel.ErrorMessage = result.Message;

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
			var outWarehouseViewModel = _serviceProvider.GetRequiredService<TransferOutWarehouseListViewModel>();
			var inWarehouseViewModel = _serviceProvider.GetRequiredService<TransferInWarehouseViewModel>();
			var inBasketViewModel = _serviceProvider.GetRequiredService<TransferInBasketViewModel>();
			var outBasketViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();


			if (outWarehouseViewModel is not null && outWarehouseViewModel.SelectedWarehouseModel is not null)
			{
				outWarehouseViewModel.SelectedWarehouseModel.IsSelected = false;
				outWarehouseViewModel.SelectedWarehouseModel = null;
			}

			if (inWarehouseViewModel is not null && inWarehouseViewModel.SelectedWarehouseModel is not null)
			{
				inWarehouseViewModel.SelectedWarehouseModel.IsSelected = false;
				inWarehouseViewModel.SelectedWarehouseModel = null;
			}

			if (outBasketViewModel is not null && outBasketViewModel.TransferBasketModel is not null)
			{
				foreach (var item in outBasketViewModel.TransferBasketModel.OutProducts)
				{
					item.LocationTransactions.Clear();
				}

				outBasketViewModel.TransferBasketModel.OutProducts.Clear();
				outBasketViewModel.TransferBasketModel.OutWarehouse = null;
			}

			if (inBasketViewModel is not null && inBasketViewModel.TransferBasketModel is not null)
			{
				foreach (var item in inBasketViewModel.TransferBasketModel.InProducts)
				{
					item.Locations.Clear();
				}

				inBasketViewModel.TransferBasketModel.InProducts.Clear();
				inBasketViewModel.TransferBasketModel.InWarehouse = null;
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
			DocumentNumber = string.Empty;
			DocumentTrackingNumber = string.Empty;
			SpecialCode = string.Empty;
			Description = string.Empty;
			FicheDate = DateTime.Now;
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

            var confirm = await _userDialogs.ConfirmAsync("Form verileriniz silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
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
}