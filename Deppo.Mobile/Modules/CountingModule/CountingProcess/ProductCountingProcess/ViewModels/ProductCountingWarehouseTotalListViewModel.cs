using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
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

[QueryProperty(nameof(ProductModel), nameof(ProductModel))]
public partial class ProductCountingWarehouseTotalListViewModel : BaseViewModel
{
    private readonly IWarehouseTotalService _warehouseTotalService;
    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;

    public ProductCountingWarehouseTotalListViewModel(IWarehouseTotalService warehouseTotalService, IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _warehouseTotalService = warehouseTotalService;
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Ambar Toplamları";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductCountingWarehouseTotalModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    [ObservableProperty]
    public ProductCountingWarehouseTotalModel selectedWarehouse;

    [ObservableProperty]
    public ProductModel productModel;

    public ObservableCollection<ProductCountingWarehouseTotalModel> Items { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<ProductCountingWarehouseTotalModel> ItemTappedCommand { get; }

    public Command NextViewCommand { get; }

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
            var result = await _warehouseTotalService.GetObjectsByProduct(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber,ProductModel.ReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<ProductCountingWarehouseTotalModel>(item));

                _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                Debug.WriteLine(result.Message);
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

        try
        {
            IsBusy = true;
            _userDialogs.Loading("Refreshing Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductModel.ReferenceId, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<ProductCountingWarehouseTotalModel>(item));

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

    private void ItemTappedAsync(ProductCountingWarehouseTotalModel item)
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

            if (SelectedWarehouse is not null)
            {

                var productCountingBasketModel = new ProductCountingBasketModel{
                    ProductReferenceId = ProductModel.ReferenceId,
                    ProductCode = ProductModel.Code,
                    ProductName = ProductModel.Name,
                    Image = ProductModel.Image,
                    StockQuantity = ProductModel.StockQuantity,
                    OutputQuantity = ProductModel.StockQuantity,
                    SubUnitsetReferenceId = ProductModel.SubUnitsetReferenceId,
                    SubUnitsetName = ProductModel.SubUnitsetName,
                    SubUnitsetCode = ProductModel.SubUnitsetCode,
                    UnitsetReferenceId = ProductModel.UnitsetReferenceId,
                    UnitsetName = ProductModel.UnitsetName,
                    UnitsetCode = ProductModel.UnitsetCode,
                    LocTracking = ProductModel.LocTracking,
                    IsVariant = ProductModel.IsVariant,
                    TrackingType = ProductModel.TrackingType,
                    DifferenceQuantity = 0,
                };


                if(SelectedWarehouse.LocationCount > 0)
                {
                    await Shell.Current.GoToAsync($"{nameof(ProductCountingLocationListView)}", new Dictionary<string, object>
                    {
                        [nameof(ProductCountingWarehouseTotalModel)] = SelectedWarehouse,
                        [nameof(ProductCountingBasketModel)] = productCountingBasketModel
                    });
                }
                else
                {
                    await Shell.Current.GoToAsync($"{nameof(ProductCountingBasketView)}", new Dictionary<string, object>
                    {
                        [nameof(ProductCountingWarehouseTotalModel)] = SelectedWarehouse,
                        [nameof(ProductModel)] = ProductModel

                    });
                }
               
            }
            else
            {
                await _userDialogs.AlertAsync("Lütfen bir ürün seçiniz.");
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
