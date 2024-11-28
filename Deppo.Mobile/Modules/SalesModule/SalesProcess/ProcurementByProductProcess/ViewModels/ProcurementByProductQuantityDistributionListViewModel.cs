using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;
using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Services;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
[QueryProperty(name: nameof(ProcurementItem), queryId: nameof(ProcurementItem))]
[QueryProperty(name: nameof(ProcurementProductBasketModel), queryId: nameof(ProcurementProductBasketModel))]
public partial class ProcurementByProductQuantityDistributionListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ITransferTransactionService _transferTransactionService;
	private readonly IProcurementAuditCustomerService _procurementAuditCustomerService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ProcurementProductBasketModel procurementProductBasketModel;                                                          

	[ObservableProperty]
	ObservableCollection<ProcurementProductBasketModel> items;

	[ObservableProperty]
	ProcurementProductBasketProductModel procurementItem;


	[ObservableProperty]
	CustomerOrderModel selectedCustomer;

	[ObservableProperty]
	public List<ProcurementProductFormModel> procurementProductFormModels = new();

	public ProcurementByProductQuantityDistributionListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IHttpClientSysService httpClientSysService, ILocationTransactionService locationTransactionService, ITransferTransactionService transferTransactionService, IProcurementAuditCustomerService procurementAuditCustomerService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_httpClientSysService = httpClientSysService;
		_locationTransactionService = locationTransactionService;
		_transferTransactionService = transferTransactionService;
		_procurementAuditCustomerService = procurementAuditCustomerService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		Title = "Miktar Dağıtım Sayfası";

		IncreaseCommand = new Command<CustomerOrderModel>(async (x) => await IncreaseAsync(x));
		DecreaseCommand = new Command<CustomerOrderModel>(async (x) => await DecreaseAsync(x));
		QuantityTappedCommand = new Command<CustomerOrderModel>(async (x) => await QuantityTappedAsync(x));
		ItemTappedCommand = new Command<CustomerOrderModel>(async (x) => await ItemTappedAsync(x));
		SaveCommand = new Command(async () => await SaveAsync());
	}
	private ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

	public Page CurrentPage { get; set; } = null!;
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command QuantityTappedCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command BackCommand { get; }
	public Command SaveCommand { get; }


	private async Task IncreaseAsync(CustomerOrderModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			SelectedCustomer = item;

			var totalDistributedQuantity = ProcurementProductBasketModel.SelectedCustomers.Sum(x => x.DistributedQuantity);

			if (ProcurementItem.Quantity > item.DistributedQuantity && ProcurementItem.Quantity > totalDistributedQuantity)
			{
				item.DistributedQuantity++;
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

	private async Task DecreaseAsync(CustomerOrderModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			SelectedCustomer = item;

			if (item.DistributedQuantity > 0)
			{
				item.DistributedQuantity--;
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

	private async Task QuantityTappedAsync(CustomerOrderModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			SelectedCustomer = item;

			var result = await CurrentPage.DisplayPromptAsync(
				title: "Miktar Girişi",
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.DistributedQuantity.ToString(),
				keyboard: Keyboard.Numeric
			);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > ProcurementItem.Quantity)
			{
				await _userDialogs.AlertAsync($"Girilen miktar, ürünün toplanabilir miktarını ({ProcurementItem.Quantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			var totalDistributedQuantity = ProcurementProductBasketModel.SelectedCustomers.Where(x => x.Code != item.Code
			).Sum(x => x.DistributedQuantity);

			if (totalDistributedQuantity + quantity > ProcurementItem.Quantity)
			{
				await _userDialogs.AlertAsync($"Toplam dağıtılan miktar, ürünün toplanabilir miktarını ({ProcurementItem.Quantity}) aşmamalıdır.", "Hata", "Tamam");

				return;
			}

			SelectedCustomer.DistributedQuantity = quantity;

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

	private async Task ItemTappedAsync(CustomerOrderModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedCustomer = item;

			await Shell.Current.GoToAsync($"{nameof(ProcurementByProductFormView)}", new Dictionary<string, object>
			{
				["SelectedCustomer"] = SelectedCustomer,
				["ProcurementItem"] = ProcurementItem,
				["ProcurementProductBasketModel"] = ProcurementProductBasketModel,
				["Items"] = Items
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


	private async Task SaveAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (ProcurementProductFormModels.Count == 0 && ProcurementProductFormModels.Count != ProcurementProductBasketModel.SelectedCustomers.Count)
			{
				await _userDialogs.AlertAsync("Lütfen tüm müşteriler için form doldurunuz", "Uyarı", "Tamam");

				return;
			}

			var confirm = await _userDialogs.ConfirmAsync("İşleminiz kaydedilecektir! Devam etmek istediğinize emin misiniz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			_userDialogs.Loading("İşlem Tamamlanıyor...");
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			int insertSuccessCount = 0;
			int insertFailureCount = 0;
			int procurementProductFormModelsCount = ProcurementProductFormModels.Count;

			ResultModel resultModel = new(); // for Success and Failure page
			resultModel.Code = procurementProductFormModelsCount == 1 ? "Ambar Transfer Fiş Numarası: " :  "Ambar Transfer Fiş Numaraları: ";

			foreach (var formModel in ProcurementProductFormModels.Where(x => x.SelectedCustomer.DistributedQuantity > 0))
			{
				var tempCustomerDistributedQuantity = formModel.SelectedCustomer.DistributedQuantity;
				var transferTransactionInsertDto = new TransferTransactionInsert
				{
					Code = string.Empty,
					FirmNumber = _httpClientService.FirmNumber,
					WarehouseNumber = ProcurementProductBasketModel.ProcurementWarehouse.Number,
					TransactionDate = formModel.TransactionDate,
					Description = formModel.Description,
					SpeCode = formModel.SpecialCode,
					DoCode = formModel.DocumentNumber,
					DocTrackingNumber = string.Empty,
					CurrentCode = formModel.SelectedCustomer.Code ?? "",
					DestinationWarehouseNumber = ProcurementProductBasketModel.OrderWarehouse.Number,
					IsEDispatch = 0,
					ShipInfoCode = formModel.SelectedCustomer.ShipAddress.Code ?? "",
				};

				var transferTransactionLineDto = new TransferTransactionLineDto
				{
					ProductCode = formModel.ProcurementItem.IsVariant ? formModel.ProcurementItem.MainItemCode : formModel.ProcurementItem.ItemCode,
					VariantCode = formModel.ProcurementItem.IsVariant ? formModel.ProcurementItem.ItemCode : "",
					WarehouseNumber = ProcurementProductBasketModel.ProcurementWarehouse.Number,
					DestinationWarehouseNumber = ProcurementProductBasketModel.OrderWarehouse.Number,
					SubUnitsetCode = formModel.ProcurementItem.SubUnitsetCode,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
					Quantity = tempCustomerDistributedQuantity,
				};

				await LoadLocationTransactionAsync(formModel.ProcurementItem);
				var locationTransactionList  = LocationTransactions.OrderBy(X => X.TransactionDate);
				
                foreach (var locationTransaction in locationTransactionList)
				{
                    var tempLocationTransactionRemainingQuantity = locationTransaction.RemainingQuantity;
                    while (tempLocationTransactionRemainingQuantity > 0 && tempCustomerDistributedQuantity > 0)
                    {
						var seriLotTransactionDto = new SeriLotTransactionDto
						{
							StockLocationCode = ProcurementProductBasketModel.LocationCode,
							InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
							OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
							SubUnitsetCode = formModel.ProcurementItem.SubUnitsetCode,
							DestinationStockLocationCode = locationTransaction.LocationCode,
							ConversionFactor = 1,
							OtherConversionFactor = 1,
							Quantity = tempCustomerDistributedQuantity > tempLocationTransactionRemainingQuantity ? tempLocationTransactionRemainingQuantity : tempCustomerDistributedQuantity,
						};

						transferTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
						tempCustomerDistributedQuantity -= (double)seriLotTransactionDto.Quantity;
						tempLocationTransactionRemainingQuantity -= (double)seriLotTransactionDto.Quantity;
					}
                }
				transferTransactionInsertDto.Lines.Add(transferTransactionLineDto);

				var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);

				if(result.IsSuccess)
				{
					insertSuccessCount += 1;
					resultModel.Code += (procurementProductFormModelsCount == 1) ? result.Data?.Code :  result.Data?.Code + ", ";

					if(insertSuccessCount == procurementProductFormModelsCount)
					{
						resultModel.PageTitle = "Ambar Transferi";
						resultModel.PageCountToBack = 8;

						//try
						//{
						//	if(ProcurementItem.RejectionCode != string.Empty)
						//	{
						//		var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();
						//		ProcurementAuditCustomerDto procurementAuditCustomerDto = new ProcurementAuditCustomerDto
						//		{
						//			ApplicationUser = _httpClientSysService.UserOid,
						//			ReasonsForRejectionProcurement = ProcurementItem.RejectionOid,
						//			//CurrentCode = ProcurementCustomerBasketModel.CustomerCode ?? string.Empty,
						//			//CurrentName = ProcurementCustomerBasketModel.CustomerName ?? string.Empty,
						//			//CurrentReferenceId = ProcurementCustomerBasketModel.CustomerReferenceId,
						//			IsVariant = ProcurementItem.IsVariant,
						//			Quantity = ProcurementItem.Quantity,
						//			ProcurementQuantity = ProcurementItem.ProcurementQuantity,
						//			ProductName = ProcurementItem.IsVariant ? ProcurementItem.MainItemName : ProcurementItem.ItemName,
						//			ProductReferenceId = ProcurementItem.IsVariant ? ProcurementItem.MainItemReferenceId : ProcurementItem.ItemReferenceId,
						//			CreatedOn = DateTime.Now,
						//			LocationCode = ProcurementProductBasketModel.LocationCode,
						//			LocationReferenceId = ProcurementProductBasketModel.LocationReferenceId,
						//			LocationName = ProcurementProductBasketModel.LocationName,
						//			WarehouseName = ProcurementProductBasketModel.ProcurementWarehouse.Name,
						//			WarehouseNumber = ProcurementProductBasketModel.ProcurementWarehouse.Number,
						//		};
						//	}
								
						//}
						//catch (Exception ex)
						//{
						//	_userDialogs.Alert(ex.Message, "Hata", "Tamam");
							
						//}

						var customerListViewModel = _serviceProvider.GetRequiredService<ProcurementByProductCustomerListViewModel>();
						var procurementByProductListViewModel = _serviceProvider.GetRequiredService<ProcurementByProductListViewModel>();
						customerListViewModel.SelectedShipAddressModel = null;
						customerListViewModel.SelectedCustomer = null;
						customerListViewModel.SelectedItems.Clear();
						procurementByProductListViewModel.SelectedProductOrderModel = null;



						await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
						{
							[nameof(ResultModel)] = resultModel
						});

						if (_userDialogs.IsHudShowing)
							_userDialogs.HideHud();

					}
				}
				else
				{
					insertFailureCount += 1;
					resultModel.ErrorMessage = result.Message;
					resultModel.PageCountToBack = 1;
					resultModel.Message = "Ambar Transferi başarısız";

					await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
					{
						[nameof(ResultModel)] = resultModel
					});

					if (_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

				}
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


	private async Task LoadLocationTransactionAsync(ProcurementProductBasketProductModel product)
	{
		try
		{
			LocationTransactions.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _locationTransactionService.GetInputObjectsAsync(
			   httpClient: httpClient,
			   firmNumber: _httpClientService.FirmNumber,
			   periodNumber: _httpClientService.PeriodNumber,
			   productReferenceId: product.IsVariant ? product.MainItemReferenceId : product.ItemReferenceId,
			   variantReferenceId: product.IsVariant ? product.ItemReferenceId : 0,
			   warehouseNumber: ProcurementProductBasketModel.OrderWarehouse.Number,
			   locationRef: ProcurementProductBasketModel.LocationReferenceId,
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
