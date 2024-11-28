using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.WaitingProductMenu.ViewModels;

public partial class WaitingPurchaseProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IWaitingPurchaseProductService _waitingPurchaseProductService;

	public ObservableCollection<WaitingProduct> Items { get; } = new();
	public ObservableCollection<WaitingPurchaseProductOrder> Orders { get; } = new();

	[ObservableProperty]
	WaitingProduct selectedItem;
	public WaitingPurchaseProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWaitingPurchaseProductService waitingPurchaseProductService)
	{
		_httpClientService = httpClientService;
		_waitingPurchaseProductService = waitingPurchaseProductService;
		_userDialogs = userDialogs;

		Title = "Bekleyen Malzemeler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WaitingProduct>(async (product) => await ItemTappedAsync(product));
	}
	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }

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
			var result = await _waitingPurchaseProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: "",
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<WaitingProduct>(item));
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		if (Items.Count < 18)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _waitingPurchaseProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: "",
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				_userDialogs.Loading("Loading More Items...");

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<WaitingProduct>(item));
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

	private async Task ItemTappedAsync(WaitingProduct product)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = product;
			await LoadProductOrderItemsAsync(product);
			CurrentPage.FindByName<BottomSheet>("orderBottomSheet").State = BottomSheetState.HalfExpanded;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadProductOrderItemsAsync(WaitingProduct product)
	{
		try
		{
			_userDialogs.Loading("Loading Items...");
			Orders.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _waitingPurchaseProductService.GetOrderById(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: product.ProductReferenceId
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Orders.Add(Mapping.Mapper.Map<WaitingPurchaseProductOrder>(item));
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
}
