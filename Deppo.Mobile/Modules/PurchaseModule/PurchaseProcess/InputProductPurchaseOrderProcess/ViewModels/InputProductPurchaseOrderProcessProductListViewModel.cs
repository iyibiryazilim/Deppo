using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    [QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
    public partial class InputProductPurchaseOrderProcessProductListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IProductService _productService;
        private readonly IUserDialogs _userDialogs;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;
        private readonly IPurchaseSupplierService _purchaseSupplierService;
        private readonly IPurchaseSupplierProductService _purchaseSupplierProductService;

        [ObservableProperty]
        private ObservableCollection<InputProductBasketModel> selectedProducts = new();

        public InputProductPurchaseOrderProcessProductListViewModel(IHttpClientService httpClientService,
        IProductService productService,

        IUserDialogs userDialogs,
        IServiceProvider serviceProvider,
        IWaitingPurchaseOrderService waitingPurchaseOrderService, IPurchaseSupplierService purchaseSupplierService, IPurchaseSupplierProductService purchaseSupplierProductService)
        {
            _httpClientService = httpClientService;
            _productService = productService;

            _userDialogs = userDialogs;
            _serviceProvider = serviceProvider;
            _waitingPurchaseOrderService = waitingPurchaseOrderService;
            _purchaseSupplierService = purchaseSupplierService;
            _purchaseSupplierProductService = purchaseSupplierProductService;

            Title = "Sipariş ve Ürün Listesi";
            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            SwitchToProductListViewCommand = new Command(SwitchToProductListViewAsync);
            SwitchToOrderListViewCommand = new Command(SwitchToOrderListViewAsync);
        }

        public Command SwitchToProductListViewCommand { get; }
        public Command SwitchToOrderListViewCommand { get; }
        public Command LoadItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command NextViewCommand { get; }

        public ObservableCollection<WaitingPurchaseOrder> Orders { get; } = new();
        public ObservableCollection<PurchaseSupplierProduct> Products { get; } = new();

        #region Properties

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private PurchaseSupplier purchaseSupplier = null!;

        [ObservableProperty]
        private bool isProductListVisible = false;

        [ObservableProperty]
        private bool isOrderListVisible = true;

        #endregion Properties

        private async void SwitchToProductListViewAsync()
        {
            IsProductListVisible = true;
            IsOrderListVisible = false;

            // Ürünler verilerini yükle
            await LoadItemsAsync();
        }

        private async void SwitchToOrderListViewAsync()
        {
            IsProductListVisible = false;
            IsOrderListVisible = true;

            // Sipariş verilerini yükle
            await LoadOrdersAsync();
        }

        private async Task LoadItemsAsync()
        {
            //try
            //{
            //    IsBusy = true;
            //    Products.Clear();
            //    _userDialogs.Loading("Loading Items...");
            //    var httpClient = _httpClientService.GetOrCreateHttpClient();
            //    var result = await _purchaseSupplierProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseSupplier.ReferenceId);

            //    if (result.Data is not null)
            //    {
            //        foreach (var item in result.Data)
            //        {
            //            var obj = Mapping.Mapper.Map<PurchaseSupplierProduct>(item);
            //            Products.Add(obj);
            //        }
            //    }
            //}
            //catch (Exception ex
            //)
            //{
            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.HideHud();

            //    _userDialogs.Alert(ex.Message);
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
            try
            {
                IsBusy = true;
                Products.Clear();
                _userDialogs.Loading("Loading Items...");
                await Task.Delay(1000);

                foreach (var item in PurchaseSupplier.Products)
                {
                    Products.Add(item);
                }

                _userDialogs.Loading().Hide();
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

        private async Task LoadOrdersAsync()
        {
            try
            {
                IsBusy = true;

                Orders.Clear();
                _userDialogs.Loading("Loading Items...");
                await Task.Delay(1000);

                foreach (var item in PurchaseSupplier.Products)
                {
                    if (item.Orders is not null)
                    {
                        foreach (var order in item.Orders)
                        {
                            Orders.Add(order);
                        }
                    }
                }

                _userDialogs.Loading().Hide();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}