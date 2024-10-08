using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
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

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

[QueryProperty(name: nameof(CustomerDetailModel), queryId: nameof(CustomerDetailModel))]
public partial class CustomerInputTransactionViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerDetailInputProductService _customerDetailInputProductService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private CustomerDetailModel customerDetailModel = null!;

    public CustomerInputTransactionViewModel(IHttpClientService httpClientService, ICustomerDetailInputProductService customerDetailInputProductService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _customerDetailInputProductService = customerDetailInputProductService;
        _userDialogs = userDialogs;

        Title = "Müşteri Giriş Hareketleri";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        GoToBackCommand = new Command(async () => await GoToBackAsync());
    }

    public ObservableCollection<ProductModel> Items { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
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

	private async Task GoToBackAsync()
    {
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync("..");
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