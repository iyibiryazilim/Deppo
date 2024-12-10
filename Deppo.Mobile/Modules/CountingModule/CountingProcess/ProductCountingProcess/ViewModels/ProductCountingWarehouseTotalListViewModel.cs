using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

[QueryProperty(nameof(ProductCountingBasketModel), nameof(ProductCountingBasketModel))]
public partial class ProductCountingWarehouseTotalListViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;
    private readonly IProductCountingService _productCountingService;

    public ProductCountingWarehouseTotalListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IProductCountingService productCountingService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;
        _productCountingService = productCountingService;

        Title = "Ambar Toplamları";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductCountingWarehouseModel>(async (x) => await ItemTappedAsync(x));
        NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

    [ObservableProperty]
    public ProductCountingWarehouseModel selectedWarehouse;

    [ObservableProperty]
    public ProductCountingBasketModel productCountingBasketModel;


    public ObservableCollection<ProductCountingWarehouseModel> Items { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<ProductCountingWarehouseModel> ItemTappedCommand { get; }

    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();

            if(ProductCountingBasketModel.IsVariant)
                Title = "Varyant Ambar Toplamları";
            else
                Title = "Malzeme Ambar Toplamları";


            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);
            
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            if (ProductCountingBasketModel.IsVariant)
            {
                var result = await _productCountingService.GetWarehousesByVariant(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, variantReferenceId:ProductCountingBasketModel.ItemReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<ProductCountingWarehouseModel>(item));
                    }
                }
            }
            else
            {
                var result = await _productCountingService.GetWarehouses(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: ProductCountingBasketModel.ItemReferenceId , string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<ProductCountingWarehouseModel>(item));
                    }
                }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

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
        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            if (ProductCountingBasketModel.IsVariant)
            {
                var result = await _productCountingService.GetWarehousesByVariant(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, variantReferenceId: ProductCountingBasketModel.ItemReferenceId, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<ProductCountingWarehouseModel>(item));
                    }
				}
            }
            else
            {
                var result = await _productCountingService.GetWarehouses(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: ProductCountingBasketModel.ItemReferenceId, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<ProductCountingWarehouseModel>(item));
                    }	
				}
            }

            if (_userDialogs.IsHudShowing)
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

    private async Task ItemTappedAsync(ProductCountingWarehouseModel item)
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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

            if(SelectedWarehouse is null)
            {
				await _userDialogs.AlertAsync("Lütfen bir ambar seçiniz.");
                return;
			}

            if (SelectedWarehouse.LocationCount > 0)
            {
                await Shell.Current.GoToAsync($"{nameof(ProductCountingLocationListView)}", new Dictionary<string, object>
                {
                    [nameof(ProductCountingWarehouseModel)] = SelectedWarehouse,
                    [nameof(ProductCountingBasketModel)] = ProductCountingBasketModel
                });
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(ProductCountingBasketView)}", new Dictionary<string, object>
                {
                    [nameof(ProductCountingWarehouseModel)] = SelectedWarehouse,
                    [nameof(ProductCountingBasketModel)] = ProductCountingBasketModel

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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(SelectedWarehouse is not null)
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
