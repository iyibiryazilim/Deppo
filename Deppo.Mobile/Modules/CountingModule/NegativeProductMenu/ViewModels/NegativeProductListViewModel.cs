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

namespace Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.ViewModels;

public partial class NegativeProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseCountingService _warehouseCountingService;

	#region Collections
	public ObservableCollection<NegativeProductModel> Items { get; } = new();

	public ObservableCollection<NegativeWarehouseModel> NegativeWarehouses { get; } = new();
	#endregion

	#region Properties
	[ObservableProperty]
	string searchText = string.Empty;

    [ObservableProperty]
    NegativeProductModel selectedItem;
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

    }
    public Page CurrentPage { get; set; } = null!;


    #region Commands
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<SearchBar> PerformSearchCommand { get; }
    public Command RefreshPageCommand { get; }
    public Command ItemTappedCommand { get; }
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

            await LoadNegativeWarehousesAsync();

            CurrentPage.FindByName<BottomSheet>("negativeWarehouseBottomSheet").State = BottomSheetState.HalfExpanded;

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

            // Sadece Raf takipli olma durumu
            if(SelectedItem.LocTracking == 1 && (SelectedItem.TrackingType == 0))
            {

            }
            else if(SelectedItem.LocTracking == 1 && (SelectedItem.TrackingType == 1))
            {
                //todo:Raf ve Seri takipli olma durumu
            }
            // Ne Raf takipli ne de Seri,Lot takipli olma durumu
            else if(SelectedItem.LocTracking == 0 && (SelectedItem.TrackingType == 0))
            {

            }

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

            _userDialogs.ShowLoading("Loading Negative Warehouses...");
            await Task.Delay(1000);

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
}
