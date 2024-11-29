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
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

[QueryProperty(nameof(ProductCountingWarehouseModel), nameof(ProductCountingWarehouseModel))]
[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(ProductCountingBasketModel), nameof(ProductCountingBasketModel))]
public partial class ProductCountingFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IInCountingTransactionService _inCountingTransactionService;
    private readonly IOutCountingTransactionService _outCountingTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;


    [ObservableProperty]
    private ProductCountingWarehouseModel productCountingWarehouseModel = null!;

    [ObservableProperty]
    private LocationModel? locationModel;

    [ObservableProperty]
    private ProductCountingBasketModel productCountingBasketModel = null!;



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


	public ProductCountingFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IOutCountingTransactionService outCountingTransactionService, IInCountingTransactionService inCountingTransactionService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_inCountingTransactionService = inCountingTransactionService;
		_outCountingTransactionService = outCountingTransactionService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		Title = "Ürüne Göre Ambar Sayım Formu";

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

            //Title = GetEnumDescription(OutputProductProcessType);
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

            if (ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity > 0)
                await InCountingTransactionInsert(httpClient);
            else
                await OutCountingTransactionInsert(httpClient);

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
            WarehouseNumber = ProductCountingWarehouseModel.Number,

        };


        var outCountingTransactionLineDto = new OutCountingTransactionLineDto
        {
            ProductCode = ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.MainItemCode : ProductCountingBasketModel.ItemCode,
            VariantCode = ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.ItemCode : string.Empty,
            WarehouseNumber = ProductCountingWarehouseModel.Number,
            Quantity = ProductCountingBasketModel.StockQuantity - ProductCountingBasketModel.OutputQuantity,

            ConversionFactor = ProductCountingBasketModel.ConversionFactor * ProductCountingBasketModel.StockQuantity,
            OtherConversionFactor = ProductCountingBasketModel.OtherConversionFactor * ProductCountingBasketModel.StockQuantity,
            SubUnitsetCode = ProductCountingBasketModel.SubUnitsetCode,
        };

        if (ProductCountingBasketModel.LocTracking == 1)
        {
            if (ProductCountingBasketModel.LocationTransactions.Count > 0)
            {
                foreach (var detail in ProductCountingBasketModel.LocationTransactions)
                {
                    var serilotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        InProductTransactionLineReferenceId = detail.TransactionReferenceId,
                        OutProductTransactionLineReferenceId = detail.ReferenceId,
                        Quantity = detail.OutputQuantity,
                        SubUnitsetCode = ProductCountingBasketModel.SubUnitsetCode,
                        DestinationStockLocationCode = string.Empty,
                        ConversionFactor = detail.OutputQuantity * ProductCountingBasketModel.ConversionFactor,
                        OtherConversionFactor = detail.OutputQuantity * ProductCountingBasketModel.OtherConversionFactor,
                    };

                    outCountingTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                }
            }

        }

        outCountingTransactionDto.Lines.Add(outCountingTransactionLineDto);


        var result = await _outCountingTransactionService.InsertOutCountingTransaction(httpClient, outCountingTransactionDto, _httpClientService.FirmNumber);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = LocationModel == null ? 5 : 6;

			await ClearFormAsync();
            await ClearDataAsync();

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
            resultModel.ErrorMessage = result.Message;
            await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
            {
                [nameof(ResultModel)] = resultModel
            });
        }
    }

    private async Task InCountingTransactionInsert(HttpClient httpClient)
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
            WarehouseNumber = ProductCountingWarehouseModel.Number,

        };


        var inCountingTransactionLineDto = new InCountingTransactionLineDto
        {
            ProductCode = ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.MainItemCode : ProductCountingBasketModel.ItemCode,
            VariantCode = ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.ItemCode : string.Empty,
            WarehouseNumber = ProductCountingWarehouseModel.Number,
            Quantity = ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity,
            ConversionFactor = ProductCountingBasketModel.ConversionFactor * ProductCountingBasketModel.DifferenceQuantity,
            OtherConversionFactor = ProductCountingBasketModel.OtherConversionFactor * ProductCountingBasketModel.DifferenceQuantity,
            SubUnitsetCode = ProductCountingBasketModel.SubUnitsetCode,
        };


        var serilotTransactionDto = new SeriLotTransactionDto
        {
            StockLocationCode = LocationModel.Code,
            InProductTransactionLineReferenceId = 0,
            OutProductTransactionLineReferenceId = 0,
            Quantity = ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity,
            SubUnitsetCode = ProductCountingBasketModel.SubUnitsetCode,
            DestinationStockLocationCode = string.Empty,
            ConversionFactor = ProductCountingBasketModel.ConversionFactor * ProductCountingBasketModel.DifferenceQuantity,
            OtherConversionFactor = ProductCountingBasketModel.OtherConversionFactor * ProductCountingBasketModel.DifferenceQuantity,
        };

        inCountingTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);




        inCountingTransactionDto.Lines.Add(inCountingTransactionLineDto);

        var result = await _inCountingTransactionService.InsertInCountingTransaction(httpClient, inCountingTransactionDto, _httpClientService.FirmNumber);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = Title;
            resultModel.PageCountToBack = LocationModel == null ? 5 : 6;

			await ClearFormAsync();
			await ClearDataAsync();

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
            resultModel.ErrorMessage = result.Message;
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

    private async Task ClearFormAsync()
    {
        DocumentNumber = string.Empty;
        DocumentTrackingNumber = string.Empty;
        Description = string.Empty;
        SpecialCode = string.Empty;
        TransactionDate = DateTime.Now;
    }

    private async Task ClearDataAsync()
    {
        try
        {
            var locationListViewModel = _serviceProvider.GetRequiredService<ProductCountingLocationListViewModel>();
            var productListViewModel = _serviceProvider.GetRequiredService<ProductCountingProductListViewModel>();
            var warehouseTotalListViewModel = _serviceProvider.GetRequiredService<ProductCountingWarehouseTotalListViewModel>();
            var basketViewModel = _serviceProvider.GetRequiredService<ProductCountingBasketViewModel>();

            if(productListViewModel is not null)
            {
                if(productListViewModel.SelectedProduct is not null)
                {
                    productListViewModel.SelectedProduct.IsSelected = false;
					productListViewModel.SelectedProduct = null;
				}   
            }

            if(warehouseTotalListViewModel is not null)
            {
                if(warehouseTotalListViewModel.SelectedWarehouse is not null)
                {
                    warehouseTotalListViewModel.SelectedWarehouse.IsSelected = false;
                    warehouseTotalListViewModel.SelectedWarehouse = null;
                }
            }

            if(locationListViewModel is not null)
            {
                if(locationListViewModel.SelectedLocation is not null)
                {
                    locationListViewModel.SelectedLocation.IsSelected = false;
                    locationListViewModel.SelectedLocation = null;
                }
            }

            if(basketViewModel is not null)
            {
                basketViewModel.ProductCountingBasketModel.LocationTransactions.Clear();

			}
            

        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
}
