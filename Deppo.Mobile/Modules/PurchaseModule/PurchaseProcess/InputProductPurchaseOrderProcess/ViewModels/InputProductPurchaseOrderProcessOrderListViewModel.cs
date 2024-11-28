using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
public partial class InputProductPurchaseOrderProcessOrderListViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;
    private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private WarehouseModel warehouseModel;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier;

    [ObservableProperty]
    public SearchBar searchText;

    public ObservableCollection<WaitingPurchaseOrderModel> Items { get; } = new();

    public ObservableCollection<WaitingPurchaseOrderModel> SelectedItems { get; } = new();

    [ObservableProperty]
    public ObservableCollection<InputPurchaseBasketModel> basketItems = new();

    public InputProductPurchaseOrderProcessOrderListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWaitingPurchaseOrderService waitingPurchaseOrderService, IServiceProvider serviceProvider)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _waitingPurchaseOrderService = waitingPurchaseOrderService;
        _serviceProvider = serviceProvider;

        Title = "Sipariş Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WaitingPurchaseOrderModel>(async (item) => await ItemTappedAsync(item));
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        CloseCommand = new Command(async () => await CloseAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<WaitingPurchaseOrderModel> ItemTappedCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command ConfirmCommand { get; }
    public Command CloseCommand { get; }

    private async Task LoadItemsAsync()
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

            var result = await _waitingPurchaseOrderService.GetObjects(httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                supplierReferenceId: PurchaseSupplier.ReferenceId,
                search: SearchText.Text,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        var order = Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item);
                        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == order.ReferenceId);
                        if (matchedItem is not null)
                            order.IsSelected = matchedItem.IsSelected;
                        else
                            order.IsSelected = false;

                        Items.Add(order);
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
        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading More Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _waitingPurchaseOrderService.GetObjects(httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                supplierReferenceId: PurchaseSupplier.ReferenceId,
                search: SearchText.Text,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        var order = Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item);
                        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == order.ReferenceId);
                        if (matchedItem is not null)
                            order.IsSelected = matchedItem.IsSelected;
                        else
                            order.IsSelected = false;

                        Items.Add(order);
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

            var result = await _waitingPurchaseOrderService.GetObjects(httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                supplierReferenceId: PurchaseSupplier.ReferenceId,
                search: SearchText.Text,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        var order = Mapping.Mapper.Map<WaitingPurchaseOrderModel>(item);
                        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == order.ReferenceId);
                        if (matchedItem is not null)
                            order.IsSelected = matchedItem.IsSelected;
                        else
                            order.IsSelected = false;

                        Items.Add(order);
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

    private async Task PerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }

    private async Task ItemTappedAsync(WaitingPurchaseOrderModel waitingPurchaseOrderModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == waitingPurchaseOrderModel.ReferenceId);
            if (selectedItem is not null)
            {
                if (selectedItem.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ReferenceId == waitingPurchaseOrderModel.ReferenceId).IsSelected = false;
                    SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId));
                    BasketItems.Remove(BasketItems.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ProductReferenceId));
                }
                else
                {
                    Items.FirstOrDefault(x => x.ReferenceId == waitingPurchaseOrderModel.ReferenceId).IsSelected = true;
                    SelectedItems.Add(selectedItem);

                    var inputPurchaseBasketModelItem = new InputPurchaseBasketModel
                    {
                        ItemReferenceId = selectedItem.ProductReferenceId,
                        ItemCode = selectedItem.ProductCode,
                        ItemName = selectedItem.ProductName,
                        IsVariant = selectedItem.IsVariant,
                        UnitsetReferenceId = selectedItem.UnitsetReferenceId,
                        UnitsetCode = selectedItem.UnitsetCode,
                        UnitsetName = selectedItem.UnitsetName,
                        SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
                        SubUnitsetCode = selectedItem.SubUnitsetCode,
                        SubUnitsetName = selectedItem.SubUnitsetName,
                        MainItemReferenceId = selectedItem.ProductReferenceId,
                        MainItemCode = selectedItem.ProductCode,
                        MainItemName = selectedItem.ProductName,
                        StockQuantity = default,
                        IsSelected = false,
                        TrackingType = selectedItem.TrackingType,
                        LocTracking = selectedItem.LocTracking,
                        Image = selectedItem.ImageData,
                        Quantity = selectedItem.WaitingQuantity,
                        LocTrackingIcon = selectedItem.LocTrackingIcon,
                        TrackingTypeIcon = selectedItem.TrackingTypeIcon,
                        VariantIcon = selectedItem.VariantIcon,
                    };

                    if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
                        inputPurchaseBasketModelItem.InputQuantity = 0;
                    else
                        inputPurchaseBasketModelItem.InputQuantity = 1;

                    BasketItems.Add(inputPurchaseBasketModelItem);
                }
            }
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

    private async Task ConfirmAsync()
    {
        if (IsBusy)
            return;
        try
        {
            var previousViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();

            if (previousViewModel is not null)
            {
                if (BasketItems.Any())
                {
                    _userDialogs.Loading("Siparişler sepete ekleniyor...");

                    foreach (var basketItem in BasketItems)
                    {
                        if (!previousViewModel.Items.Any(x => x.ItemReferenceId == basketItem.ItemReferenceId))
                        {
                            previousViewModel.Items.Add(basketItem);
                            basketItem.Orders.Clear();
                            foreach (var order in SelectedItems)
                            {
                                basketItem.Orders.Add(new InputPurchaseBasketOrderModel
                                {
									ShippedQuantity = order.ShippedQuantity,
									DueDate = order.DueDate,
									OrderReferenceId = order.OrderReferenceId,
									SupplierCode = order.SupplierCode,
									SupplierName = order.SupplierName,
									SupplierReferenceId = order.SupplierReferenceId,
									OrderDate = order.OrderDate,
									ProductCode = order.ProductCode,
									ProductName = order.ProductName,
									ProductReferenceId = order.ProductReferenceId,
									SubUnitsetCode = order.SubUnitsetCode,
									SubUnitsetName = order.SubUnitsetName,
									SubUnitsetReferenceId = order.SubUnitsetReferenceId,
									UnitsetReferenceId = order.UnitsetReferenceId,
									UnitsetCode = order.UnitsetCode,
									UnitsetName = order.UnitsetName,
									ReferenceId = order.ReferenceId,
									WaitingQuantity = order.WaitingQuantity,
									Quantity = order.Quantity,
								});
                            }
                        }
                        else
                        {
							var order = SelectedItems.FirstOrDefault(x => x.ReferenceId == basketItem.OrderReferenceId);
							if (order != null)
							{
								foreach (var item in previousViewModel.Items)
								{
									var existingOrder = item.Orders.FirstOrDefault(x => x.ReferenceId == order.ReferenceId);
									if (existingOrder == null)
									{
										item.Orders.Add(new InputPurchaseBasketOrderModel
										{
											ShippedQuantity = order.ShippedQuantity,
											DueDate = order.DueDate,
											OrderReferenceId = order.OrderReferenceId,
											SupplierCode = order.SupplierCode,
											SupplierName = order.SupplierName,
											SupplierReferenceId = order.SupplierReferenceId,
											OrderDate = order.OrderDate,
											ProductCode = order.ProductCode,
											ProductName = order.ProductName,
											ProductReferenceId = order.ProductReferenceId,
											SubUnitsetCode = order.SubUnitsetCode,
											SubUnitsetName = order.SubUnitsetName,
											SubUnitsetReferenceId = order.SubUnitsetReferenceId,
											UnitsetReferenceId = order.UnitsetReferenceId,
											UnitsetCode = order.UnitsetCode,
											UnitsetName = order.UnitsetName,
											ReferenceId = order.ReferenceId,
											WaitingQuantity = order.WaitingQuantity,
											Quantity = order.Quantity,
										});
									}

								}
							}
						}

						basketItem.Quantity = basketItem.Orders.Where(x => x.ProductReferenceId == basketItem.ItemReferenceId).Sum(x => x.WaitingQuantity);
					}
                    
                }

				SelectedItems.Clear();
				BasketItems.Clear();

				await Shell.Current.GoToAsync("..");
                _userDialogs.HideHud();
            }
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

    private async Task CloseAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (SelectedItems.Count > 0)
            {
                var confirm = await _userDialogs.ConfirmAsync("Seçmiş olduğunuz ürünler silinecektir. Devam etmek istiyor musunuz?", "Hata", "Evet", "Hayır");

                if (!confirm)
                    return;

                SelectedItems.Clear();
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.GoToAsync("..");
            }
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