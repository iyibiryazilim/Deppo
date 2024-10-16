﻿using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels
{
    [QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
    [QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
    [QueryProperty(nameof(Items), nameof(Items))]
    public partial class ProcurementSalesProcessFormViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ICarrierService _carrierService;
        private readonly IDriverService _driverService;
        private readonly IRetailSalesDispatchTransactionService _retailSalesDispatchTransactionService;
        private readonly IWholeSalesDispatchTransactionService _wholeSalesDispatchTransactionService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserDialogs _userDialogs;
        private readonly ILocationTransactionService _locationTransactionService;
        private readonly ITransferTransactionService _transferTransactionService;

        [ObservableProperty]
        WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        ProcurementSalesCustomerModel procurementSalesCustomerModel = null!;

        [ObservableProperty]
        ObservableCollection<ProcurementPackageBasketModel> items = null!;

        ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

        public ObservableCollection<Carrier> Carriers { get; } = new();
        public ObservableCollection<Driver> Drivers { get; } = new();

        [ObservableProperty]
        Carrier? selectedCarrier;
        [ObservableProperty]
        Driver? selectedDriver;

        [ObservableProperty]
        DateTime transactionDate = DateTime.Now;

        [ObservableProperty]
        string documentNumber = string.Empty;

        [ObservableProperty]
        string documentTrackingNumber = string.Empty;

        [ObservableProperty]
        string specialCode = string.Empty;

        [ObservableProperty]
        string description = string.Empty;

        [ObservableProperty]
        string cargoTrackingNumber = string.Empty;

        public ProcurementSalesProcessFormViewModel(IHttpClientService httpClientService, ICarrierService carrierService, IDriverService driverService, IUserDialogs userDialogs, IRetailSalesDispatchTransactionService retailSalesDispatchTransactionService, IWholeSalesDispatchTransactionService wholeSalesDispatchTransactionService, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService, ITransferTransactionService transferTransactionService)
        {
            _httpClientService = httpClientService;

            _carrierService = carrierService;
            _driverService = driverService;
            _userDialogs = userDialogs;
            _retailSalesDispatchTransactionService = retailSalesDispatchTransactionService;
            _wholeSalesDispatchTransactionService = wholeSalesDispatchTransactionService;
            _serviceProvider = serviceProvider;
            _transferTransactionService = transferTransactionService;
            _locationTransactionService = locationTransactionService;


            Title = "Sevk İşlemi";

            LoadPageCommand = new Command(async () => await LoadPageAsync());
            ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            SelectWholeCommand = new Command(async () => await SelectWholeAsync());
            SelectRetailCommand = new Command(async () => await SelectRetailAsync());
            BackCommand = new Command(async () => await BackAsync());

            LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
            LoadDriversCommand = new Command(async () => await LoadDriversAsync());
        }

        public Page CurrentPage { get; set; } = null!;

        public Command LoadPageCommand { get; }
        public Command BackCommand { get; }
        public Command SaveCommand { get; }
        public Command ShowBasketItemCommand { get; }

        public Command LoadCarriersCommand { get; }
        public Command LoadDriversCommand { get; }

        public Command SelectWholeCommand { get; }
        public Command SelectRetailCommand { get; }


        private async Task ShowBasketItemAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                //CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
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

        private async Task LoadPageAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

               // CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

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

        private async Task LoadCarriersAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                Carriers.Clear();
                SelectedCarrier = null;

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _carrierService.GetObjects(
                        httpClient: httpClient,
                        firmNumber: _httpClientService.FirmNumber

                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Carriers.Add(Mapping.Mapper.Map<Carrier>(item));
                    }
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
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
        }

        private async Task LoadDriversAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                Drivers.Clear();
                SelectedDriver = null;

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _driverService.GetObjects(
                        httpClient: httpClient
                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Drivers.Add(Mapping.Mapper.Map<Driver>(item));
                    }
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
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                await OpenInsertOptionsAsync();

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
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
        }

       

        private async Task UpdateTransferTransaction(string ficheNumber)
        {
            try
            {
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _transferTransactionService.UpdateDocumentTrackingNumber(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ficheNumber, ProcurementSalesCustomerModel.ReferenceId);
            }
            catch (Exception ex)
            {

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
        }
        private async Task SelectWholeAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var confirmInsert = await _userDialogs.ConfirmAsync("Siparişe Bağlı Toptan Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
                if (!confirmInsert)
                {
                    await CloseInsertOptionsAsync();
                    return;
                }
                await CloseInsertOptionsAsync();

                _userDialogs.Loading("İşlem tamamlanıyor...");

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await WholeSalesDispatchTransactionInsertAsync(httpClient);

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

        private async Task SelectRetailAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var confirmInsert = await _userDialogs.ConfirmAsync("Siparişe Bağlı Perakende Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
                if (!confirmInsert)
                {
                    await CloseInsertOptionsAsync();
                    return;
                }
                await CloseInsertOptionsAsync();

                _userDialogs.Loading("İşlem tamamlanıyor...");

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await RetailSalesDispatchTransactionInsertAsync(httpClient);

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

        public async Task LoadLocationTransaction(ProcurementSalesProductModel procurementSalesProductModel)
        {
            try
            {

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _locationTransactionService.GetInputObjectsAsync(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    productReferenceId: procurementSalesProductModel.IsVariant ? procurementSalesProductModel.MainItemReferenceId : procurementSalesProductModel.ItemReferenceId,
                    variantReferenceId: procurementSalesProductModel.IsVariant ? procurementSalesProductModel.ItemReferenceId : 0,
                    warehouseNumber: WarehouseModel.Number,
                    skip: 0,
                    take: 999999,
                    search: ""
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
            finally
            {
                IsBusy = false;
            }
        }

        private async Task WholeSalesDispatchTransactionInsertAsync(HttpClient httpClient)
        {
            var dto = new WholeSalesDispatchTransactionInsert
            {
                Code = "",
                CurrentCode = ProcurementSalesCustomerModel != null ? ProcurementSalesCustomerModel.CustomerCode : "",
                DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
                ShipInfoCode = ProcurementSalesCustomerModel?.ShipAddressReferenceId != 0 ? ProcurementSalesCustomerModel?.ShipAddressCode : "",
                DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
                CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
                IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
                Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
                IsEDispatch = (short?)((bool)ProcurementSalesCustomerModel?.IsEDispatch ? 1 : 0),
                DispatchType = (short?)((bool)ProcurementSalesCustomerModel?.IsEDispatch ? 1 : 0),
                DispatchStatus = 1,
                EDispatchProfileId = (short?)((bool)ProcurementSalesCustomerModel?.IsEDispatch ? 1 : 0),
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
                foreach (var product in item.PackageProducts)
                {
                    var wholeSalesDispatchTransactionLineDto = new WholeSalesDispatchTransactionLineInsert
                    {
                        ProductCode = product.IsVariant ? product.MainItemCode : product.ItemCode,
                        VariantCode = product.IsVariant ? product.ItemCode : string.Empty,
                        WarehouseNumber = (short?)WarehouseModel.Number,
                        Quantity = product.OutputQuantity,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                        SubUnitsetCode = product.SubUnitsetCode,
                    };

                    await LoadLocationTransaction(product);
                    LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                    foreach (var locationTransaction in LocationTransactions)
                    {
                        while (locationTransaction.RemainingQuantity > 0 && product.OutputQuantity> 0)
                        {
                            var serilotTransactionDto = new SeriLotTransactionDto
                            {
                                StockLocationCode = locationTransaction.LocationCode,
                                InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                                OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                                Quantity = product.OutputQuantity > locationTransaction.RemainingQuantity ? locationTransaction.RemainingQuantity : product.OutputQuantity,
                                SubUnitsetCode = product.SubUnitsetCode,
                                DestinationStockLocationCode = string.Empty,
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                            };

                            wholeSalesDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                            locationTransaction.RemainingQuantity -= (double)serilotTransactionDto.Quantity;
                            product.OutputQuantity -= (double)serilotTransactionDto.Quantity;

                        }
                    }

                    dto.Lines.Add(wholeSalesDispatchTransactionLineDto);


                }

            }
            Console.WriteLine(dto);

            var result = await _wholeSalesDispatchTransactionService.InsertWholeSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                await UpdateTransferTransaction(result.Data.Code);

                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 6;

               
                await ClearFormAsync();

                var basketViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
                basketViewModel.Items.Clear();

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
                resultModel.PageCountToBack = 1;
                await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
        }

        private async Task RetailSalesDispatchTransactionInsertAsync(HttpClient httpClient)
        {
            var dto = new RetailSalesDispatchTransactionInsert
            {
                Code = "",
                CurrentCode = ProcurementSalesCustomerModel != null ? ProcurementSalesCustomerModel.CustomerCode : "",
                DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
                ShipInfoCode = ProcurementSalesCustomerModel?.ShipAddressReferenceId != 0 ? ProcurementSalesCustomerModel?.ShipAddressCode : "",
                DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
                CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
                IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
                Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
                IsEDispatch = (short?)((bool)ProcurementSalesCustomerModel?.IsEDispatch ? 1 : 0),
                DispatchType = (short?)((bool)ProcurementSalesCustomerModel?.IsEDispatch ? 1 : 0),
                DispatchStatus = 1,
                EDispatchProfileId = (short?)((bool)ProcurementSalesCustomerModel?.IsEDispatch ? 1 : 0),
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
                foreach (var product in item.PackageProducts)
                {
                    var retailSalesDispatchTransactionLineDto = new RetailSalesDispatchTransactionLineInsert
                    {
                        ProductCode = product.IsVariant ? product.MainItemCode : product.ItemCode,
                        VariantCode = product.IsVariant ? product.ItemCode : string.Empty,
                        WarehouseNumber = (short?)WarehouseModel.Number,
                        Quantity = product.OutputQuantity,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                        SubUnitsetCode = product.SubUnitsetCode,
                    };

                    await LoadLocationTransaction(product);
                    LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                    foreach (var locationTransaction in LocationTransactions)
                    {
                        while (locationTransaction.RemainingQuantity > 0 && product.OutputQuantity > 0)
                        {
                            var serilotTransactionDto = new SeriLotTransactionDto
                            {
                                StockLocationCode = locationTransaction.LocationCode,
                                InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                                OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                                Quantity = product.OutputQuantity > locationTransaction.RemainingQuantity ? locationTransaction.RemainingQuantity : product.OutputQuantity,
                                SubUnitsetCode = product.SubUnitsetCode,
                                DestinationStockLocationCode = string.Empty,
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                            };

                            retailSalesDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                            locationTransaction.RemainingQuantity -= (double)serilotTransactionDto.Quantity;
                            product.OutputQuantity -= (double)serilotTransactionDto.Quantity;

                        }
                    }

                    dto.Lines.Add(retailSalesDispatchTransactionLineDto);


                }

            }
            Console.WriteLine(dto);

            var result = await _retailSalesDispatchTransactionService.InsertRetailSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                await UpdateTransferTransaction(result.Data.Code);

                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 6;

            
                await ClearFormAsync();

                var basketViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
                basketViewModel.Items.Clear();

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

                var result = await _userDialogs.ConfirmAsync("Form verileri silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
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
                CargoTrackingNumber = string.Empty;
                DocumentNumber = string.Empty;
                SpecialCode = string.Empty;
                DocumentTrackingNumber = string.Empty;
                Description = string.Empty;
                CargoTrackingNumber = string.Empty;
                SelectedCarrier = null;
                SelectedDriver = null;

            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }
        }

        private async Task CloseInsertOptionsAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("insertOptionsBottomSheet").State = BottomSheetState.Hidden;
            });
        }

        private async Task OpenInsertOptionsAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("insertOptionsBottomSheet").State = BottomSheetState.HalfExpanded;
            });
        }
    }
}
