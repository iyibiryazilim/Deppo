﻿using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ReturnSalesProductListViewModel : BaseViewModel
{

    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseTotalService _warehouseTotalService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;
   private readonly IProductService _productService;
    [ObservableProperty]
    private WarehouseModel warehouseModel= null!;

    [ObservableProperty]
    private ProductModel? selectedProduct;
    [ObservableProperty]
    private ObservableCollection<ReturnSalesBasketModel> selectedProducts = new();


    public ObservableCollection<ProductModel> Items { get; } = new();
    public ObservableCollection<ProductModel> SelectedItems { get; } = new();

    public ReturnSalesProductListViewModel(
        IHttpClientService httpClientService,
        IProductService productService,
        IWarehouseTotalService warehouseTotalService,
    IUserDialogs userDialogs,
        IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _productService = productService;
        _warehouseTotalService = warehouseTotalService;
        Title = "Ürün Listesi";


        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductModel>(async (item) => await ItemTappedAsync(item));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());


    }
    private Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;
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

            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if(result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var product = Mapping.Mapper.Map<ProductModel>(item);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == product.ReferenceId);
                        if (matchingItem != null)
                            product.IsSelected = matchingItem.IsSelected;
                        else
                            product.IsSelected = false;
                        Items.Add(product);
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

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var product = Mapping.Mapper.Map<ProductModel>(item);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == product.ReferenceId);
                        if (matchingItem != null)
                            product.IsSelected = matchingItem.IsSelected;
                        else
                            product.IsSelected = false;
                        Items.Add(product);
                    }
                }
            }

        }

        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
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

            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
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

            if (!result.IsSuccess)
            {
                _userDialogs.Alert(result.Message, "Hata");
                return;
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
    private async Task ItemTappedAsync(ProductModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
                if (item.IsVariant)
                {
                    CurrentPage.FindByName<BottomSheet>("").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    if (!item.IsSelected)
                    {
                        Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = true;

                        SelectedProduct = item;
                        SelectedItems.Add(item);

                        var basketItem = new ReturnSalesBasketModel
                        {
                            ItemReferenceId = item.ReferenceId,
                            ItemCode = item.Code,
                            ItemName = item.Name,
                            UnitsetReferenceId = item.UnitsetReferenceId,
                            UnitsetCode = item.UnitsetCode,
                            UnitsetName = item.UnitsetName,
                            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                            SubUnitsetCode = item.SubUnitsetCode,
                            SubUnitsetName = item.SubUnitsetName,
                            IsSelected = false,
                            MainItemCode = string.Empty,
                            MainItemName = string.Empty,
                            MainItemReferenceId = default,
                            StockQuantity = item.StockQuantity,
                            Quantity = item.LocTracking == 0 ? 1 : 0,
                            InputQuantity = item.LocTracking == 0 ? 1 : 0,
                            LocTracking = item.LocTracking,
                            TrackingType = item.TrackingType,
                            IsVariant = item.IsVariant
                        };

                        SelectedProducts.Add(basketItem);
                    }
                    else
                    {
                        SelectedProduct = null;
                        var selectedItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ReferenceId);
                        if (selectedItem != null)
                        {
                            SelectedProducts.Remove(selectedItem);
                            Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = false;
                            SelectedItems.Remove(item);
                        }
                    }
                }
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
    private async Task ConfirmAsync()
    {
        
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var previouseViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();
            if (previouseViewModel is not null)
            {
                foreach (var item in SelectedProducts)
                    if (!previouseViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
                        previouseViewModel.Items.Add(item);

                await Shell.Current.GoToAsync($"..");
                SelectedItems.Clear();
            }
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            SelectedProducts.Clear();
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

            await Shell.Current.GoToAsync($"..");
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
    public async Task LoadPageAsync()
    {
   
        try
        {


            if (Items?.Count > 0)
                Items.Clear();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }

    }

}