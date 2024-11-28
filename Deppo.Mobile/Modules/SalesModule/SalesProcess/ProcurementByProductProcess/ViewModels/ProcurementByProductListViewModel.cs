using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ProcurementByProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementByProductService _procurementByProductService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

	public ObservableCollection<ProductOrderModel> Items { get; } = new();

	[ObservableProperty]
    ProductOrderModel? selectedProductOrderModel;

	[ObservableProperty]
	public SearchBar searchText;

	public ProcurementByProductListViewModel(
        IHttpClientService httpClientService,
        IProcurementByProductService procurementByProductService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementByProductService = procurementByProductService;
        _userDialogs = userDialogs;

        Title = "Sipariş Ürünü Seçiniz";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductOrderModel>(ItemTappedAsync);
		NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
	}
    

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<ProductOrderModel> ItemTappedCommand { get; }
    public Command BackCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
	public Command NextViewCommand { get; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementByProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
			    search: SearchText.Text,
				skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var productOrder = Mapping.Mapper.Map<ProductOrderModel>(item);
                        Items.Add(productOrder);
                    }
            }
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
        }
        finally
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

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
            var result = await _procurementByProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
				search: SearchText.Text,
				skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var productOrder = Mapping.Mapper.Map<ProductOrderModel>(item);
                        Items.Add(productOrder);
                    }
            }

        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
        }
        finally
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            IsBusy = false;
        }
    }

    private void ItemTappedAsync(ProductOrderModel productOrder)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (productOrder == SelectedProductOrderModel)
            {
                SelectedProductOrderModel.IsSelected = false;
                SelectedProductOrderModel = null;
            }
            else
            {
                if (SelectedProductOrderModel != null)
                {
                    SelectedProductOrderModel.IsSelected = false;
                }

                SelectedProductOrderModel = productOrder;
                SelectedProductOrderModel.IsSelected = true;
            }

        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
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

			if (SelectedProductOrderModel is not null)
			{
				await Shell.Current.GoToAsync($"{nameof(ProcurementByProductCustomerListView)}", new Dictionary<string, object>
				{
					[nameof(ProductOrderModel)] = SelectedProductOrderModel,
                    [nameof(WarehouseModel)] = WarehouseModel
				});
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
            SearchText.Text = string.Empty;
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
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _procurementByProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<ProductOrderModel>(product);
					item.IsSelected = SelectedProductOrderModel?.ItemReferenceId == item?.ItemReferenceId ? true : false;
					Items.Add(item);
				}
			}
			if (!result.IsSuccess)
			{
				_userDialogs.Alert(result.Message, "Hata");
				return;
			}
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
    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(SelectedProductOrderModel is not null)
            {
                SelectedProductOrderModel.IsSelected = false;
                SelectedProductOrderModel = null;
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
