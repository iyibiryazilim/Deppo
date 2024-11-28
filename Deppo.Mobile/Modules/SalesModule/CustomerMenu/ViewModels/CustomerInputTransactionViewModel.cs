using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

[QueryProperty(name: nameof(CustomerDetailModel), queryId: nameof(CustomerDetailModel))]
public partial class CustomerInputTransactionViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ICustomerDetailInputProductService _customerDetailInputProductService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	private CustomerDetailModel customerDetailModel = null!;

	[ObservableProperty]
	public SearchBar searchText;

	public ObservableCollection<ProductModel> Items { get; } = new();
	public CustomerInputTransactionViewModel(IHttpClientService httpClientService, ICustomerDetailInputProductService customerDetailInputProductService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_customerDetailInputProductService = customerDetailInputProductService;
		_userDialogs = userDialogs;

		Title = "Müşteri Giriş Hareketleri";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		GoToBackCommand = new Command(async () => await GoToBackAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
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

			var result = await _customerDetailInputProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
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

			var result = await _customerDetailInputProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
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

			var result = await _customerDetailInputProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
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
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Task.Delay(300);
			await Shell.Current.GoToAsync("..");
			SearchText.Text = string.Empty;
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