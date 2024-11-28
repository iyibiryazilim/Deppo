using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.QueryHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

[QueryProperty(name: nameof(SupplierDetailModel), queryId: nameof(SupplierDetailModel))]
public partial class SupplierOutputTransactionViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISupplierDetailOutputProductService _supplierDetailOutputProductService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    SupplierDetailModel supplierDetailModel = null!;

	public ObservableCollection<ProductModel> Items { get; } = new();

	[ObservableProperty]
	public SearchBar searchText;

	public SupplierOutputTransactionViewModel(IHttpClientService httpClientService, ISupplierDetailOutputProductService supplierDetailOutputProductService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _supplierDetailOutputProductService = supplierDetailOutputProductService;
        _userDialogs = userDialogs;
        Title = "Tedarikçi Çıkış Hareketleri";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        GoToBackCommand = new Command(async () => await GoToBackAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }
    public Command GoToBackCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Load Items");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _supplierDetailOutputProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId,
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
					Items.Add(Mapping.Mapper.Map<ProductModel>(item));
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata", "Tamam");

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

			_userDialogs.Loading("Load More Items");


			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _supplierDetailOutputProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId,
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
					Items.Add(Mapping.Mapper.Map<ProductModel>(item));
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata", "Tamam");

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

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _supplierDetailOutputProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				Items.Clear();
				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<ProductModel>(item));
				}
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

	private async Task GoToBackAsync()
    {
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync("..");
			SearchText.Text = "";
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }
}