using Android.Views.Accessibility;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;

public partial class AllFicheListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IProductPanelService _productPanelService;
	private readonly IUserDialogs _userDialogs;

	public ObservableCollection<ProductFiche> Items { get; } = new();
	public ObservableCollection<ProductTransaction> Transactions { get; } = new();

	[ObservableProperty]
	ProductFiche? selectedFiche;
	public AllFicheListViewModel(IHttpClientService httpClientService, ICountingPanelService countingPanelService, IUserDialogs userDialogs, IProductPanelService productPanelService)
	{
		_httpClientService = httpClientService;
		_productPanelService = productPanelService;
		_userDialogs = userDialogs;

		Title = "Malzeme Fişleri";

		BackCommand = new Command(async () => await BackAsync());
		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<ProductFiche>(async (product) => await ItemTappedAsync(product));

		LoadMoreTransactionsCommand = new Command(async () => await LoadMoreTransactionsAsync());
	}
	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command BackCommand { get; }

	public Command LoadMoreTransactionsCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			Items.Clear();
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _productPanelService.GetAllFiches(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: string.Empty,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{

						Items.Add(Mapping.Mapper.Map<ProductFiche>(item));
					}
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

			_userDialogs.ShowLoading("Yükleniyor...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _productPanelService.GetAllFiches(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: string.Empty,
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{

						Items.Add(Mapping.Mapper.Map<ProductFiche>(item));
					}
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

	private async Task ItemTappedAsync(ProductFiche productFiche)
	{
		if (productFiche is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedFiche = productFiche;

			_userDialogs.Loading("Load Transactions");
			await Task.Delay(500);
			await LoadTransactionsAsync(productFiche);

			CurrentPage.FindByName<BottomSheet>("ficheTransactionsBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task LoadTransactionsAsync(ProductFiche productFiche)
	{
		try
		{
			Transactions.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productPanelService.GetTransactionsByFiche(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				ficheReferenceId: SelectedFiche.ReferenceId,
				skip: 0,
				take: 20
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
					Transactions.Add(Mapping.Mapper.Map<ProductTransaction>(item));
                }
            }

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadMoreTransactionsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Load More Transactions...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productPanelService.GetTransactionsByFiche(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				ficheReferenceId: SelectedFiche.ReferenceId,
				skip: Transactions.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Transactions.Add(Mapping.Mapper.Map<ProductTransaction>(item));
				}
			}

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
