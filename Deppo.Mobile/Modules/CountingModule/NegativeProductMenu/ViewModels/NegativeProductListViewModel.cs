using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Core.Models.WarehouseModels;
using DevExpress.Maui.Controls;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.Views;

namespace Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.ViewModels;

public partial class NegativeProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseCountingService _warehouseCountingService;

	#region Collections
	public ObservableCollection<NegativeProductModel> Items { get; } = new();

    public ObservableCollection<NegativeProductModel> SelectedNegativeProducts { get; } = new();

    public ObservableCollection<NegativeWarehouseModel> NegativeWarehouses { get; } = new();

    public ObservableCollection<NegativeWarehouseModel> SelectedNegativeWarehouses { get;} = new();

    public ObservableCollection<NegativeProductBasketModel> NegativeBasketModels { get; set; } = new();
    #endregion

    #region Properties
    [ObservableProperty]
	string searchText = string.Empty;

    [ObservableProperty]
    NegativeProductModel selectedItem = new();
	#endregion
	public NegativeProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseCountingService warehouseCountingService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _warehouseCountingService = warehouseCountingService;


        Title = "Negatif Malzemeler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        RefreshPageCommand = new Command(async () => await RefreshPageAsync());
        ItemTappedCommand = new Command<NegativeProductModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());

    }
    public Page CurrentPage { get; set; } = null!;


    #region Commands
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<SearchBar> PerformSearchCommand { get; }
    public Command RefreshPageCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command NextViewCommand { get; }

    #endregion



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

            var result = await _warehouseCountingService.GetNegativeProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty,0,20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    item.LocTrackingIcon = "location-dot";
                    item.VariantIcon = "bookmark";
                    item.TrackingTypeIcon = "box-archive";
					Items.Add(Mapping.Mapper.Map<NegativeProductModel>(item));
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

    public async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;
        if (Items.Count < 18)  // 18 equals to PageSize (20) - RemainingItemsThreshold (2)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.Loading("Load more Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseCountingService.GetNegativeProducts(httpClient,_httpClientService.FirmNumber,_httpClientService.PeriodNumber,string.Empty,Items.Count,20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
					item.LocTrackingIcon = "location-dot";
					item.VariantIcon = "bookmark";
					item.TrackingTypeIcon = "box-archive";
					Items.Add(Mapping.Mapper.Map<NegativeProductModel>(item));
				}
					

                if (_userDialogs.IsHudShowing)
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

    public async Task RefreshPageAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            IsRefreshing = true;
            IsRefreshing = false;

            Items.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseCountingService.GetNegativeProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<NegativeProductModel>(item));
            }
            else
            {
                _userDialogs.Alert(message: result.Message, title: "Hata");
            }

        }
        catch (Exception ex)
        {
            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
    private async void ItemTappedAsync(NegativeProductModel negativeProductModel)
    {
        if (negativeProductModel is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;


            SelectedItem = negativeProductModel;
            SelectedItem.NegativeWarehouses = new();



            if (SelectedItem is not null)
            {
                if (SelectedItem.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ProductReferenceId == negativeProductModel.ProductReferenceId).IsSelected = false;
                    SelectedNegativeProducts.Remove(SelectedNegativeProducts.FirstOrDefault(x => x.ProductReferenceId == SelectedItem.ProductReferenceId));
                    foreach (var warehouse in NegativeWarehouses)
                    {
                        warehouse.IsSelected = false;
                        SelectedNegativeWarehouses.Remove(SelectedNegativeWarehouses.FirstOrDefault(x=>x.WarehouseReferenceId == warehouse.WarehouseReferenceId));
                        SelectedItem.NegativeWarehouses.Remove(warehouse);
                    }
                }
                else
                {
                    Items.FirstOrDefault(x => x.ProductReferenceId == negativeProductModel.ProductReferenceId).IsSelected = true;

                    await LoadNegativeWarehousesAsync();


                    foreach (var warehouse in NegativeWarehouses)
                    {
                        warehouse.IsSelected = true;
                        SelectedNegativeWarehouses.Add(warehouse);
                        SelectedItem.NegativeWarehouses.Add(warehouse);
                    }

                    SelectedNegativeProducts.Add(SelectedItem);
                }
            }

            

        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void ItemSwipedAsync(NegativeProductModel negativeProductModel)
    {
        if (negativeProductModel is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            SelectedItem = negativeProductModel;

            await LoadNegativeWarehousesAsync();

            CurrentPage.FindByName<BottomSheet>("negativeWarehouseBottomSheet").State = BottomSheetState.HalfExpanded;
           

        }
        catch (Exception ex) 
        {
            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadNegativeWarehousesAsync()
    {
        try
        {
            NegativeWarehouses.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseCountingService.GetNegativeWarehousesByProductReferenceId(
                httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				SelectedItem.ProductReferenceId
            );

            if(result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    NegativeWarehouses.Add(Mapping.Mapper.Map<NegativeWarehouseModel>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {

            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            _userDialogs.Alert(ex.Message, "Hata");
        }
    }


    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var warehouseProductDictionary = new Dictionary<int, NegativeProductBasketModel>();

            foreach (var selectedNegativeProduct in SelectedNegativeProducts)
            {
                foreach (var warehouse in selectedNegativeProduct.NegativeWarehouses)
                {
                    if (warehouseProductDictionary.TryGetValue(warehouse.WarehouseReferenceId, out var basket))
                    {
                        basket.NegativeProducts.Add(selectedNegativeProduct);
                    }
                    else
                    {
                        var newBasket = new NegativeProductBasketModel
                        {
                            NegativeWarehouseModel = warehouse,
                            NegativeProducts = new ObservableCollection<NegativeProductModel>{ selectedNegativeProduct }
                        };

                        warehouseProductDictionary[warehouse.WarehouseReferenceId] = newBasket;
                    }
                }
            }

            NegativeBasketModels = new ObservableCollection<NegativeProductBasketModel>(warehouseProductDictionary.Values.ToList());




            if (SelectedNegativeProducts.Count == 0)
            {
                await _userDialogs.AlertAsync("Lütfen en az bir ürün seçiniz.", "Hata", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(NegativeProductFormView)}", new Dictionary<string, object>
            {
                [nameof(NegativeProductBasketModel)] = NegativeBasketModels,
            });
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
}
