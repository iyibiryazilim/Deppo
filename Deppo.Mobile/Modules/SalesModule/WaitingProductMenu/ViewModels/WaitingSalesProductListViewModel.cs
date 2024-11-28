using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Data.Async.Helpers;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.WaitingProductMenu.ViewModels;

public partial class WaitingSalesProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
	private readonly IWaitingSalesProductService _waitingSalesProductService;

	[ObservableProperty]
	WaitingProduct selectedItem;

	public ObservableCollection<WaitingProduct> Items { get; } = new();
	public ObservableCollection<WaitingSalesProductOrder> Orders { get; } = new();
	public WaitingSalesProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWaitingSalesProductService waitingSalesProductService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_waitingSalesProductService = waitingSalesProductService;

		Title = "Bekleyen Malzemeler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WaitingProduct>(async (product) => await ItemTappedAsync(product));

        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

    }

	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }

	public Command PerformSearchCommand { get; }

	public Command PerformEmptySearchCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;


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
			var result = await _waitingSalesProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: "",
				skip: 0,
				take: 20
			);

			if(result.IsSuccess)
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

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _waitingSalesProductService.GetObjects(
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
			if(_userDialogs.IsHudShowing)
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

			var result = await _waitingSalesProductService.GetOrderById(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: product.ProductReferenceId
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
					Orders.Add(Mapping.Mapper.Map<WaitingSalesProductOrder>(item));
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

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _waitingSalesProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (!result.IsSuccess)
            {
                _userDialogs.Alert(result.Message, "Hata");
                return;
            }

            Items.Clear();
            foreach (var item in result.Data)
                Items.Add(Mapping.Mapper.Map<WaitingProduct>(item));
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
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

}
