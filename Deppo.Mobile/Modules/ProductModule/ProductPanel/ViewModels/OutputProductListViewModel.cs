using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using Org.Apache.Http.Conn.Params;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

public partial class OutputProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IProductPanelService _productPanelService;
	private readonly IUserDialogs _userDialogs;

	public ObservableCollection<ProductModel> Items { get; } = new();
	public ObservableCollection<ProductTransaction> Transactions { get; } = new();

	[ObservableProperty]
	private ProductModel? selectedProduct;

	public OutputProductListViewModel(IProductPanelService productPanelService, IHttpClientService httpClientService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_productPanelService = productPanelService;
		_userDialogs = userDialogs;

		Title = "Malzeme Çıkış Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		BackCommand = new Command(async () => await BackAsync());
		ItemTappedCommand = new Command<ProductModel>(async (product) => await ItemTappedAsync(product));
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command BackCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Load Items...");
			await Task.Delay(1000);
			Items.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productPanelService.GetOutputProduct(
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
					var obj = Mapping.Mapper.Map<ProductModel>(item);
					Items.Add(obj);
				}
			}

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
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Load More Items...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _productPanelService.GetOutputProduct(
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

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<ProductModel>(item);
					Items.Add(obj);
				}
			}

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

	private async Task ItemTappedAsync(ProductModel product)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedProduct = product;

			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			await GetLastCountingTransactionsAsync(SelectedProduct);

			CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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
	private async Task GetLastCountingTransactionsAsync(ProductModel productModel)
	{
		try
		{
			Transactions.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productPanelService.GetOutputTransactions(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: productModel.ReferenceId
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<ProductTransaction>(item);
						Transactions.Add(obj);
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			if (Items.Count > 0)
			{
				Items.Clear();
			}
			if (Transactions.Count > 0)
			{
				Transactions.Clear();
			}


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

}
