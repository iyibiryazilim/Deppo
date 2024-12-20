using Android.Net.Wifi;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.ConsumableTransaction;
using Deppo.Core.DTOs.OutCountingTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.WastageTransaction;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;
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
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

    [ObservableProperty]
    private OutputProductProcessType outputProductProcessType;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private ObservableCollection<OutputProductBasketModel> items = null!;

    private ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    private string documentNumber = string.Empty;

    [ObservableProperty]
    private DateTime transactionDate = DateTime.Now;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    private string specialCode = string.Empty;

    public OutputProductProcessFormViewModel(IHttpClientService httpClientService, IConsumableTransactionService consumableTransactionService, IUserDialogs userDialogs, IWastageTransactionService wastageTransactionService, IOutCountingTransactionService outCountingTransactionService, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService, ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService, ITransactionAuditHelperService transactionAuditHelperService)
    {
        _httpClientService = httpClientService;
        _consumableTransactionService = consumableTransactionService;
        _wastageTransactionService = wastageTransactionService;
        _outCountingTransactionService = outCountingTransactionService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _locationTransactionService = locationTransactionService;
        _transactionAuditService = transactionAuditService;
        _httpClientSysService = httpClientSysService;
        _transactionAuditHelperService = transactionAuditHelperService;

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

    public async Task LoadLocationTransaction(OutputProductBasketModel outputProductBasketModel, OutputProductBasketDetailModel outputProductBasketDetailModel)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            LocationTransactions.Clear();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: outputProductBasketModel.IsVariant ? outputProductBasketModel.MainItemReferenceId : outputProductBasketModel.ItemReferenceId,
                variantReferenceId: outputProductBasketModel.IsVariant ? outputProductBasketModel.ItemReferenceId : 0,
                warehouseNumber: WarehouseModel.Number,
                locationRef: outputProductBasketDetailModel.LocationReferenceId,
                skip: 0,
                take: 999999,
                search: "",
                externalDb: _httpClientService.ExternalDatabase
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

            switch (OutputProductProcessType)
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
            var tempItemQuantity = item.Quantity;
			var consumableTransactionLineDto = new ConsumableTransactionLineDto
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
                var tempDetailQuantity = detail.Quantity;
				await LoadLocationTransaction(item, detail);
                LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                foreach (var locationTransaction in LocationTransactions)
                {
                    var tempLocationRemaningQuantity = locationTransaction.RemainingQuantity;
					while (tempLocationRemaningQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity > 0)
                    {
                        var serilotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                            OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                            Quantity = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity,
                            SubUnitsetCode = item.SubUnitsetCode,
                            DestinationStockLocationCode = string.Empty,
                            ConversionFactor = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity * item.ConversionFactor,
                            OtherConversionFactor = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity * item.OtherConversionFactor,
                        };

						tempLocationRemaningQuantity -= (double)serilotTransactionDto.Quantity;
                        consumableTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
						tempItemQuantity -= (double)serilotTransactionDto.Quantity;
                        tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
					}
                }
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

            try
            {
                await _transactionAuditHelperService.InsertProducTransactionAuditAsync(firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ioType: 3,
                    transactionType: 12,
                    transactionDate: TransactionDate,
                    transactionReferenceId: result.Data.ReferenceId,
                    transactionNumber: result.Data.Code,
                    documentNumber: DocumentNumber,
                    warehouseNumber: WarehouseModel.Number,
                    warehouseName: WarehouseModel.Name,
                    productReferenceCount: Items.Count
                    );
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }

            await ClearFormAsync();
            await ClearPageAsync();

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
            resultModel.PageTitle = Title;
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
			var tempItemQuantity = item.Quantity;
			var wastageTransactionLineDto = new WastageTransactionLineDto
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
                var tempDetailQuantity = detail.Quantity;
				await LoadLocationTransaction(item, detail);
                LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                foreach (var locationTransaction in LocationTransactions)
                {
                    var tempLocationRemaningQuantity = locationTransaction.RemainingQuantity;
					while (tempLocationRemaningQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity > 0)
					{
                        var serilotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                            OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                            Quantity = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity,
                            SubUnitsetCode = item.SubUnitsetCode,
                            DestinationStockLocationCode = string.Empty,
                            ConversionFactor = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity * item.ConversionFactor,
                            OtherConversionFactor = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity * item.OtherConversionFactor,
                        };

						tempLocationRemaningQuantity -= (double)serilotTransactionDto.Quantity;
                        wastageTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
						tempItemQuantity -= (double)serilotTransactionDto.Quantity;
						tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
					}
                }
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

			try
			{
				await _transactionAuditHelperService.InsertProducTransactionAuditAsync(firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					ioType: 3,
					transactionType: 11,
					transactionDate: TransactionDate,
					transactionReferenceId: result.Data.ReferenceId,
					transactionNumber: result.Data.Code,
					documentNumber: DocumentNumber,
					warehouseNumber: WarehouseModel.Number,
					warehouseName: WarehouseModel.Name,
					productReferenceCount: Items.Count

					);
			}
			catch (Exception ex)
			{
				_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			}

			await ClearFormAsync();
			await ClearPageAsync();

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
			resultModel.PageTitle = Title;
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
            var tempItemQuantity = item.Quantity;
			var outCountingTransactionLineDto = new OutCountingTransactionLineDto
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
                var tempDetailQuantity = detail.Quantity;
				await LoadLocationTransaction(item, detail);
                LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                foreach (var locationTransaction in LocationTransactions)
                {
                    var tempLocationRemaningQuantity = locationTransaction.RemainingQuantity;
					while (tempLocationRemaningQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity > 0)
					{
                        var serilotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                            OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                            Quantity = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity,
                            SubUnitsetCode = item.SubUnitsetCode,
                            DestinationStockLocationCode = string.Empty,
                            ConversionFactor = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity * item.ConversionFactor,
                            OtherConversionFactor = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity * item.OtherConversionFactor,
                        };

						tempLocationRemaningQuantity -= (double)serilotTransactionDto.Quantity;
                        outCountingTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
						tempItemQuantity -= (double)serilotTransactionDto.Quantity;
						tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
					}
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
            resultModel.PageCountToBack = 4;

            try
            {
                await _transactionAuditHelperService.InsertProducTransactionAuditAsync(firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ioType: 3,
                    transactionType: 51,
                    transactionDate: TransactionDate,
                    transactionReferenceId: result.Data.ReferenceId,
                    transactionNumber: result.Data.Code,
                    documentNumber: DocumentNumber,
                    warehouseNumber: WarehouseModel.Number,
                    warehouseName: WarehouseModel.Name,
                    productReferenceCount: Items.Count

                    );
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }


			await ClearFormAsync();
			await ClearPageAsync();

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
            resultModel.PageTitle = Title;
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
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task ClearPageAsync()
    {
        try
        {
            var warehouseListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessWarehouseListViewModel>();
            var basketViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
            var productListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessProductListViewModel>();

            if(warehouseListViewModel is not null && warehouseListViewModel.SelectedWarehouseModel is not null)
            {
                warehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
                warehouseListViewModel.SelectedWarehouseModel = null;

				foreach (var item in warehouseListViewModel.Items)
					item.IsSelected = false;
			}

            if(basketViewModel is not null)
            {
                basketViewModel.Items.ForEach(x => x.Details.Clear());
				basketViewModel.Items.Clear();
                basketViewModel.SelectedItems.Clear();
            }

            if(productListViewModel is not null)
            {
				foreach (var item in productListViewModel.Items)
					item.IsSelected = false;

				productListViewModel.Items.Clear();
				productListViewModel.SelectedProducts.Clear();
                productListViewModel.SelectedItems.Clear(); 
			}
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
}