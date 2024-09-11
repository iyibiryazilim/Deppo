using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class InputProductPurchaseOrderProcessBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ILocationService _locationService;
    private readonly ISeriLotService _seriLotService;

    [ObservableProperty]
    private WarehouseModel warehouseModel;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier;

    [ObservableProperty]
    private ObservableCollection<InputPurchaseBasketModel> items;

    [ObservableProperty]
    private InputPurchaseBasketModel? selectedItem;

    public InputProductPurchaseOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService, ISeriLotService seriLotService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _locationService = locationService;
        _seriLotService = seriLotService;

        Title = "Satınalma Sepeti";

        IncreaseCommand = new Command<InputPurchaseBasketModel>(async (x) => await IncreaseAsync(x));

        LoadMoreWarehouseLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
        LocationIncreaseCommand = new Command<LocationModel>(LocationIncrease);
        LocationDecreaseCommand = new Command<LocationModel>(LocationDecrease);
        LocationConfirmCommand = new Command(LocationConfirm);
        LocationCloseCommand = new Command(async () => await LocationCloseAsync());

        LoadMoreSeriLotsCommand = new Command(async () => await LoadMoreSeriLotAsync());
        SeriLotIncreaseCommand = new Command<SeriLotModel>(SeriLotIncrease);
        SeriLotDecreaseCommand = new Command<SeriLotModel>(SeriLotDecrease);
        SeriLotConfirmCommand = new Command(SeriLotConfirm);
        SeriLotCloseCommand = new Command(async () => await SeriLotCloseAsync());

        BackCommand = new Command(async () => await BackAsync());
    }

    public Page CurrentPage { get; set; }

    public ObservableCollection<LocationModel> Locations { get; } = new();
    public ObservableCollection<SeriLotModel> SeriLots { get; } = new();

    #region Commands

    public Command<InputPurchaseBasketModel> DeleteItemCommand { get; }
    public Command<InputPurchaseBasketModel> IncreaseCommand { get; }
    public Command<InputPurchaseBasketModel> DecreaseCommand { get; }

    public Command LoadMoreWarehouseLocationsCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }

    public Command LoadMoreSeriLotsCommand { get; }
    public Command<SeriLotModel> SeriLotIncreaseCommand { get; }
    public Command<SeriLotModel> SeriLotDecreaseCommand { get; }
    public Command SeriLotConfirmCommand { get; }
    public Command SeriLotCloseCommand { get; }

    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    #endregion Commands

    private async Task IncreaseAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            SelectedItem = inputPurchaseBasketModel;

            if (inputPurchaseBasketModel.LocTracking == 1)
            {
                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketLocationListView)}", new Dictionary<string, object>
                {
                    {nameof(WarehouseModel), WarehouseModel},
                    {nameof(InputProductBasketModel), inputPurchaseBasketModel}
                });
            }
            else if (inputPurchaseBasketModel.TrackingType == 1)
            {
                await LoadSeriLotAsync(inputPurchaseBasketModel);
                CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else
            {
                inputPurchaseBasketModel.InputQuantity += 1;
            }
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadWarehouseLocationsAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Locations.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, inputPurchaseBasketModel.ItemReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
    }

    private async Task LoadMoreWarehouseLocationsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SelectedItem.ItemReferenceId, string.Empty, Locations.Count, 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
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

    private void LocationIncrease(LocationModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            item.InputQuantity += 1;
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

    private void LocationDecrease(LocationModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item.InputQuantity > 0)
            {
                item.InputQuantity -= 1;
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

    private void LocationConfirm()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (Locations.Count > 0)
            {
                double totalInputQuantity = 0;
                foreach (var location in Locations)
                {
                    if (location.InputQuantity > 0)
                        totalInputQuantity += location.InputQuantity;
                }

                SelectedItem.InputQuantity = totalInputQuantity;

                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
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

    private async Task LocationCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task LoadSeriLotAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            SeriLots.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, search: string.Empty, skip: 0, take: 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        SeriLots.Add(Mapping.Mapper.Map<SeriLotModel>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
    }

    private async Task LoadMoreSeriLotAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, search: string.Empty, skip: SeriLots.Count, take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    SeriLots.Add(Mapping.Mapper.Map<SeriLotModel>(item));
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

    private async Task SeriLotCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private void SeriLotIncrease(SeriLotModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            item.InputQuantity += 1;
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

    private void SeriLotDecrease(SeriLotModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item.InputQuantity > 0)
            {
                item.InputQuantity -= 1;
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

    private void SeriLotConfirm()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SeriLots.Count > 0)
            {
                double totalInputQuantity = 0;
                foreach (var seriLot in SeriLots)
                {
                    if (seriLot.InputQuantity > 0)
                        totalInputQuantity += seriLot.InputQuantity;
                }

                SelectedItem.InputQuantity = totalInputQuantity;

                CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
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

    private async Task BackAsync()
    {
        try
        {
            IsBusy = true;

            if (Items.Count > 0)
            {
                var result = await _userDialogs.ConfirmAsync(message: "Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", title: "Uyarı", okText: "Evet", cancelText: "Hayır");
                if (result)
                {
                    SelectedItem = null;
                    Items.Clear();
                    await Shell.Current.GoToAsync("..");
                }
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

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
}