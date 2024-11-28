using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;
using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Services;
using System.Collections.ObjectModel;
using static Android.Util.EventLogTags;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Helpers.MappingHelper;
using System.Text;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels
{
	[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
	[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
	[QueryProperty(nameof(SelectedItems), nameof(SelectedItems))]
	[QueryProperty(nameof(SelectedOrderWarehouseModel), nameof(SelectedOrderWarehouseModel))]
	[QueryProperty(nameof(BasketModel), nameof(BasketModel))]
	public partial class ProcurementByLocationCustomerFormViewModel : BaseViewModel
	{
		private readonly IHttpClientService _httpClientService;
		private readonly IUserDialogs _userDialogs;
		private readonly IProcurementByLocationCustomerService _procurementByLocationCustomerService;
		private readonly ILocationTransactionService _locationTransactionService;
		private readonly ITransferTransactionService _transferTransactionService;
		private readonly IHttpClientSysService _httpClientSysService;
		private readonly IProcurementAuditCustomerService _procurementAuditCustomerService;
		private readonly IServiceProvider _serviceProvider;

		public ProcurementByLocationCustomerFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProcurementByLocationCustomerService procurementByLocationCustomerService, ILocationTransactionService locationTransactionService, ITransferTransactionService transferTransactionService, IHttpClientSysService httpClientSysService, IProcurementAuditCustomerService procurementAuditCustomerService, IServiceProvider serviceProvider)
		{
			_httpClientService = httpClientService;
			_userDialogs = userDialogs;
			_procurementByLocationCustomerService = procurementByLocationCustomerService;
			_locationTransactionService = locationTransactionService;
			_transferTransactionService = transferTransactionService;
			_httpClientSysService = httpClientSysService;
			_procurementAuditCustomerService = procurementAuditCustomerService;
			_serviceProvider = serviceProvider;


			Title = "Form";

			LoadItemsCommand = new Command(async () => await LoadItemsAsync());
			ItemTappedCommand = new Command<ProcurementLocationFormModel>(async (procurementLocationFormModel) => await ItemTappedAsync(procurementLocationFormModel));
			SaveCommand = new Command(async () => await SaveAsync());
			SwipeItemCommand = new Command<ProcurementLocationFormModel>(async (item) => await SwipeItemAsync(item));
			CloseShowProductCommand = new Command(async () => await CloseShowProductAsync());
		}


		[ObservableProperty]
		ObservableCollection<Core.Models.ProcurementModels.ByLocationModels.ProcurementCustomerBasketModel> selectedItems;

		[ObservableProperty]
		ObservableCollection<ProcurementLocationFormModel> items = new();

		[ObservableProperty]
		ObservableCollection<ProcurementLocationBasketModel> basketModel;

		[ObservableProperty]
		ObservableCollection<Core.Models.ProcurementModels.ByLocationModels.ProcurementCustomerModel> procurementCustomers = new();

		public ObservableCollection<LocationTransaction> LocationTransactions { get; } = new();

		[ObservableProperty]
		private ProcurementLocationFormModel selectedItem;


		[ObservableProperty]
		private WarehouseModel warehouseModel = null!;

		[ObservableProperty]
		private WarehouseModel selectedOrderWarehouseModel = null!;

		[ObservableProperty]
		private LocationModel locationModel = null!;

		public Command LoadItemsCommand { get; }
		public Command ItemTappedCommand { get; }
		public Command SaveCommand { get; }
		public Command SwipeItemCommand { get; }
		public Command CloseShowProductCommand { get; }

		public Page CurrentPage { get; set; }

		private async Task LoadItemsAsync()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				_userDialogs.ShowLoading("Loading...");
				await Task.Delay(200);
				Items.Clear();
				var httpClient = _httpClientService.GetOrCreateHttpClient();

				foreach (var item in BasketModel)
				{
					foreach (var customer in item.ProcurementCustomers)
					{

						if (!Items.Any(x => x.ProcurementCustomerModel?.CustomerReferenceId == customer.CustomerReferenceId))
						{
							var formBasket = new ProcurementLocationFormModel();
							formBasket.ProcurementCustomerModel = customer;
							var product = item.ProcurementByLocationProduct;
							product.OutputQuantity = customer.OutputQuantity;
							formBasket.ProcurementByLocationProduct.Add(product);
							Items.Add(formBasket);
						}
						else
						{
							var formBasket = Items.FirstOrDefault(x => x.ProcurementCustomerModel?.CustomerReferenceId == customer.CustomerReferenceId);
							if (formBasket != null)
							{
								var product = item.ProcurementByLocationProduct;
								product.OutputQuantity = customer.OutputQuantity;
								formBasket.ProcurementByLocationProduct.Add(product);

							}
						}


					}
				}


			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				_userDialogs.Alert(ex.Message, "Error", "OK");
			}
			finally
			{
				IsBusy = false;
				_userDialogs.HideHud();
			}
		}

		private async Task ItemTappedAsync(ProcurementLocationFormModel procurementLocationFormModel)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationFormView)}", new Dictionary<string, object>
				{
					[nameof(WarehouseModel)] = WarehouseModel,
					[nameof(SelectedOrderWarehouseModel)] = SelectedOrderWarehouseModel,
					[nameof(ProcurementLocationFormModel)] = procurementLocationFormModel,
				});

			}
			catch (Exception ex)
			{
				_userDialogs.Alert(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public async Task LoadLocationTransactionAsync(ProcurementByLocationProduct procurementByLocationProduct)
		{
			try
			{
				LocationTransactions.Clear();
				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var result = await _locationTransactionService.GetInputObjectsAsync(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					productReferenceId: procurementByLocationProduct.IsVariant ? procurementByLocationProduct.MainItemReferenceId : procurementByLocationProduct.ItemReferenceId,
					variantReferenceId: procurementByLocationProduct.IsVariant ? procurementByLocationProduct.ItemReferenceId : 0,
					warehouseNumber: SelectedOrderWarehouseModel.Number,
					locationRef: LocationModel.ReferenceId,
					skip: 0,
					take: 99999,
					search: string.Empty
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

			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

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

				var confirm = await _userDialogs.ConfirmAsync("Transfer işlemi yapmak istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
				if (!confirm)
					return;

				_userDialogs.ShowLoading("İşlem Tamamlanıyor...");
				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var allCodes = new StringBuilder(); // Holds all codes for successful transactions

				foreach (var item in Items)
				{
					var transferTransactionInsertDto = new TransferTransactionInsert
					{
						Code = string.Empty,
						IsEDispatch = 0,
						SpeCode = item.SpecialCode,
						CurrentCode = item.ProcurementCustomerModel?.CustomerCode ?? string.Empty,
						DoCode = item.DocumentNumber,
						TransactionDate = item.TransactionDate,
						Description = item.Description,
						DestinationWarehouseNumber = SelectedOrderWarehouseModel.Number,
						FirmNumber = _httpClientService.FirmNumber,
						WarehouseNumber = WarehouseModel.Number,
						ShipInfoCode = item.ProcurementCustomerModel?.ShipAddress.Code ?? string.Empty
						// ShipInfoCode can be set here if needed
					};

					foreach (var product in item.ProcurementByLocationProduct)
					{
						var tempProductQuantity = product.OutputQuantity;
						var transferTransactionLineDto = new TransferTransactionLineDto
						{
							ProductCode = product.IsVariant ? product.MainItemCode : product.ItemCode,
							VariantCode = product.IsVariant ? product.ItemCode : string.Empty,
							WarehouseNumber = WarehouseModel.Number,
							DestinationWarehouseNumber = SelectedOrderWarehouseModel.Number,
							SubUnitsetCode = product.SubUnitsetCode,
							ConversionFactor = 1,
							OtherConversionFactor = 1,
							Quantity = product.OutputQuantity,
						};

						await LoadLocationTransactionAsync(product);

						LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

						foreach (var locationTransaction in LocationTransactions)
						{
							var tempLocationTransactionQuantity = locationTransaction.RemainingQuantity;
							while (tempLocationTransactionQuantity > 0 && tempProductQuantity > 0)
							{
								var serilotTransactionDto = new SeriLotTransactionDto
								{
									StockLocationCode = LocationModel.Code,
									InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
									OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
									SubUnitsetCode = product.SubUnitsetCode,
									DestinationStockLocationCode = locationTransaction.LocationCode,
									ConversionFactor = 1,
									OtherConversionFactor = 1,
									Quantity = tempProductQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempProductQuantity,
								};

								tempLocationTransactionQuantity -= (double)serilotTransactionDto.Quantity;
								transferTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
								tempProductQuantity -= (double)serilotTransactionDto.Quantity;
							}
						}

						transferTransactionInsertDto.Lines.Add(transferTransactionLineDto);
					}

					var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);
					ResultModel resultModel = new();

					if (result.IsSuccess)
					{
						allCodes.AppendLine(result.Data.Code); 

						resultModel.Message = "Başarılı";
						resultModel.Code = allCodes.ToString(); 
						resultModel.PageTitle = "Ambar Transferi";
						resultModel.PageCountToBack = 8;

						var customerFormViewModel = _serviceProvider.GetRequiredService<ProcurementByLocationCustomerFormViewModel>();
						customerFormViewModel.Items.Clear();

						var basketViewModel = _serviceProvider.GetRequiredService<ProcurementByLocationBasketViewModel>();
						basketViewModel.Items.Clear();

						var productViewModel = _serviceProvider.GetRequiredService<ProcurementByLocationProductListViewModel>();
						productViewModel.SelectedItems.Clear();

						var customerViewModel = _serviceProvider.GetRequiredService<ProcurementByLocationCustomerListViewModel>();
						customerViewModel.SelectedItems.Clear();

						try
						{
							foreach (var customers in Items.Where(x => x.ProcurementCustomerModel?.RejectionCode != string.Empty))
							{
								foreach (var item1 in customers.ProcurementByLocationProduct)
								{
									var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

									ProcurementAuditCustomerDto procurementAuditCustomerDto = new ProcurementAuditCustomerDto
									{
										ApplicationUser = _httpClientSysService.UserOid,
										ReasonsForRejectionProcurement = (Guid)(customers.ProcurementCustomerModel?.RejectionOid),
										CurrentCode = customers.ProcurementCustomerModel?.CustomerCode ?? string.Empty,
										CurrentName = customers.ProcurementCustomerModel?.CustomerName ?? string.Empty,
										CurrentReferenceId = (int)(customers.ProcurementCustomerModel?.CustomerReferenceId),
										IsVariant = item1.IsVariant,
										Quantity = item1.OutputQuantity,
										ProcurementQuantity = customers.ProcurementCustomerModel.WaitingQuantity,
										ProductName = item1.IsVariant ? item1.MainItemName : item1.ItemName,
										ProductReferenceId = item1.IsVariant ? item1.MainItemReferenceId : item1.ItemReferenceId,
										CreatedOn = DateTime.Now,
										LocationCode = LocationModel.Code,
										LocationReferenceId = LocationModel.ReferenceId,
										LocationName = LocationModel.Name,
										WarehouseName = WarehouseModel.Name,
										WarehouseNumber = WarehouseModel.Number,
									};

									await _procurementAuditCustomerService.CreateAsync(
										httpClient: httpSysClient,
										dto: procurementAuditCustomerDto
									);
								}
							}
						}
						catch (Exception ex)
						{
							if (_userDialogs.IsHudShowing)
								_userDialogs.HideHud();
							_userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
						}
					}
					else
					{
						resultModel.Message = "Başarısız";
						resultModel.PageTitle = "Ambar Transferi";
						resultModel.PageCountToBack = 1;
						resultModel.ErrorMessage = result.Message;

						if (_userDialogs.IsHudShowing)
							_userDialogs.HideHud();

						await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
						{
							[nameof(ResultModel)] = resultModel
						});

						return; 
					}
				}

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
				{
					[nameof(ResultModel)] = new ResultModel
					{
						Message = "Başarılı",
						Code = allCodes.ToString(),
						PageTitle = "Ambar Transferi",
						PageCountToBack = 8
					}
				});
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

		private async Task SwipeItemAsync(ProcurementLocationFormModel item)
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;

				if (item is not null)
				{
					SelectedItem = item;
					CurrentPage.FindByName<BottomSheet>("showProductsbottomsheet").State = BottomSheetState.HalfExpanded;
				}
			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task CloseShowProductAsync()
		{
			if (IsBusy) return;

			try
			{
				IsBusy = true;


				CurrentPage.FindByName<BottomSheet>("showProductsbottomsheet").State = BottomSheetState.Hidden;

			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			}
			finally
			{
				IsBusy = false;
			}
		}

	}
}
