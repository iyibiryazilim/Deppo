using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using DevExpress.Maui.Controls;
using DevExpress.Utils.Filtering.Internal;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

[QueryProperty(name: nameof(VirmanBasketModel), queryId: nameof(VirmanBasketModel))]
public partial class VirmanProductBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocationService _locationService;
    private readonly ILocationTransactionService _locationTransactionService;

    [ObservableProperty]
    private VirmanBasketModel virmanBasketModel = null!;

    public ObservableCollection<LocationTransactionModel> SelectedLocationTransactions { get; } = new();

    [ObservableProperty]
    private LocationTransactionModel? selectedLocationTransaction;

    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    public ObservableCollection<LocationModel> SelectedLocations { get; } = new();
    public ObservableCollection<LocationModel> Locations { get; } = new();

    public VirmanProductBasketListViewModel(IHttpClientService httpClientService
        , ISeriLotTransactionService serilotTransactionService
        , IUserDialogs userDialogs
        , IProductService productService
        , IServiceProvider serviceProvider,
ILocationService locationService, ILocationTransactionService locationTransactionService)
    {
        _httpClientService = httpClientService;
        _locationTransactionService = locationTransactionService;
        _userDialogs = userDialogs;
        _productService = productService;
        Title = "Virman Sepet Listesi";
        _serviceProvider = serviceProvider;
        _locationService = locationService;
        Title = "Sepet Listesi";

        IncreaseCommand = new Command(async () => await IncreaseAsync());
        DecreaseCommand = new Command(async () => await DecreaseAsync());
        // DeleteItemCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await DeleteItemAsync(item));
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());
        IncreaseInCommand = new Command(async () => await IncreaseInAsync());
        DecreaseInCommand = new Command(async () => await DecreaseInAsync());

        LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
        LocationTransactionIncreaseCommand = new Command<LocationTransactionModel>(item => LocationTransactionIncreaseAsync(item));
        LocationTransactionDecreaseCommand = new Command<LocationTransactionModel>(item => LocationTransactionDecreaseAsync(item));
        ConfirmLocationTransactionCommand = new Command(async () => await ConfirmLocationTransactionAsync());
        LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

        LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
        LocationCloseCommand = new Command(async () => await LocationCloseAsync());
        LocationConfirmCommand = new Command(async () => await LocationConfirmAsync());
        LocationTappedCommand = new Command<LocationModel>(async (item) => await LocationTappedAsync(item));
        LocationIncreaseCommand = new Command<LocationModel>(async (item) => await LocationIncreaseAsync(item));
        LocationDecreaseCommand = new Command<LocationModel>(async (item) => await LocationDecraseAsync(item));
    }

    public Page CurrentPage { get; set; } = null!;

    public Command ShowProductViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command DeleteItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }
    public Command IncreaseInCommand { get; }
    public Command DecreaseInCommand { get; }

    public Command LoadMoreLocationTransactionsCommand { get; }
    public Command LocationTransactionIncreaseCommand { get; }
    public Command LocationTransactionDecreaseCommand { get; }
    public Command ConfirmLocationTransactionCommand { get; }
    public Command LocationTransactionCloseCommand { get; }

    public Command LoadMoreLocationsCommand { get; }
    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }
    public Command<LocationModel> LocationTappedCommand { get; }

    private async Task DecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (VirmanBasketModel.OutVirmanQuantity > 0)
            {
                if (VirmanBasketModel.OutVirmanProduct.LocTracking == 1)
                {
                    await LoadLocationTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    VirmanBasketModel.OutVirmanQuantity = VirmanBasketModel.OutVirmanQuantity - 1;
                    VirmanBasketModel.InVirmanQuantity = VirmanBasketModel.OutVirmanQuantity;
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

    private async Task IncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (VirmanBasketModel.OutVirmanProduct.LocTracking == 1)
            {
                await LoadLocationTransactionsAsync();
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else
            {
                VirmanBasketModel.OutVirmanQuantity = VirmanBasketModel.OutVirmanQuantity + 1;
                VirmanBasketModel.InVirmanQuantity = VirmanBasketModel.OutVirmanQuantity;
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

    private async Task LoadLocationTransactionsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Load Location Items...");
            await Task.Delay(1000);
            LocationTransactions.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: VirmanBasketModel.OutVirmanProduct.ReferenceId, warehouseNumber: VirmanBasketModel.OutVirmanWarehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
                }
            }

            _userDialogs.Loading().Hide();
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

    private async Task LoadMoreLocationTransactionsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Load Location Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: VirmanBasketModel.OutVirmanProduct.ReferenceId, warehouseNumber: VirmanBasketModel.OutVirmanWarehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
                }
            }

            _userDialogs.Loading().Hide();
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

    private async Task LocationTransactionIncreaseAsync(LocationTransactionModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item is not null)
            {
                selectedLocationTransaction = item;
                if (item.OutputQuantity < item.Quantity)
                {
                    item.OutputQuantity++;

                    if (item.OutputQuantity > 0 && !item.IsSelected)
                        item.IsSelected = true;
                }
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationTransactionDecreaseAsync(LocationTransactionModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item is not null)
            {
                SelectedLocationTransaction = item;
                if (item.OutputQuantity == 0)
                    item.IsSelected = false;
                if (item.OutputQuantity > 0)
                {
                    item.OutputQuantity--;
                }
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ConfirmLocationTransactionAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (LocationTransactions.Count > 0)
            {
                SelectedLocationTransactions.Clear();

                foreach (var x in LocationTransactions.Where(x => x.OutputQuantity > 0))
                {
                    SelectedLocationTransactions.Add(x);
                }

                foreach (var item in SelectedLocationTransactions)
                {
                    var selectedLocationTransactionItem = VirmanBasketModel.OutVirmanProduct.LocationTransactionModels.FirstOrDefault(x => x.TransactionReferenceId == item.TransactionReferenceId);
                    if (selectedLocationTransactionItem is not null)
                    {
                        selectedLocationTransactionItem.Quantity = VirmanBasketModel.OutVirmanQuantity;
                    }

                    VirmanBasketModel.OutVirmanProduct.LocationTransactionModels.Add(new LocationTransactionModel
                    {
                        ReferenceId = item.ReferenceId,
                        LocationReferenceId = item.LocationReferenceId,
                        LocationCode = item.LocationCode,
                        LocationName = item.LocationName,
                        TransactionReferenceId = item.TransactionReferenceId,
                        InSerilotTransactionReferenceId = item.InSerilotTransactionReferenceId,
                        TransactionFicheReferenceId = item.TransactionFicheReferenceId,
                        InTransactionReferenceId = item.InTransactionReferenceId,
                        Quantity = item.OutputQuantity,
                        RemainingQuantity = item.OutputQuantity,
                    });
                }

                var totalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
                VirmanBasketModel.OutVirmanQuantity = totalOutputQuantity;
                VirmanBasketModel.InVirmanQuantity = totalOutputQuantity;
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationTransactionCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (VirmanBasketModel.OutVirmanQuantity == 0 || VirmanBasketModel.OutVirmanQuantity == 0)
            {
                await _userDialogs.AlertAsync("0 Miktarda Bir Ürüne Virman İşlemi Uygulanamaz.", "Uyarı", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(VirmanProductFormListView)}", new Dictionary<string, object>
            {
                [nameof(VirmanBasketModel)] = VirmanBasketModel
            });
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
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

            var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürün silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!result)
                return;

            VirmanBasketModel.OutVirmanQuantity = 0;
            VirmanBasketModel.InVirmanQuantity = 0;
            VirmanBasketModel.OutVirmanProduct = null!;
            VirmanBasketModel.InVirmanProduct = null!;
            VirmanBasketModel.OutVirmanWarehouse = null!;
            VirmanBasketModel.InVirmanWarehouse = null!;
            SelectedLocationTransactions.Clear();
            SelectedLocations.Clear();
            VirmanBasketModel = null;
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DecreaseInAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (VirmanBasketModel.InVirmanQuantity > 0)
            {
                if (VirmanBasketModel.InVirmanProduct.LocTracking == 1)
                {
                    await LoadLocationTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    VirmanBasketModel.OutVirmanQuantity = VirmanBasketModel.OutVirmanQuantity - 1;
                    VirmanBasketModel.InVirmanQuantity = VirmanBasketModel.OutVirmanQuantity;
                }
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task IncreaseInAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (VirmanBasketModel.InVirmanProduct.LocTracking == 1)
            {
                await LoadWarehouseLocationsAsync(VirmanBasketModel);
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else
            {
                VirmanBasketModel.OutVirmanQuantity = VirmanBasketModel.OutVirmanQuantity + 1;
                VirmanBasketModel.InVirmanQuantity = VirmanBasketModel.OutVirmanQuantity;
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadWarehouseLocationsAsync(VirmanBasketModel virmanBasketModel)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Locations.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, VirmanBasketModel.InVirmanWarehouse.Number, VirmanBasketModel.InVirmanProduct.ReferenceId, string.Empty, 0, 20);
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
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, VirmanBasketModel.InVirmanWarehouse.Number, VirmanBasketModel.InVirmanProduct.ReferenceId, search: string.Empty, skip: Locations.Count, take: 20);

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

    private async Task LocationCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task LocationTappedAsync(LocationModel locationModel)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (locationModel.IsSelected != true)
            {
                var tappedItem = Locations.ToList().FirstOrDefault(x => x.Code == locationModel.Code);
                if (tappedItem != null)
                    tappedItem.IsSelected = true;
                SelectedLocations.Add(locationModel);
            }
            else
            {
                var tappedItem = Locations.ToList().FirstOrDefault(x => x.Code == locationModel.Code);
                if (tappedItem != null)
                    tappedItem.IsSelected = false;
                SelectedLocations.Remove(locationModel);
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationConfirmAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (Locations.Count > 0 && SelectedLocations.Count > 0)
            {
                if (VirmanBasketModel.OutVirmanQuantity != SelectedLocations.Sum(x => x.InputQuantity))
                {
                    _userDialogs.Alert("Miktarlar eşit olmalıdır.", "Hata", "Tamam");
                }
                else
                {
                    foreach (var i in SelectedLocations)
                    {
                        VirmanBasketModel.InVirmanProduct.Locations.Add(i);
                    }
                    CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
                }
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationIncreaseAsync(LocationModel locationModel)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            double count = 0;
            if (VirmanBasketModel.InVirmanQuantity <= VirmanBasketModel.OutVirmanQuantity)
                locationModel.InputQuantity++;
            //var count = SelectedLocations.Sum(x => x.InputQuantity);
            foreach (var i in SelectedLocations)
            {
                if (count == VirmanBasketModel.OutVirmanQuantity)
                {
                    if (_userDialogs.IsHudShowing)
                        _userDialogs.HideHud();
                    await _userDialogs.AlertAsync("Çıkış Ürün Miktarından Fazla Giriş Ürünü Olamaz", "Hata", "Tamam");
                }
                else
                    count += i.InputQuantity;
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationDecraseAsync(LocationModel locationModel)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (locationModel.InputQuantity > 0)
            {
                locationModel.InputQuantity--;
            }
        }
        catch (Exception e)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(e.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
}