using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
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

namespace Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;

public partial class CountingInputReferenceProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICountingPanelService _countingPanelService;
    private readonly IUserDialogs _userDialogs;
    

    public ObservableCollection<ProductModel> Items { get; } = new();

    public ObservableCollection<CountingTransaction> Transactions { get; } = new();


    [ObservableProperty]
    private ProductModel? selectedProduct;

    public CountingInputReferenceProductListViewModel(IHttpClientService httpClientService, ICountingPanelService countingPanelService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _countingPanelService = countingPanelService;
        _userDialogs = userDialogs;

        Title = "Artan Referans Ürün Listesi";

          BackCommand = new Command(async () => await BackAsync());
        ItemTappedCommand = new Command<ProductModel>(async (product) => await ItemTappedAsync(product));
          LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command BackCommand { get; }
    public Command ItemTappedCommand { get; }
    public Page CurrentPage { get; set; }



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

            var result = await _countingPanelService.GetInProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<ProductModel>(item));
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

            var result = await _countingPanelService.GetInProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<ProductModel>(item));
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
        catch (System.Exception)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.", "Hata", "Tamam");
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
            var result = await _countingPanelService.GetInCountingTransactions(
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
                        var obj = Mapping.Mapper.Map<CountingTransaction>(item);
                        Transactions.Add(obj);
                    }
                }
            }
        }
        catch (System.Exception ex)
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
            if(Items.Count > 0)
            {
                Items.Clear();
            }
            if(Transactions.Count > 0)
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
