using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

public partial class WarehouseCountingWarehouseListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseCountingService _warehouseCountingService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseCountingWarehouseModel selectedWarehouse = null!;

    public ObservableCollection<WarehouseCountingWarehouseModel> Items { get; } = new();

    public WarehouseCountingWarehouseListViewModel(IHttpClientService httpClientService,
    IWarehouseCountingService warehouseCountingService,
    IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseCountingService = warehouseCountingService;
        _userDialogs = userDialogs;

        Title = "Ambar Seçimi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseCountingWarehouseModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
        SelectProductsCommand = new Command(async () => await SelectProductsAsync());
        SelectVariantsCommand = new Command(async () => await SelectVariantsAsync());
		GoToBackCommand = new Command(async () => await GoToBackAsync());
	}

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command GoToBackCommand { get; }

    public Command SelectProductsCommand { get; }
    public Command SelectVariantsCommand { get; }

    public Page CurrentPage { get; set; } = null!;

    [ObservableProperty]
    ProductVariantType productVariantType;

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
            var result = await _warehouseCountingService.GetWarehouses(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<WarehouseCountingWarehouseModel>(item));
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

            _userDialogs.ShowLoading("Loading...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseCountingService.GetWarehouses(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<WarehouseCountingWarehouseModel>(item));
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

    private void ItemTappedAsync(WarehouseCountingWarehouseModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item == SelectedWarehouse)
            {
                SelectedWarehouse.IsSelected = false;
                SelectedWarehouse = null;
            }
            else
            {
                if (SelectedWarehouse != null)
                {
                    SelectedWarehouse.IsSelected = false;
                }

                SelectedWarehouse = item;
                SelectedWarehouse.IsSelected = true;
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SelectedWarehouse.LocationCount == 0)
                CurrentPage.FindByName<BottomSheet>("productOrVariantBottomSheet").State = BottomSheetState.HalfExpanded;
            else
            {
                await Shell.Current.GoToAsync($"{nameof(WarehouseCountingLocationListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseCountingWarehouseModel)] = SelectedWarehouse,
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

    private async Task SelectProductsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            CurrentPage.FindByName<BottomSheet>("productOrVariantBottomSheet").State = BottomSheetState.Hidden;
            ProductVariantType = ProductVariantType.Product;
            if (SelectedWarehouse is not null)
            {
                await Shell.Current.GoToAsync($"{nameof(WarehouseCountingProductListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseCountingWarehouseModel)] = SelectedWarehouse,
                    [nameof(ProductVariantType)] = ProductVariantType
                });

            }
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

    private async Task SelectVariantsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            CurrentPage.FindByName<BottomSheet>("productOrVariantBottomSheet").State = BottomSheetState.Hidden;
            ProductVariantType = ProductVariantType.Variant;
            if (SelectedWarehouse is not null)
            {
                if (SelectedWarehouse is not null)
                {
                    await Shell.Current.GoToAsync($"{nameof(WarehouseCountingProductListView)}", new Dictionary<string, object>
                    {
                        [nameof(WarehouseCountingWarehouseModel)] = SelectedWarehouse,
                        [nameof(ProductVariantType)] = ProductVariantType
                    });

                }

            }
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

	private async Task GoToBackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedWarehouse is not null)
			{
				SelectedWarehouse.IsSelected = false;
				SelectedWarehouse = null;
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
