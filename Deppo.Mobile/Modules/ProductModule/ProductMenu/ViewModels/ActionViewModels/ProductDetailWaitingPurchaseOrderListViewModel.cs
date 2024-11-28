using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels
{
    [QueryProperty(nameof(ProductDetailModel), nameof(ProductDetailModel))]
    public partial class ProductDetailWaitingPurchaseOrderListViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;
        private readonly IProductDetailActionService _productDetailActionService;

        public ProductDetailWaitingPurchaseOrderListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IProductDetailActionService productDetailActionService)
        {
            _userDialogs = userDialogs;
            _httpClientService = httpClientService;
            _productDetailActionService = productDetailActionService;

            Title = "Bekleyen Satınalma Siparişleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            BackCommand = new Command(async () => await BackAsync());

        }


        [ObservableProperty]
        public ProductDetailModel productDetailModel;


        public ObservableCollection<WaitingPurchaseOrderModel> Items { get; } = new();

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command BackCommand { get; }

        public async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Items.Clear();

                _userDialogs.Loading("Loading Items...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();

                await Task.Delay(1000);



                var result = await _productDetailActionService.GetWaitingPurchaseOrders(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item));

                    }
                    _userDialogs.Loading().Hide();
                }
                else
                {
                    if (_userDialogs.IsHudShowing)
                        _userDialogs.Loading().Hide();

                    Debug.WriteLine(result.Message);
                    _userDialogs.Alert(message: result.Message, title: "Load Items");

                }
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadMoreItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                _userDialogs.Loading("Loading Items...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _productDetailActionService.GetWaitingPurchaseOrders(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item));

                    }
                    _userDialogs.Loading().Hide();
                }
                else
                {
                    if (_userDialogs.IsHudShowing)
                        _userDialogs.Loading().Hide();

                    _userDialogs.Alert(message: result.Message, title: "Load Items");
                }
            }
            catch (Exception ex)
            {

                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
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
}
