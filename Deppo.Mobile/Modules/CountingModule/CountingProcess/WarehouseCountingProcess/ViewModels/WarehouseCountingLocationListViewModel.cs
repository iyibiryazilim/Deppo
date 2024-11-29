using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using DevExpress.Maui.Controls;
using Syncfusion.Maui.DataSource.Extensions;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(nameof(WarehouseCountingWarehouseModel), nameof(WarehouseCountingWarehouseModel))]
public partial class WarehouseCountingLocationListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ILocationService _locationService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private LocationModel selectedLocation = null!;

    [ObservableProperty]
    private ProductVariantType productVariantType;

    [ObservableProperty]
    private WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;

    public ObservableCollection<LocationModel> Items { get; } = new();

    public WarehouseCountingLocationListViewModel(IHttpClientService httpClientService,
    ILocationService locationService,
    IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _locationService = locationService;
        _userDialogs = userDialogs;

        Title = "Raf Seçimi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<LocationModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        SelectProductsCommand = new Command(async () => await SelectProductsAsync());
        SelectVariantsCommand = new Command(async () => await SelectVariantsAsync());
		GoToBackCommand = new Command(async () => await GoToBackAsync());
	}

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command SelectProductsCommand { get; }
    public Command SelectVariantsCommand { get; }
    public Command GoToBackCommand { get; }

	[ObservableProperty]
    public SearchBar searchText;

    public Page CurrentPage { get; set; } = null!;


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
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
						var obj = Mapping.Mapper.Map<LocationModel>(item);
						obj.IsSelected = SelectedLocation != null && obj.ReferenceId == SelectedLocation.ReferenceId ? SelectedLocation.IsSelected : false;
						Items.Add(obj);
					}
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
        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
						var obj = Mapping.Mapper.Map<LocationModel>(item);
						obj.IsSelected = SelectedLocation != null && obj.ReferenceId == SelectedLocation.ReferenceId ? SelectedLocation.IsSelected : false;
						Items.Add(obj);
					}
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

    private void ItemTappedAsync(LocationModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item == SelectedLocation)
            {
                SelectedLocation.IsSelected = false;
                SelectedLocation = null;
            }
            else
            {
                if (SelectedLocation != null)
                {
                    SelectedLocation.IsSelected = false;
                }

                SelectedLocation = item;
                SelectedLocation.IsSelected = true;
                Items.Where(x => x.ReferenceId != SelectedLocation.ReferenceId).ForEach(x => x.IsSelected = false);
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

            CurrentPage.FindByName<BottomSheet>("productOrVariantBottomSheet").State = BottomSheetState.HalfExpanded;
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
            if (SelectedLocation is not null)
            {

                await Shell.Current.GoToAsync($"{nameof(WarehouseCountingBasketView)}", new Dictionary<string, object>
                {
                    [nameof(LocationModel)] = SelectedLocation,
                    [nameof(WarehouseCountingWarehouseModel)] = WarehouseCountingWarehouseModel,
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
            if (SelectedLocation is not null)
            {

                await Shell.Current.GoToAsync($"{nameof(WarehouseCountingBasketView)}", new Dictionary<string, object>
                {
                    [nameof(LocationModel)] = SelectedLocation,
                    [nameof(WarehouseCountingWarehouseModel)] = WarehouseCountingWarehouseModel,
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
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
						var obj = Mapping.Mapper.Map<LocationModel>(item);
                        obj.IsSelected = SelectedLocation != null && obj.ReferenceId == SelectedLocation.ReferenceId ? SelectedLocation.IsSelected : false;
						Items.Add(obj);
					}
                        
                }
            }
            else
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

    private async Task GoToBackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(SelectedLocation is not null)
            {
                SelectedLocation.IsSelected = false;
                SelectedLocation = null;
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
