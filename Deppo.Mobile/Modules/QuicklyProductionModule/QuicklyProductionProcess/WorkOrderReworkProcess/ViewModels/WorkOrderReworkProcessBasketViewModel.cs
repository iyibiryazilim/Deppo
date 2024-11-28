using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

[QueryProperty(name: nameof(WorkOrderReworkBasketModel), queryId: nameof(WorkOrderReworkBasketModel))]
public partial class WorkOrderReworkProcessBasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly IUserDialogs _userDialogs;
	private readonly IWarehouseService _warehouseService;
	private readonly IQuicklyBomService _quicklyBomService;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	WorkOrderReworkBasketModel workOrderReworkBasketModel = null!;

	public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();

	public WorkOrderReworkProcessBasketViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IWarehouseService warehouseService, IQuicklyBomService quicklyBomService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_locationTransactionService = locationTransactionService;
		_userDialogs = userDialogs;
		_warehouseService = warehouseService;
		_quicklyBomService = quicklyBomService;
		_serviceProvider = serviceProvider;

		Title = "Sepet";

		LoadItemsSubCommand = new Command(async () => await LoadItemsSubAsync());
		IncreaseCommand = new Command(async () => await IncreaseAsync());
		DecreaseCommand = new Command(async () => await DecreaseAsync());

		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (x) => await LocationTransactionIncreaseAsync(x));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (x) => await LocationTransactionDecreaseAsync(x));
		LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());

		SubProductIncreaseCommand = new Command<WorkOrderReworkSubProductModel>(async (x) => await SubProductIncreaseAsync(x));
		SubProductDecreaseCommand = new Command<WorkOrderReworkSubProductModel>(async (x) => await SubProductDecreaseAsync(x));

		BackCommand = new Command(async () => await BackAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
	}
	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsSubCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	public Command SubProductIncreaseCommand { get; }
	public Command SubProductDecreaseCommand { get; }


	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command LocationTransactionIncreaseCommand { get; }
	public Command LocationTransactionDecreaseCommand { get; }
	public Command LocationTransactionConfirmCommand { get; }
	public Command LocationTransactionCloseCommand { get; }


	private async Task LoadItemsSubAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items");
			WorkOrderReworkBasketModel.WorkOrderReworkSubProducts.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _quicklyBomService.GetObjectsWorkSubProducts(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				mainProductReferenceId: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.MainItemReferenceId : WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.ReferenceId,
				search: string.Empty,
				skip: 0,
				take: 9999
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<BOMSubProductModel>(product);

					if (item is not null)
					{
						WorkOrderReworkSubProductModel workOrderReworkSubProductModel = new();
						workOrderReworkSubProductModel.ProductModel = item;
						workOrderReworkSubProductModel.SubAmount = item.Amount;
						workOrderReworkSubProductModel.WarehouseModel = new WarehouseModel
						{
							Name = item.WarehouseName,
							Number = item.WarehouseNumber,
							IsSelected = false
						};

						WorkOrderReworkBasketModel.WorkOrderReworkSubProducts.Add(workOrderReworkSubProductModel);
					}
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

	private async Task LoadLocationTransactionsAsync()
	{
		try
		{
			_userDialogs.ShowLoading("Load LocationTransaction Items...");
			LocationTransactions.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.MainItemReferenceId : WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.ReferenceId,
				variantReferenceId: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.ReferenceId : 0,
				warehouseNumber: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.WarehouseNumber,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					GroupLocationTransactionModel model = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
					if (model != null)
					{
						if (WorkOrderReworkBasketModel.MainProductLocationTransactions.Any(x => x.LocationCode == model.LocationCode))
						{
							if (WorkOrderReworkBasketModel.MainProductLocationTransactions is null)
							{
								WorkOrderReworkBasketModel.MainProductLocationTransactions = new();
							}

							model.OutputQuantity = WorkOrderReworkBasketModel.MainProductLocationTransactions.FirstOrDefault(x => x.LocationReferenceId == model.LocationReferenceId).OutputQuantity;
						}
					}
					LocationTransactions.Add(model);
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadMoreLocationTransactionsAsync()
	{
		if (IsBusy)
			return;
		if (LocationTransactions.Count < 18)
			return;
		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Load More Location Items...");


			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.MainItemReferenceId : WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.ReferenceId,
				variantReferenceId: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.IsVariant ? WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.ReferenceId : 0,
				warehouseNumber: WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.WarehouseNumber,
				skip: LocationTransactions.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					GroupLocationTransactionModel model = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
					if (model != null)
					{
						if (WorkOrderReworkBasketModel.MainProductLocationTransactions.Any(x => x.LocationCode == model.LocationCode))
						{
							if (WorkOrderReworkBasketModel.MainProductLocationTransactions is null)
							{
								WorkOrderReworkBasketModel.MainProductLocationTransactions = new();
							}

							model.OutputQuantity = WorkOrderReworkBasketModel.MainProductLocationTransactions.FirstOrDefault(x => x.LocationReferenceId == model.LocationReferenceId).OutputQuantity;
						}
					}
					LocationTransactions.Add(model);
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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


	private async Task IncreaseAsync()
	{
		if (IsBusy)
			return;
		if (WorkOrderReworkBasketModel is null)
			return;
		try
		{
			IsBusy = true;

			if (WorkOrderReworkBasketModel.BOMQuantity >= WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.StockQuantity)
			{
				await _userDialogs.AlertAsync($"Stok miktarı {WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.StockQuantity} kadar miktarı arttırabilirsiniz.", "Uyarı", "Tamam");
				return;
			}

			if (WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.LocTracking == 1)
			{
				await LoadLocationTransactionsAsync();
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
			}
			else
			{
				WorkOrderReworkBasketModel.BOMQuantity += 1;

				foreach (var subProduct in WorkOrderReworkBasketModel.WorkOrderReworkSubProducts)
				{
					subProduct.SubAmount = subProduct.ProductModel.Amount * WorkOrderReworkBasketModel.BOMQuantity;
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
	private async Task DecreaseAsync()
	{
		if (IsBusy)
			return;
		if (WorkOrderReworkBasketModel is null)
			return;
		try
		{
			IsBusy = true;

			if (WorkOrderReworkBasketModel.BOMQuantity > 0)
			{
				if (WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.LocTracking == 1)
				{
					await LoadLocationTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				else
				{
					WorkOrderReworkBasketModel.BOMQuantity -= 1;

					foreach (var subProduct in WorkOrderReworkBasketModel.WorkOrderReworkSubProducts)
					{
						subProduct.SubAmount = subProduct.ProductModel.Amount * WorkOrderReworkBasketModel.BOMQuantity;
					}
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

	private async Task LocationTransactionIncreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var locationTransactionTotalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => x.OutputQuantity);

			if (item.RemainingQuantity <= item.OutputQuantity)
				return;

			if (item.OutputQuantity >= WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.StockQuantity)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await _userDialogs.AlertAsync($"Arttırabileceğiniz miktar ana ürünün stok miktarını {WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.StockQuantity} geçmemelidir!", "Uyarı", "Tamam");

				return;
			}

			if (item.RemainingQuantity > item.OutputQuantity && WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.StockQuantity > item.OutputQuantity)
			{
				item.OutputQuantity += 1;
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

	private async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item.OutputQuantity > 0 && item.RemainingQuantity >= item.OutputQuantity)
			{
				item.OutputQuantity -= 1;
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

	private async Task LocationTransactionConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (LocationTransactions.Count <= 0)
			{
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
				return;
			}

			var confirm = await _userDialogs.ConfirmAsync("Onaylamak istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			var locationTransacionTotalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => x.OutputQuantity);

			foreach(var locationTransaction in LocationTransactions.Where(x => x.OutputQuantity <= 0)) {
				if(WorkOrderReworkBasketModel.MainProductLocationTransactions.Any(x=> x.LocationCode == locationTransaction.LocationCode))
				{
					WorkOrderReworkBasketModel.MainProductLocationTransactions?.Remove(WorkOrderReworkBasketModel.MainProductLocationTransactions?.FirstOrDefault(x => x.LocationCode == locationTransaction.LocationCode));
				}
			}

			foreach(var locationTransaction in LocationTransactions.Where(x => x.OutputQuantity > 0))
			{
				if(WorkOrderReworkBasketModel.MainProductLocationTransactions.Any(x => x.LocationCode == locationTransaction.LocationCode))
				{
					WorkOrderReworkBasketModel.MainProductLocationTransactions.FirstOrDefault(x => x.LocationCode == locationTransaction.LocationCode).OutputQuantity = locationTransaction.OutputQuantity;
				}
				else
				{
					WorkOrderReworkBasketModel.MainProductLocationTransactions.Add(locationTransaction);
				}
			}

			WorkOrderReworkBasketModel.BOMQuantity = locationTransacionTotalOutputQuantity * WorkOrderReworkBasketModel.WorkOrderReworkMainProductModel.Amount;

			foreach(var subProduct in WorkOrderReworkBasketModel.WorkOrderReworkSubProducts)
			{
				subProduct.SubAmount = WorkOrderReworkBasketModel.BOMQuantity * subProduct.ProductModel.Amount;
				subProduct.SubBOMQuantity = 0;
				subProduct.Locations.Clear();
			}


			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
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

	private async Task SubProductIncreaseAsync(WorkOrderReworkSubProductModel subProduct)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (subProduct.ProductModel.LocTracking == 1)
			{
				var locationViewModel = _serviceProvider.GetRequiredService<WorkOrderReworkProcessSubProductLocationListViewModel>();

				locationViewModel.WorkOrderReworkSubProductModel = subProduct;

				await locationViewModel.LoadSelectedItemsAsync();

				await Shell.Current.GoToAsync($"{nameof(WorkOrderReworkProcessSubProductLocationListView)}", new Dictionary<string, object>
				{
					[nameof(WorkOrderReworkSubProductModel)] = subProduct
				});
			}
			else
			{
				subProduct.SubBOMQuantity += 1;
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

	private async Task SubProductDecreaseAsync(WorkOrderReworkSubProductModel subProduct)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (subProduct.SubBOMQuantity > 0)
			{
				if (subProduct.ProductModel.LocTracking == 1)
				{
					var locationViewModel = _serviceProvider.GetRequiredService<WorkOrderReworkProcessSubProductLocationListViewModel>();

					locationViewModel.WorkOrderReworkSubProductModel = subProduct;

					await locationViewModel.LoadSelectedItemsAsync();

					await Shell.Current.GoToAsync($"{nameof(WorkOrderReworkProcessSubProductLocationListView)}", new Dictionary<string, object>
					{
						[nameof(WorkOrderReworkSubProductModel)] = subProduct
					});
				}
				else
				{
					subProduct.SubBOMQuantity -= 1;
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


	private async Task LocationTransactionCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Verileriniz silinecektir. Devam etmek istiyor musunuz?", "İptal", "Evet", "Hayır");

			if (!confirm)
				return;

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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(WorkOrderReworkBasketModel.BOMQuantity == 0)
			{
				await _userDialogs.AlertAsync($"Ana ürünün miktarı 0 olamaz. Lütfen miktarı arttırınız", "Uyarı", "Tamam");
				return;
			}

			bool canNext = true;

			foreach(var subProduct in WorkOrderReworkBasketModel.WorkOrderReworkSubProducts)
			{
				if(subProduct.SubBOMQuantity != WorkOrderReworkBasketModel.BOMQuantity)
				{
					canNext = false;
					await _userDialogs.AlertAsync($" {subProduct.ProductModel.Code} alt ürününün miktarı ana ürünün miktarı {WorkOrderReworkBasketModel.BOMQuantity} ile aynı olmalıdır.", "Uyarı", "Tamam");
				}
			}

			if(canNext)
			{
				await Shell.Current.GoToAsync($"{nameof(WorkOrderReworkProcessFormView)}", new Dictionary<string, object>
				{
					[nameof(WorkOrderReworkBasketModel)] = WorkOrderReworkBasketModel
				});
			}
		}
		catch (Exception ex) 
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

}
