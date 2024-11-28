using Android.Renderscripts;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

[QueryProperty(name: nameof(SelectedCustomers), queryId: nameof(SelectedCustomers))]
[QueryProperty(name: nameof(ProductOrderModel), queryId: nameof(ProductOrderModel))]
[QueryProperty(name: nameof(OrderWarehouse), queryId: nameof(OrderWarehouse))]
[QueryProperty(name: nameof(ProcurementWarehouse), queryId: nameof(ProcurementWarehouse))]

public partial class ProcurementByProductProcurableProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IProcurementByProductProcurableProductService _procurementByProductProcurableProductService;

	[ObservableProperty]
	ObservableCollection<CustomerOrderModel> selectedCustomers;

	[ObservableProperty]
	WarehouseModel orderWarehouse;

	[ObservableProperty]
	WarehouseModel procurementWarehouse;

	[ObservableProperty]
	ProductOrderModel productOrderModel;

	public ObservableCollection<ProcurementProductProcurableProductModel> Items { get; } = new();

	[ObservableProperty]
	SearchBar searchText;

	public ProcurementByProductProcurableProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProcurementByProductProcurableProductService procurementByProductProcurableProductService)
	{
		_httpClientService = httpClientService;
		_procurementByProductProcurableProductService = procurementByProductProcurableProductService;
		_userDialogs = userDialogs;

		Title = "Toplanabilir Ürün Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}
	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _procurementByProductProcurableProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: ProductOrderModel.ItemReferenceId,
				warehouseNumber: ProcurementWarehouse.Number,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
					var obj = Mapping.Mapper.Map<ProcurementProductProcurableProductModel>(item);
					obj.WaitingQuantity = ProductOrderModel.WaitingQuantity;
					obj.ShippedQuantity = ProductOrderModel.ShippedQuantity;
					obj.Quantity = ProductOrderModel.Quantity;
					Items.Add(obj);
                }
            }

			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;

		if (Items.Count < 18)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading More Items...");
			Items.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _procurementByProductProcurableProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: ProductOrderModel.ItemReferenceId,
				warehouseNumber: ProcurementWarehouse.Number,
				search: SearchText.Text,
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<ProcurementProductProcurableProductModel>(item));
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

	private async Task PerformSearchAsync()
	{
		if (IsBusy)
			return;
		try
		{
			if (string.IsNullOrWhiteSpace(SearchText.Text))
			{
				await LoadItemsAsync();
				SearchText.Unfocus();
				return;
			}
			IsBusy = true;

			Items.Clear();
			_userDialogs.Loading("Searching Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _procurementByProductProcurableProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: ProductOrderModel.ItemReferenceId,
				warehouseNumber: ProcurementWarehouse.Number,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<ProcurementProductProcurableProductModel>(item);
					obj.WaitingQuantity = ProductOrderModel.WaitingQuantity;
					obj.ShippedQuantity = ProductOrderModel.ShippedQuantity;
					obj.Quantity = ProductOrderModel.Quantity;
					Items.Add(obj);
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

	private async Task PerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(SearchText.Text))
		{
			await PerformSearchAsync();
		}
	}

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (Items.Count == 0)
			{
				await _userDialogs.AlertAsync("Herhangi bir ürününüz olmadığı için sonraki işlemlere devam edemezsiniz", "Uyarı", "Tamam");
				return;
			}

			var param = GetProcurementProductList(Items);

			if(param.ProcurementProductList.Count == 0)
			{
				await _userDialogs.AlertAsync("Toplanacak ürün bulunamadı", "Hata", "Tamam");
				return;
			}

			await Shell.Current.GoToAsync($"{nameof(ProcurementByProductBasketView)}", new Dictionary<string, object>
			{
				[nameof(ProcurementProductBasketModel)] = param
			});
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			SearchText.Text = string.Empty;
			IsBusy = false;
		}
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SearchText.Text = string.Empty;
			await Shell.Current.GoToAsync("..");
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}


	/// <summary>
	/// Kısmi veya tamamı toplanabilir ürünleri getirir.
	/// </summary>
	/// <param name="items"></param>
	/// <returns></returns>
	private ProcurementProductBasketModel GetProcurementProductList(ObservableCollection<ProcurementProductProcurableProductModel> items) {
		var procurementProductList = Items.Where(x => x.StockQuantity >= x.WaitingQuantity || x.StockQuantity > 0).ToList();

		ProcurementProductBasketModel procurementProductBasketModel = new();
		procurementProductBasketModel.OrderWarehouse = OrderWarehouse;
		procurementProductBasketModel.ProcurementWarehouse = ProcurementWarehouse;
		procurementProductBasketModel.SelectedCustomers = SelectedCustomers;
		procurementProductBasketModel.ProcurementProductList = procurementProductList;

		return procurementProductBasketModel;
	}
}
