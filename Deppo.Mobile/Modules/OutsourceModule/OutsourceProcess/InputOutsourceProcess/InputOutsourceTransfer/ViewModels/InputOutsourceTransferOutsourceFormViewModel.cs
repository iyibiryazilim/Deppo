using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    [QueryProperty(name: nameof(Items), queryId: nameof(Items))]
    [QueryProperty(name: nameof(OutsourceModel), queryId: nameof(OutsourceModel))]
    public partial class InputOutsourceTransferOutsourceFormViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly IPurchaseDispatchTransactionService _purchaseDispatchTransactionService;

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private OutsourceModel outsourceModel = null!;

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
        private ObservableCollection<InputOutsourceTransferBasketModel> items = null!;

        public InputOutsourceTransferOutsourceFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _purchaseDispatchTransactionService = purchaseDispatchTransactionService;
            Items = new();
            Title = "Fason Kabul İşlemi";

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
            catch (System.Exception)
            {
                throw;
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
                Console.WriteLine(Items);

                CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

                Console.WriteLine(Items);
            }
            catch (Exception ex)
            {
                _userDialogs.AlertAsync(ex.Message, "hata");
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

                _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var purchaseDispatchDto = new PurchaseDispatchTransactionInsert
                {
                    SpeCode = SpecialCode,
                    CurrentCode = OutsourceModel.Code,
                    Code = string.Empty,
                    DocTrackingNumber = DocumentTrackingNumber,
                    DoCode = DocumentNumber,
                    TransactionDate = FicheDate,
                    FirmNumber = _httpClientService.FirmNumber,
                    WarehouseNumber = WarehouseModel.Number,
                    Description = Description,
                    IsEDispatch = (short?)((bool)OutsourceModel?.IsEDispatch ? 1 : 0),
                    DispatchType = (short?)((bool)OutsourceModel?.IsEDispatch ? 1 : 0),
                    DispatchStatus = 1,
                    EDispatchProfileId = (short?)((bool)OutsourceModel?.IsEDispatch ? 1 : 0),
                };

                foreach (var item in Items)
                {
                    var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                    {
                        ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                        VariantCode = item.IsVariant ? item.ItemCode : "",

                        WarehouseNumber = (short)WarehouseModel.Number,
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
                            SubUnitsetCode = item.SubUnitsetCode,
                            Quantity = detail.Quantity,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                            DestinationStockLocationCode = string.Empty,
                        };
                        purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                    }

                    purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);
                }

                var result = await _purchaseDispatchTransactionService.InsertPurchaseDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, purchaseDispatchDto);

                Console.WriteLine(result);
                ResultModel resultModel = new();
                if (result.IsSuccess)
                {
                    resultModel.Message = "Başarılı";
                    resultModel.Code = result.Data.Code;
                    resultModel.PageTitle = Title;
                    resultModel.PageCountToBack = 5;

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
                    resultModel.PageCountToBack = 4;
                    await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                    {
                        [nameof(ResultModel)] = resultModel
                    });
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}