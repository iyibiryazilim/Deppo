using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;
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
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels
{
    [QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
    [QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
    [QueryProperty(nameof(Items), nameof(Items))]
	[QueryProperty(nameof(CollectedProducts), nameof(CollectedProducts))]

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

        ObservableCollection<ProcurementSalesProductModel> PackageProducts { get; } = new();

		[ObservableProperty]
		ObservableCollection<ProcurementSalesProductModel> collectedProducts;

		ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

		ObservableCollection<LocationTransactionModel> LineLocationTransactions { get; } = new();


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

        private async Task LoadPageAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                await Task.Delay(300);

                DocumentTrackingNumber = ProcurementSalesCustomerModel.DocumentTrackingNumber;
				DocumentNumber = ProcurementSalesCustomerModel.DocumentNumber;
				SpecialCode = ProcurementSalesCustomerModel.SpeCode;
				Description = ProcurementSalesCustomerModel.Description;

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

                if (ProcurementSalesCustomerModel.IsEDispatch)
                {
                    if(SelectedCarrier == null)
                    {
                        _userDialogs.Alert("Taşıyıcı seçmelisiniz.", "Uyarı", "Tamam");
                        return;
					}
					if (SelectedDriver == null)
					{
						_userDialogs.Alert("Şoför seçmelisiniz.", "Uyarı", "Tamam");
						return;
					}
				}

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

        private async Task RetailSalesDispatchTransactionInsertAsync(HttpClient httpClient)
        {
            foreach (var item in Items)
            {
                foreach (var product in item.PackageProducts)
                {

                    var p = PackageProducts.FirstOrDefault(x => x.ReferenceId == product.ReferenceId);
                    if (p is null)
                        PackageProducts.Add(product);
                    else
                        p.OutputQuantity += product.OutputQuantity;
                }
            }

			var dto = new RetailSalesDispatchUpdateDto
			{
				CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
				DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
				DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
				IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
				Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
				DocumentNumber = DocumentNumber,
				DocumentTrackingNumber = DocumentTrackingNumber,
				TransactionDate = TransactionDate,
				SpeCode = SpecialCode,
				Description = Description,
				FicheReferenceId = ProcurementSalesCustomerModel.ReferenceId,
                IsEDispatch = ProcurementSalesCustomerModel.IsEDispatch == true ? 1 : 0,
                FirmNumber = _httpClientService.FirmNumber
                
			};

			await _retailSalesDispatchTransactionService.UpdateFiche(httpClient, _httpClientService.FirmNumber, dto);

			foreach (var item in CollectedProducts)
            {
                LineLocationTransactions.Clear();

				if (!PackageProducts.Any(x=>x.ReferenceId == item.ReferenceId))
                {
					var locationTransactions = await _locationTransactionService.GetLineLocationTransactionsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ItemReferenceId, WarehouseModel.Number, 0, item.BaseTransactionReferenceId, item.ReferenceId, 0, 999999, string.Empty);


					if (locationTransactions.Data != null && locationTransactions.IsSuccess)
					{
						foreach (var locationTransaction in locationTransactions.Data)
						{
                            var locTransaction = Mapping.Mapper.Map<LocationTransactionModel>(locationTransaction);
							var locationResult = await _locationTransactionService.Delete(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, locTransaction.ReferenceId);
						}
					}

					var result = await _retailSalesDispatchTransactionService.Delete(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ReferenceId);
                }
                else
                {
                    var product = PackageProducts.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
					if (product.OutputQuantity != item.Quantity)
					{
						var tempQuantity = product.OutputQuantity;
						var locationTransactions = await _locationTransactionService.GetLineLocationTransactionsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ItemReferenceId, WarehouseModel.Number, 0, item.BaseTransactionReferenceId, item.ReferenceId, 0, 999999, string.Empty);

						if (locationTransactions.Data != null && locationTransactions.IsSuccess)
						{
							foreach (var locationTransaction in locationTransactions.Data)
							{
								LineLocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(locationTransaction));

							}

							foreach (var lineLocationTransaction in LineLocationTransactions.OrderBy(x => x.TransactionDate))
							{
								if (tempQuantity != 0)
								{
									if (tempQuantity > lineLocationTransaction.Quantity)
									{
										tempQuantity -= (double)lineLocationTransaction.Quantity;
									}
									else
									{
										var updateResult = await _locationTransactionService.Update(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, lineLocationTransaction.ReferenceId, tempQuantity);
										tempQuantity = 0;
									}
								}
								else
								{
									var deleteResult = await _locationTransactionService.Delete(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, lineLocationTransaction.ReferenceId);
								}
							}

						}

						var result = await _retailSalesDispatchTransactionService.UpdateLine(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ReferenceId, product.OutputQuantity);
					}
				}
            }


            var ficheResult = await _retailSalesDispatchTransactionService.UpdateFicheStatus(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber,ProcurementSalesCustomerModel.ReferenceId,7, ProcurementSalesCustomerModel.IsEDispatch == true ? 1 : 0);

         

            ResultModel resultModel = new(); 
            if (ficheResult.IsSuccess)
            {
                resultModel.IsSuccess = true;
                resultModel.Message = "Başarılı";
                resultModel.PageTitle = "Sevk İşlemi";
                resultModel.PageCountToBack = 6;

                await ClearFormAsync();
                await ClearDataAsync();

				await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });


            }
			else
            {
                resultModel.ErrorMessage = ficheResult.Message;
                resultModel.IsSuccess = false;
                resultModel.PageCountToBack = 1;
                resultModel.Message = "Başarısız";

				await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = resultModel
				});
			}

			if (_userDialogs.IsHudShowing)
		           _userDialogs.HideHud();

		}


		private async Task WholeSalesDispatchTransactionInsertAsync(HttpClient httpClient)
		{
			foreach (var item in Items)
			{
				foreach (var product in item.PackageProducts)
				{

					var p = PackageProducts.FirstOrDefault(x => x.ReferenceId == product.ReferenceId);
					if (p is null)
						PackageProducts.Add(product);
					else
						p.OutputQuantity += product.OutputQuantity;
				}
			}

			var dto = new RetailSalesDispatchUpdateDto
			{
				CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
				DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
				DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
				IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
				Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
				DocumentNumber = DocumentNumber,
				DocumentTrackingNumber = DocumentTrackingNumber,
				TransactionDate = TransactionDate,
				SpeCode = SpecialCode,
				Description = Description,
				FicheReferenceId = ProcurementSalesCustomerModel.ReferenceId,
				IsEDispatch = ProcurementSalesCustomerModel.IsEDispatch == true ? 1 : 0,
				FirmNumber = _httpClientService.FirmNumber

			};

			await _retailSalesDispatchTransactionService.UpdateFiche(httpClient, _httpClientService.FirmNumber, dto);


			foreach (var item in CollectedProducts)
			{
				LineLocationTransactions.Clear();

				if (!PackageProducts.Any(x => x.ReferenceId == item.ReferenceId))
				{
					var locationTransactions = await _locationTransactionService.GetLineLocationTransactionsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ItemReferenceId, WarehouseModel.Number, 0, item.BaseTransactionReferenceId, item.ReferenceId, 0, 999999, string.Empty);


					if (locationTransactions.Data != null && locationTransactions.IsSuccess)
					{
						foreach (var locationTransaction in locationTransactions.Data)
						{
							var locTransaction = Mapping.Mapper.Map<LocationTransactionModel>(locationTransaction);
							var locationResult = await _locationTransactionService.Delete(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, locTransaction.ReferenceId);
						}
					}

					var result = await _retailSalesDispatchTransactionService.Delete(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ReferenceId);
				}
				else
				{
					var product = PackageProducts.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
					if (product.OutputQuantity != item.Quantity)
					{
						var tempQuantity = product.OutputQuantity;
						var locationTransactions = await _locationTransactionService.GetLineLocationTransactionsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ItemReferenceId, WarehouseModel.Number, 0, item.BaseTransactionReferenceId, item.ReferenceId, 0, 999999, string.Empty);

						if (locationTransactions.Data != null && locationTransactions.IsSuccess)
						{
							foreach (var locationTransaction in locationTransactions.Data)
							{
								LineLocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(locationTransaction));

							}

							foreach (var lineLocationTransaction in LineLocationTransactions.OrderBy(x => x.TransactionDate))
							{
								if (tempQuantity != 0)
								{
									if (tempQuantity > lineLocationTransaction.Quantity)
									{
										tempQuantity -= (double)lineLocationTransaction.Quantity;
									}
									else
									{
										var updateResult = await _locationTransactionService.Update(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, lineLocationTransaction.ReferenceId, tempQuantity);
										tempQuantity = 0;
									}
								}
								else
								{
									var deleteResult = await _locationTransactionService.Delete(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, lineLocationTransaction.ReferenceId);
								}
							}

						}

						var result = await _retailSalesDispatchTransactionService.UpdateLine(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ReferenceId, product.OutputQuantity);
					}
				}
			}


			var ficheResult = await _retailSalesDispatchTransactionService.UpdateFicheStatus(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProcurementSalesCustomerModel.ReferenceId,8,ProcurementSalesCustomerModel.IsEDispatch == true ? 1 :0);

			

			ResultModel resultModel = new();
			if (ficheResult.IsSuccess)
			{
				resultModel.IsSuccess = true;
				resultModel.Message = "Başarılı";
				resultModel.PageTitle = "Sevk İşlemi";
				resultModel.PageCountToBack = 6;

                await ClearFormAsync();
                await ClearDataAsync();

				await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = resultModel
				});


			}
			else
			{
				resultModel.ErrorMessage = ficheResult.Message;
				resultModel.IsSuccess = false;
				resultModel.PageCountToBack = 1;
				resultModel.Message = "Başarısız";

				await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = resultModel
				});
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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

        private async Task ClearDataAsync()
        {
            try
            {
                var warehouseViewModel = _serviceProvider.GetRequiredService<ProcurementSalesProcessWarehouseListViewModel>();
                var customerViewModel = _serviceProvider.GetRequiredService<ProcurementSalesProcessCustomerListViewModel>();
                var packageBasketViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
                var productBasketViewModel = _serviceProvider.GetRequiredService<ProcurementSalesProcessProductBasketViewModel>();

                if(warehouseViewModel is not null && warehouseViewModel.SelectedWarehouseModel is not null)
                {
                    warehouseViewModel.SelectedWarehouseModel.IsSelected = false;
                    warehouseViewModel.SelectedWarehouseModel = null;
                }

                if(customerViewModel is not null && customerViewModel.SelectedCustomerModel is not null)
                {
                    customerViewModel.SelectedCustomerModel.IsSelected = false;
					customerViewModel.SelectedCustomerModel = null;
				}

                if(packageBasketViewModel is not null)
                {
                    if(packageBasketViewModel.SelectedPackageBasketModel is not null)
                    {
                        packageBasketViewModel.SelectedPackageBasketModel.PackageProducts.Clear();
                        packageBasketViewModel.SelectedPackageBasketModel = null;
                    }
                    foreach (var item in packageBasketViewModel.Items)
                    {
                        item.PackageProducts.Clear();
                    }
                    packageBasketViewModel.Items.Clear();
                    packageBasketViewModel.SelectedItems.Clear();
                    packageBasketViewModel.CollectedProducts.Clear();
				}

                if(productBasketViewModel is not null)
                {
                    productBasketViewModel.Items.Clear();
                    productBasketViewModel.SelectedSalesProductModel.IsSelected = false;
                    productBasketViewModel.SelectedSalesProductModel = null;
				}

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
