using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
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

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

[QueryProperty(name: nameof(CustomerDetailModel), queryId: nameof(CustomerDetailModel))]
public partial class CustomerDetailAllFichesViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerDetailAllFichesService _customerDetailAllFichesService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private CustomerDetailModel customerDetailModel = null!;

    [ObservableProperty]
    public SalesFiche selectedItem;

    public ObservableCollection<SalesFiche> Items { get; } = new();

    public ObservableCollection<SalesTransactionModel> Transactions { get; } = new();

    public CustomerDetailAllFichesViewModel(IHttpClientService httpClientService, ICustomerDetailAllFichesService customerDetailAllFichesService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _customerDetailAllFichesService = customerDetailAllFichesService;
        _userDialogs = userDialogs;

        Title = "Müşteri Hareketleri";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<SalesFiche>(async (item) => await ItemTappedAsync(item));
        LoadMoreTransactionsCommand = new Command(async () => await LoadMoreFicheTransactionsAsync());
        BackCommand = new Command(async () => await BackAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command LoadMoreTransactionsCommand { get; }
    public Command BackCommand { get; }

    public Page CurrentPage { get; set; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _customerDetailAllFichesService.GetAllFiches(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
                search: "", 
                skip: 0, 
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

				foreach (var item in result.Data)
                {   
                    Items.Add(Mapping.Mapper.Map<SalesFiche>(item));   
                }
            }

            _userDialogs.HideHud();
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

			_userDialogs.ShowLoading("Loading More...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _customerDetailAllFichesService.GetAllFiches(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: CustomerDetailModel.Customer.ReferenceId,
				search: "",
				skip: Items.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<SalesFiche>(item));
				}
			}

			_userDialogs.HideHud();
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

    private async Task ItemTappedAsync(SalesFiche salesFiche)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedItem = salesFiche;

			_userDialogs.ShowLoading("Yükleniyor...");
			await LoadTransactionsAsync(salesFiche);
            await Task.Delay(500);
            CurrentPage.FindByName<BottomSheet>("ficheTransactionsBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task LoadTransactionsAsync(SalesFiche salesFiche)
    {
        try
        {
            Transactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _customerDetailAllFichesService.GetTransactionsByFiche(httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: salesFiche.ReferenceId, 
                skip: Transactions.Count, 
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Transactions.Add(Mapping.Mapper.Map<SalesTransactionModel>(item));
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task LoadMoreFicheTransactionsAsync()
    {
        if (Transactions.Count < 18)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;


			_userDialogs.ShowLoading("Yükleniyor...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _customerDetailAllFichesService.GetTransactionsByFiche(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: SelectedItem.ReferenceId,
                skip: Transactions.Count,
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Transactions.Add(Mapping.Mapper.Map<SalesTransactionModel>(item));
            }

            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
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