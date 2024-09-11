using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Supplier), queryId: nameof(Supplier))]
[QueryProperty(name: nameof(InputProductProcessType), queryId: nameof(InputProductProcessType))]
public partial class InputProductPurchaseProcessBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ILocationService _locationService;
    private readonly ISeriLotService _seriLotService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private Supplier supplier = null!;

    [ObservableProperty]
    private InputProductBasketModel? selectedInputProductBasketModel;

    [ObservableProperty]
    private InputProductProcessType inputProductProcessType;

    public InputProductPurchaseProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IHttpClientService httpClientService2, ILocationService locationService, ISeriLotService seriLotService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _locationService = locationService;
        _seriLotService = seriLotService;
        Title = "Sepet Listesi";

        ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());

        DeleteItemCommand = new Command<InputProductBasketModel>(async (item) => await DeleteItemAsync(item));
        IncreaseCommand = new Command<InputProductBasketModel>(async (item) => await IncreaseAsync(item));
        DecreaseCommand = new Command<InputProductBasketModel>(async (item) => await DecreaseAsync(item));

        LocationCloseCommand = new Command(async () => await LocationCloseAsync());
        LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
        LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
        LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));

        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());

        Items.Clear();
    }

    public Page CurrentPage { get; set; } = null!;

    public Command ShowProductViewCommand { get; }
    public Command<InputProductBasketModel> DeleteItemCommand { get; }
    public Command<InputProductBasketModel> IncreaseCommand { get; }
    public Command<InputProductBasketModel> DecreaseCommand { get; }

    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command<LocationModel> LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }

    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public ObservableCollection<InputProductBasketModel> Items { get; } = new();
    public ObservableCollection<LocationModel> Locations { get; }

    private async Task ShowProductViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessProductListView)}", new Dictionary<string, object>
            {
                {nameof(WarehouseModel), WarehouseModel}
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

    private async Task DeleteItemAsync(InputProductBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Items.Remove(item);
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

    private async Task IncreaseAsync(InputProductBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item.LocTracking == 1)
            {
                await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketLocationListView)}", new Dictionary<string, object>
                {
                    {nameof(WarehouseModel), WarehouseModel},
                    {nameof(InputProductBasketModel), item}
                });
            }
            else
                item.Quantity++;
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

    private async Task DecreaseAsync(InputProductBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item.Quantity > 1)
                item.Quantity--;
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

    private async Task LoadLocationItemsAsync(InputProductBasketModel item)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, item.ItemReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var location in result.Data)
                {
                    Locations.Add(Mapping.Mapper.Map<LocationModel>(location));
                }
            }
            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task LocationCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task LocationIncreaseAsync(LocationModel locationModel)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            locationModel.InputQuantity++;
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync($"{ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationConfirmAsync(LocationModel locationModel)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (Locations.Count > 0)
            {
                double totalInputQuantity = 0;
                foreach (var location in Locations)
                {
                    if (location.InputQuantity > 0)
                    {
                        totalInputQuantity += location.InputQuantity;
                    }
                }
                SelectedInputProductBasketModel.Quantity = totalInputQuantity;

                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationDecreaseAsync(LocationModel locationModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (locationModel.InputQuantity > 0)
            {
                locationModel.InputQuantity -= 1;
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (Items.Count == 0)
            {
                await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
                return;
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (Items.Count > 0)
            {
                var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                    return;

                Items.Clear();
                await Shell.Current.GoToAsync("..");
            }
            else
                await Shell.Current.GoToAsync("..");
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