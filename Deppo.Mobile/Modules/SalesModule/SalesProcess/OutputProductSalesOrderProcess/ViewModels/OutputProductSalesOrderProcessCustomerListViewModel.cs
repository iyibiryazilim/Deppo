using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesOrderProcessCustomerListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;
	private readonly IUserDialogs _userDialogs;
	public OutputProductSalesOrderProcessCustomerListViewModel(IHttpClientService httpClientService, IWaitingSalesOrderService waitingSalesOrderService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_waitingSalesOrderService = waitingSalesOrderService;
		_userDialogs = userDialogs;

		Title = "Müşteri seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<SalesCustomer>(async (customer) => await ItemTappedAsync(customer));
		NextViewCommand = new Command(async () => await NextViewAsync());
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command NextViewCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<SalesCustomer> Items { get; } = new();
	public ObservableCollection<WaitingSalesOrder> SalesOrders { get; } = new();

	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer? selectedSalesCustomer;
	#endregion

	private async Task GetSalesOrders(int skip = 0, int take = 20)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _waitingSalesOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, skip: skip, take: take);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<WaitingSalesOrder>(item);
					SalesOrders.Add(obj);
				}
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message);
		}

	}

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await GetSalesOrders(skip: 0, take: 20);
			if (SalesOrders.Count > 0)
			{
				var groupByCustomer = SalesOrders.GroupBy(x => x.CustomerReferenceId);
				foreach (var item in groupByCustomer)
				{
					SalesCustomer salesCustomer = new();
					salesCustomer.ReferenceId = item.Key;
					salesCustomer.Code = item.FirstOrDefault().CustomerCode;
					salesCustomer.Name = item.FirstOrDefault().CustomerName;
					salesCustomer.ProductReferenceCount = item.GroupBy(x => x.ProductReferenceId).Count();

					var groupByProduct = item.ToList().GroupBy(x => x.ProductReferenceId);
					salesCustomer.Products = new();

					foreach (var product in groupByProduct)
					{
						SalesCustomerProduct salesCustomerProduct = new();
						salesCustomerProduct.ReferenceId = product.Key;
						salesCustomerProduct.ItemReferenceId = product.FirstOrDefault().ProductReferenceId;
						salesCustomerProduct.ItemCode = product.FirstOrDefault().ProductCode;
						salesCustomerProduct.ItemName = product.FirstOrDefault().ProductName;
						salesCustomerProduct.IsVariant = product.FirstOrDefault().IsVariant;
						salesCustomerProduct.ShippedQuantity = product.Sum(x => x.ShippedQuantity);
						salesCustomerProduct.WaitingQuantity = product.Sum(x => x.WaitingQuantity);
						salesCustomerProduct.Quantity = product.Sum(x => x.Quantity);

						salesCustomer.Products.Add(salesCustomerProduct);

						salesCustomerProduct.Orders.AddRange(product.ToList());
					}

					Items.Add(salesCustomer);
				}
			}


			Console.WriteLine(Items);

			_userDialogs.Loading().Hide();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
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
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await GetSalesOrders(skip: SalesOrders.Count, take: 20);
			if (SalesOrders.Count > 0)
			{

				var groupByCustomer = SalesOrders.GroupBy(x => x.CustomerReferenceId);
				foreach (var item in groupByCustomer)
				{
					SalesCustomer salesCustomer = new();
					salesCustomer.ReferenceId = item.Key;
					salesCustomer.Code = item.FirstOrDefault().CustomerCode;
					salesCustomer.Name = item.FirstOrDefault().CustomerName;
					salesCustomer.ProductReferenceCount = item.GroupBy(x => x.ProductReferenceId).Count();

					var groupByProduct = item.ToList().GroupBy(x => x.ProductReferenceId);
					salesCustomer.Products = new();

					foreach (var product in groupByProduct)
					{
						SalesCustomerProduct salesCustomerProduct = new();
						salesCustomerProduct.ReferenceId = product.Key;
						salesCustomerProduct.ItemReferenceId = product.FirstOrDefault().ProductReferenceId;
						salesCustomerProduct.ItemCode = product.FirstOrDefault().ProductCode;
						salesCustomerProduct.ItemName = product.FirstOrDefault().ProductName;
						salesCustomerProduct.IsVariant = product.FirstOrDefault().IsVariant;
						salesCustomerProduct.ShippedQuantity = product.Sum(x => x.ShippedQuantity);
						salesCustomerProduct.WaitingQuantity = product.Sum(x => x.WaitingQuantity);
						salesCustomerProduct.Quantity = product.Sum(x => x.Quantity);

						salesCustomer.Products.Add(salesCustomerProduct);

						salesCustomerProduct.Orders.AddRange(product.ToList());
					}

					Items.Add(salesCustomer);
				}
			}

			Console.WriteLine(Items);

			_userDialogs.Loading().Hide();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ItemTappedAsync(SalesCustomer item)
	{
		try
		{
			IsBusy = true;

			if (SelectedSalesCustomer == item)
			{
				SelectedSalesCustomer.IsSelected = false;
				SelectedSalesCustomer = null;
			}
			else
			{
				if (SelectedSalesCustomer is not null)
				{
					SelectedSalesCustomer.IsSelected = false;
				}
				SelectedSalesCustomer = item;
				SelectedSalesCustomer.IsSelected = true;
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedSalesCustomer is not null)
			{
				await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessProductListView)}", new Dictionary<string, object>
				{
					[nameof(SalesCustomer)] = SelectedSalesCustomer,
					[nameof(WarehouseModel)] = WarehouseModel,
				});
			}
			else
			{
				_userDialogs.Alert("Lütfen bir müşteri seçiniz.", "Hata", "Tamam");
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
}
