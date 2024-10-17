using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseCountingWarehouseModel), queryId: nameof(WarehouseCountingWarehouseModel))]
[QueryProperty(name: nameof(ProductVariantType), queryId: nameof(ProductVariantType))]
public partial class WarehouseCountingProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseCountingService _warehouseCountingService;

    [ObservableProperty]
    WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;

    [ObservableProperty]
    ProductVariantType productVariantType;

    public ObservableCollection<WarehouseCountingBasketModel> Items { get; } = new();
	public ObservableCollection<WarehouseCountingBasketModel> SelectedItems { get; } = new();
	public WarehouseCountingProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseCountingService warehouseCountingService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _warehouseCountingService = warehouseCountingService;

        Title = "Ürün Sepeti";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        IncreaseCommand = new Command<WarehouseCountingBasketModel>(async (item) => await IncreaseAsync(item));
        DecreaseCommand = new Command<WarehouseCountingBasketModel>(async (item) => await DecreaseAsync(item));
        SwipeItemCommand = new Command<WarehouseCountingBasketModel>(async (item) => await SwipeItemAsync(item));
        BackCommand = new Command(async () => await BackAsync());
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command BackCommand { get; }
    public Command NextViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command SwipeItemCommand { get; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            await Task.Delay(1000);
            Items.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            if (ProductVariantType == ProductVariantType.Variant)
            {
                Title = "Varyant Sepeti";
                var result = await _warehouseCountingService.GetVariantsByWarehouse(
                     httpClient: httpClient,
                     firmNumber: _httpClientService.FirmNumber,
                     periodNumber: _httpClientService.PeriodNumber,
                     warehouseNumber: WarehouseCountingWarehouseModel.Number, // WarehouseCountingWarehouseModel.Number
                     search: string.Empty,
                     skip: 0,
                     take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                        obj.OutputQuantity = obj.StockQuantity;

                        Items.Add(obj);
                    }
                }
            }
            else
            {
                var result = await _warehouseCountingService.GetProductsByWarehouse(
                     httpClient: httpClient,
                     firmNumber: _httpClientService.FirmNumber,
                     periodNumber: _httpClientService.PeriodNumber,
                     warehouseNumber: WarehouseCountingWarehouseModel.Number, // WarehouseCountingWarehouseModel.Number
                     search: string.Empty,
                     skip: 0,
                     take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                        obj.OutputQuantity = obj.StockQuantity;

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

            _userDialogs.Loading("Loading More Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            if (ProductVariantType == ProductVariantType.Variant)
            {
                var result = await _warehouseCountingService.GetVariantsByWarehouse(
                     httpClient: httpClient,
                     firmNumber: _httpClientService.FirmNumber,
                     periodNumber: _httpClientService.PeriodNumber,
                     warehouseNumber: WarehouseCountingWarehouseModel.Number, // WarehouseCountingWarehouseModel.Number
                     search: string.Empty,
                     skip: Items.Count,
                     take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                        obj.OutputQuantity = obj.StockQuantity;

                        Items.Add(obj);
                    }
                }
            }
            else
            {
                var result = await _warehouseCountingService.GetProductsByWarehouse(
                     httpClient: httpClient,
                     firmNumber: _httpClientService.FirmNumber,
                     periodNumber: _httpClientService.PeriodNumber,
                     warehouseNumber: WarehouseCountingWarehouseModel.Number, // WarehouseCountingWarehouseModel.Number
                     search: string.Empty,
                     skip: Items.Count,
                     take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                        obj.OutputQuantity = obj.StockQuantity;

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

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task IncreaseAsync(WarehouseCountingBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            item.OutputQuantity++;
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

    private async Task DecreaseAsync(WarehouseCountingBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item.OutputQuantity > 0)
                item.OutputQuantity--;
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

    private async Task SwipeItemAsync(WarehouseCountingBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item.IsCompleted)
            {
                item.IsCompleted = false;
            }
            else
            {
                item.IsCompleted = true;
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

			SelectedItems.Clear();
			foreach (var item in Items.Where(x => (x.OutputQuantity - x.StockQuantity) != 0))
			{
				SelectedItems.Add(item);
			}

			if (SelectedItems.Count == 0)
			{
				await _userDialogs.AlertAsync("Sayım yapılacak ürün bulunamadı", "Uyarı", "Tamam");
				return;
			}

			var confirm = await _userDialogs.ConfirmAsync("Miktarı sayılan ürünlerle sayım işlemine devam etmek istiyor musunuz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			await Shell.Current.GoToAsync($"{nameof(WarehouseCountingFormView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseCountingWarehouseModel)] = WarehouseCountingWarehouseModel,
                [nameof(WarehouseCountingBasketModel)] = SelectedItems
			});
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            SelectedItems.Clear();
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
