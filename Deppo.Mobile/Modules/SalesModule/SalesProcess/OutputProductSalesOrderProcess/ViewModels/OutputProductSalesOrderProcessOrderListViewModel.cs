using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class OutputProductSalesOrderProcessOrderListViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;
    private readonly IWaitingSalesOrderService _waitingSalesOrderService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SalesCustomer salesCustomer = null!;

    [ObservableProperty]
    public SearchBar searchText;

    public ObservableCollection<WaitingSalesOrderModel> Items { get; } = new();

    public ObservableCollection<WaitingSalesOrderModel> SelectedItems { get; } = new();

    [ObservableProperty]
    public ObservableCollection<OutputSalesBasketModel> basketItems = new();

    public OutputProductSalesOrderProcessOrderListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWaitingSalesOrderService waitingSalesOrderService, IServiceProvider serviceProvider)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _waitingSalesOrderService = waitingSalesOrderService;
        _serviceProvider = serviceProvider;

        Title = "Sipariş Listesi";
        BasketItems.Clear();

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WaitingSalesOrderModel>(async (item) => await ItemTappedAsync(item));
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await OrderEmptySearchAsync());
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        CloseCommand = new Command(async () => await CloseAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<WaitingSalesOrderModel> ItemTappedCommand { get; }
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

            _userDialogs.ShowLoading("Loading Orders...");
            Items.Clear();
            SelectedItems.Clear();
			await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingSalesOrderService.GetObjects(httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                customerReferenceId: SalesCustomer.ReferenceId,
                shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId,
                SearchText.Text,
                0,
                20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<WaitingSalesOrderModel>(item);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == obj.ReferenceId);
                    if (matchingItem is not null)
                        obj.IsSelected = matchingItem.IsSelected;

                    Items.Add(obj);
                }
            }

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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading More Orders...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _waitingSalesOrderService.GetObjects(httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                customerReferenceId: SalesCustomer.ReferenceId,
                shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId,
                SearchText.Text,
                Items.Count,
                20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<WaitingSalesOrderModel>(item);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == obj.ReferenceId);
                    if (matchingItem is not null)
                        obj.IsSelected = matchingItem.IsSelected;

                    Items.Add(obj);
                }
            }

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

            var result = await _waitingSalesOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, customerReferenceId: SalesCustomer.ReferenceId, shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId, SearchText.Text, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<WaitingSalesOrderModel>(item);
                    var matchingItem = Items.FirstOrDefault(x => x.ReferenceId == obj.ReferenceId);
                    if (matchingItem is not null)
                        obj.IsSelected = matchingItem.IsSelected;

                    Items.Add(obj);
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

    private async Task OrderEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }

    private async Task ItemTappedAsync(WaitingSalesOrderModel waitingSalesOrderModel)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == waitingSalesOrderModel.ReferenceId);
            if (selectedItem is not null)
            {
                if (selectedItem.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ReferenceId == waitingSalesOrderModel.ReferenceId).IsSelected = false;
                    SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId));
                    BasketItems.Remove(BasketItems.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ProductReferenceId));
                }
                else
                {
                    Items.FirstOrDefault(x => x.ReferenceId == waitingSalesOrderModel.ReferenceId).IsSelected = true;
                    SelectedItems.Add(selectedItem);

                    var outputSalesBasketModelItem = new OutputSalesBasketModel
                    {
                        OrderReferenceId = selectedItem.OrderReferenceId,
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
                        outputSalesBasketModelItem.OutputQuantity = 0;
                    else
                        outputSalesBasketModelItem.OutputQuantity = 1;

                    BasketItems.Add(outputSalesBasketModelItem);
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
            IsBusy = true;

            var previousViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();

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
							foreach (var order in SelectedItems.Where(x => x.ProductReferenceId == basketItem.ItemReferenceId).ToList())
							{
								basketItem.Orders.Add(new OutputSalesBasketOrderModel
								{
									ShippedQuantity = order.ShippedQuantity,
									DueDate = order.DueDate,
									OrderReferenceId = order.OrderReferenceId,
									CustomerCode = order.CustomerCode,
									CustomerName = order.CustomerName,
									CustomerReferenceId = order.CustomerReferenceId,
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
                                    Price = order.Price,
                                    Vat = order.Vat,
									Quantity = order.Quantity,
								});
							}
                        }
                        else
                        {
                            var order = SelectedItems.FirstOrDefault(x=>x.ReferenceId == basketItem.OrderReferenceId);
                            if(order != null)
                            {
                                foreach (var item in previousViewModel.Items)
                                {
                                    var existingOrder = item.Orders.FirstOrDefault(x => x.ReferenceId == order.ReferenceId);
                                    if (existingOrder == null)
                                    {
										item.Orders.Add(new OutputSalesBasketOrderModel
										{
											ShippedQuantity = order.ShippedQuantity,
											DueDate = order.DueDate,
											OrderReferenceId = order.OrderReferenceId,
											CustomerCode = order.CustomerCode,
											CustomerName = order.CustomerName,
											CustomerReferenceId = order.CustomerReferenceId,
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
                                            Price = order.Price,
											Vat = order.Vat,
											WaitingQuantity = order.WaitingQuantity,
											Quantity = order.Quantity,
										});
									}
                                  
                                }
                            }
						}
                        basketItem.Quantity = basketItem.Orders.Where(x=>x.ProductReferenceId == basketItem.ItemReferenceId).Sum(x => x.WaitingQuantity);
					}
                    BasketItems.Clear();
					SelectedItems.Clear();
                }

                await Shell.Current.GoToAsync("..");

				if (_userDialogs.IsHudShowing)
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

                BasketItems.Clear();

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