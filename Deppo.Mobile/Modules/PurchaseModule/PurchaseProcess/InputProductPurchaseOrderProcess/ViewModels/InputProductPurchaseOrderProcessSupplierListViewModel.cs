using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    [QueryProperty(name: nameof(InputProductProcessType), queryId: nameof(InputProductProcessType))]
    public partial class InputProductPurchaseOrderProcessSupplierListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ISupplierService _supplierService;
        private readonly IUserDialogs _userDialogs;
        private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;

        [ObservableProperty]
        private SupplierModel supplierModel = null!;

        [ObservableProperty]
        private WarehouseModel selectedWarehouseModel = null!;

        [ObservableProperty]
        private Supplier selectedSupplier = null!;

        [ObservableProperty]
        private InputProductProcessType inputProductProcessType;

        public InputProductPurchaseOrderProcessSupplierListViewModel(IHttpClientService httpClientService,
        ISupplierService supplierService,
        IUserDialogs userDialogs,
        IWaitingPurchaseOrderService waitingPurchaseOrderService)
        {
            _httpClientService = httpClientService;
            _supplierService = supplierService;
            _userDialogs = userDialogs;

            Title = "Tedarikçiler";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            PerformSearchCommand = new Command<SearchBar>(async (searchBar) => await PerformSearchAsync(searchBar));

            ItemTappedCommand = new Command<PurchaseSupplier>(async (supplier) => await ItemTappedAsync(supplier));
            NextViewCommand = new Command(async () => await NextViewAsync());
            _waitingPurchaseOrderService = waitingPurchaseOrderService;
        }

        //Collections

        public ObservableCollection<PurchaseSupplier> Items { get; } = new();

        public ObservableCollection<WaitingPurchaseOrder> PurchaseOrders { get; } = new();

        //Properties
        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private PurchaseSupplier? selectedPurchaseSupplier;

        // public ObservableCollection<SupplierModel> Items { get; } = new();

        // public ObservableCollection<SupplierModel> SelectedItems { get; } = new();
        public Command LoadItemsCommand { get; }

        public Command LoadMoreItemsCommand { get; }
        public Command<SearchBar> PerformSearchCommand { get; }

        public Command ItemTappedCommand { get; }

        public Command NextViewCommand { get; }

        public async Task GetSalesOrders(int skip = 0, int take = 20)
        {
            try
            {
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _waitingPurchaseOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, skip: skip, take: take);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<WaitingPurchaseOrder>(item);
                        PurchaseOrders.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
        }

        public async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading Items...");
                Items.Clear();
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await GetSalesOrders(skip: 0, take: 20);
                if (PurchaseOrders.Count > 0)
                {
                    var groupBySupplier = PurchaseOrders.GroupBy(x => x.SupplierReferenceId);
                    foreach (var item in groupBySupplier)
                    {
                        PurchaseSupplier purchaseSupplier = new();
                        purchaseSupplier.ReferenceId = item.Key;
                        purchaseSupplier.Code = item.FirstOrDefault().SupplierCode;
                        purchaseSupplier.Name = item.FirstOrDefault().SupplierName;
                        purchaseSupplier.ProductReferenceCount = item.GroupBy(x => x.ProductReferenceId).Count();

                        var groupByProduct = item.ToList().GroupBy(x => x.ProductReferenceId);
                        purchaseSupplier.Products = new();

                        foreach (var product in groupByProduct)
                        {
                            PurchaseSupplierProduct purchaseSupplierProduct = new();
                            purchaseSupplierProduct.ReferenceId = product.Key;
                            purchaseSupplierProduct.ItemReferenceId = product.FirstOrDefault().ProductReferenceId;
                            purchaseSupplierProduct.ItemCode = product.FirstOrDefault().ProductCode;
                            purchaseSupplierProduct.ItemName = product.FirstOrDefault().ProductName;
                            purchaseSupplierProduct.IsVariant = product.FirstOrDefault().IsVariant;
                            purchaseSupplierProduct.ShippedQuantity = product.Sum(x => x.ShippedQuantity);
                            purchaseSupplierProduct.WaitingQuantity = product.Sum(x => x.WaitingQuantity);
                            purchaseSupplierProduct.Quantity = product.Sum(x => x.Quantity);

                            purchaseSupplier.Products.Add(purchaseSupplierProduct);

                            purchaseSupplierProduct.Orders.AddRange(product.ToList());
                        }

                        Items.Add(purchaseSupplier);
                    }
                }

                Console.WriteLine(Items);

                _userDialogs.Loading().Hide();
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

        public async Task LoadMoreItemsAsync()
        {
            //if (IsBusy)
            //    return;

            //try
            //{
            //    IsBusy = true;
            //    _userDialogs.Loading("Refreshing Items...");
            //    var httpClient = _httpClientService.GetOrCreateHttpClient();
            //    var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
            //    if (result.IsSuccess)
            //    {
            //        if (result.Data == null)
            //            return;

            //        foreach (var item in result.Data)
            //            Items.Add(Mapping.Mapper.Map<SupplierModel>(item));

            //        if (_userDialogs.IsHudShowing)
            //            _userDialogs.Loading().Hide();
            //    }
            //    else
            //    {
            //        if (_userDialogs.IsHudShowing)
            //            _userDialogs.Loading().Hide();

            //        _userDialogs.Alert(message: result.Message, title: "Load Items");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.Loading().Hide();

            //    _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }

        private async Task PerformSearchAsync(SearchBar searchBar)
        {
            if (IsBusy)
                return;

            try
            {
                if (string.IsNullOrWhiteSpace(searchBar.Text))
                {
                    await LoadItemsAsync();
                    searchBar.Unfocus();
                    return;
                }
                else
                {
                    if (searchBar.Text.Length >= 3)
                    {
                        IsBusy = true;
                        using (_userDialogs.Loading("Searching.."))
                        {
                            var httpClient = _httpClientService.GetOrCreateHttpClient();

                            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
                            if (!result.IsSuccess)
                            {
                                _userDialogs.Alert(result.Message, "Hata");
                                return;
                            }

                            Items.Clear();
                            foreach (var item in result.Data)
                                Items.Add(item);
                        }
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

        private async Task ItemTappedAsync(PurchaseSupplier item)
        {
            try
            {
                IsBusy = true;

                if (SelectedPurchaseSupplier == item)
                {
                    SelectedPurchaseSupplier.IsSelected = false;
                    SelectedPurchaseSupplier = null;
                }
                else
                {
                    if (SelectedPurchaseSupplier is not null)
                    {
                        SelectedPurchaseSupplier.IsSelected = false;
                    }
                    SelectedPurchaseSupplier = item;
                    SelectedPurchaseSupplier.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

                if (SelectedPurchaseSupplier is not null)
                {
                    await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessProductListView)}", new Dictionary<string, object>
                    {
                        [nameof(PurchaseSupplier)] = SelectedPurchaseSupplier,
                        [nameof(WarehouseModel)] = SelectedWarehouseModel,
                        [nameof(SelectedWarehouseModel)] = SelectedWarehouseModel,
                        [nameof(SelectedSupplier)] = SelectedSupplier,
                        [nameof(InputProductProcessType)] = InputProductProcessType
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
            }
        }
    }
}