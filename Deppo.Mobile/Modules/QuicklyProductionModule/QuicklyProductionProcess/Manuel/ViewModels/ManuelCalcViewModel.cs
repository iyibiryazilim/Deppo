using Android.App;
using AndroidX.AppCompat.View.Menu;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using static Java.Text.Normalizer;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

[QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
public partial class ManuelCalcViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseService _warehouseService;
    private readonly ILocationService _locationService;

    [ObservableProperty]
    private QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;

    [ObservableProperty]
    public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();

    public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    public QuicklyBomSubProductModel selectedItem = new();

    public ObservableCollection<LocationModel> SelectedLocations { get; } = new();
    public ObservableCollection<LocationModel> Locations { get; } = new();

    public ManuelCalcViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IWarehouseService warehouseService, ILocationService locationService)
    {
        _httpClientService = httpClientService;
        _locationTransactionService = locationTransactionService;
        _userDialogs = userDialogs;
        _warehouseService = warehouseService;
        Title = "Ürün Detayı";

        IncreaseCommand = new Command(async () => await IncreaseAsync());

        DecreaseCommand = new Command(async () => await DecreaseAsync());

        NextViewCommand = new Command(async () => await NextViewAsync());
        _locationService = locationService;

        BackCommand = new Command(async () => await BackAsync());

        AddConsumableItemCommand = new Command(async () => await AddConsumableItemAsync());

        LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
        LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
        LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
        LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
        LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
        DeleteItemCommand = new Command<QuicklyBomSubProductModel>(async (item) => await DeleteItemAsync(item));
        SubIncreaseCommand = new Command<QuicklyBomSubProductModel>(async (item) => await SubIncreaseAsync(item));
        SubDecreaseCommand = new Command<QuicklyBomSubProductModel>(async (item) => await SubDecreaseAsync(item));

        //Locations
        LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
        LocationCloseCommand = new Command(async () => await LocationCloseAsync());
        LocationConfirmCommand = new Command(async () => await LocationConfirmAsync());
        LocationTappedCommand = new Command<LocationModel>(async (item) => await LocationTappedAsync(item));
        LocationIncreaseCommand = new Command<LocationModel>(async (item) => await LocationIncreaseAsync(item));
        LocationDecreaseCommand = new Command<LocationModel>(async (item) => await LocationDecraseAsync(item));
    }

    public ContentPage CurrentPage { get; set; } = null!;

    #region Commands

    public Command ShowProductViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command DeleteItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }
    public Command AddConsumableItemCommand { get; }

    //LocationTransaction
    public Command LoadMoreLocationTransactionsCommand { get; }

    public Command<GroupLocationTransactionModel> LocationTransactionIncreaseCommand { get; }
    public Command<GroupLocationTransactionModel> LocationTransactionDecreaseCommand { get; }
    public Command LocationTransactionConfirmCommand { get; }
    public Command LocationTransactionCloseCommand { get; }

    public Command ConfirmLocationTransactionCommand { get; }

    //SubProduct Increase Decrease
    public Command<QuicklyBomSubProductModel> SubIncreaseCommand { get; }

    public Command<QuicklyBomSubProductModel> SubDecreaseCommand { get; }

    //Locations
    public Command LoadMoreLocationsCommand { get; }

    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }
    public Command<LocationModel> LocationTappedCommand { get; }

    #endregion Commands

    private async Task IncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel is not null && QuicklyBomProductBasketModel.QuicklyBomProduct.LocTracking == 1)
            {
                await LoadWarehouseLocationsAsync();
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else if (QuicklyBomProductBasketModel is not null)
            {
                QuicklyBomProductBasketModel.BOMQuantity += 1;
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

    private async Task DecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel is not null && QuicklyBomProductBasketModel.QuicklyBomProduct.LocTracking == 1)
            {
                await LoadWarehouseLocationsAsync();
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else if (QuicklyBomProductBasketModel is not null && QuicklyBomProductBasketModel.BOMQuantity > 0)
            {
                QuicklyBomProductBasketModel.BOMQuantity -= 1;
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
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel != null)
            {
                var result = await _userDialogs.ConfirmAsync("Çıkıyorsunuz Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                    return;
                QuicklyBomProductBasketModel.MainLocations.Clear();

                LocationTransactions.Clear();
                SelectedLocationTransactions.Clear();

                await Shell.Current.GoToAsync("..");
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            foreach (var subProducts in QuicklyBomProductBasketModel.SubProducts)
            {
                if (subProducts.SubBOMQuantity == 0)
                {
                    _userDialogs.Alert("Alt Ürün miktarlarında 0 olamaz.", "Hata", "Tamam");
                    return;
                }
            }
            if (QuicklyBomProductBasketModel.BOMQuantity == 0)
            {
                _userDialogs.Alert("Ana Ürün miktarı 0 olamaz.", "Hata", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(ManuelFormListView)}", new Dictionary<string, object>
            {
                [nameof(QuicklyBomProductBasketModel)] = QuicklyBomProductBasketModel
            });
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

    private async Task DeleteItemAsync(QuicklyBomSubProductModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var result = await _userDialogs.ConfirmAsync($"{item.ProductModel.Code}\n{item.ProductModel.Name}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

            if (!result)
                return;

            if (SelectedItem == item)
            {
                SelectedItem.ProductModel.IsSelected = false;
                SelectedItem = null;
            }

            QuicklyBomProductBasketModel.SubProducts.Remove(item);
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

    // + işareti tıklanınca çalışacak olan fonksiyon
    private async Task AddConsumableItemAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (QuicklyBomProductBasketModel.QuicklyBomProduct != null)
                await Shell.Current.GoToAsync($"{nameof(ManuelCalcOutWarehouseListView)}");
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

    public async Task SubIncreaseAsync(QuicklyBomSubProductModel item)
    {
        if (IsBusy)
            return;
        try
        {
            if (item is not null)
            {
                SelectedItem = item;
                if (item.ProductModel.LocTracking == 1)
                {
                    await LoadLocationTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    if (item.SubBOMQuantity < item.ProductModel.StockQuantity)
                        item.SubBOMQuantity = item.SubBOMQuantity + 1;
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

    public async Task SubDecreaseAsync(QuicklyBomSubProductModel item)
    {
        if (IsBusy)
            return;
        try
        {
            if (item is not null)
            {
                SelectedItem = item;
                if (item.ProductModel.LocTracking == 1)
                {
                    await LoadLocationTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    if (item.SubBOMQuantity < item.ProductModel.StockQuantity)
                        item.SubBOMQuantity = item.SubBOMQuantity - 1;
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

    public async Task LoadLocationTransactionsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Load Location Items...");
            await Task.Delay(1000);
            LocationTransactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ProductModel.IsVariant ? SelectedItem.ProductModel.MainProductReferenceId : SelectedItem.ProductModel.ReferenceId,
                variantReferenceId: SelectedItem.ProductModel.IsVariant ? SelectedItem.ProductModel.ReferenceId : 0,
                warehouseNumber: SelectedItem.WarehouseModel.Number,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    GroupLocationTransactionModel model = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                    if (model != null)
                    {
                        if (selectedItem.LocationTransactions.Any(x => x.LocationReferenceId == model.LocationReferenceId))
                        {
                            model.OutputQuantity = SelectedItem.LocationTransactions.FirstOrDefault(x => x.LocationReferenceId == model.LocationReferenceId).OutputQuantity;
                        }
                    }
                    LocationTransactions.Add(model);
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    public async Task LoadMoreLocationTransactionsAsync()
    {
        if (LocationTransactions.Count < 18)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Load More Location Items");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
			    productReferenceId: SelectedItem.ProductModel.IsVariant ? SelectedItem.ProductModel.MainProductReferenceId : SelectedItem.ProductModel.ReferenceId,
				variantReferenceId: SelectedItem.ProductModel.IsVariant ? SelectedItem.ProductModel.ReferenceId : 0,
				warehouseNumber: SelectedItem.WarehouseModel.Number,
                skip: LocationTransactions.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    GroupLocationTransactionModel model = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                    if (model != null)
                    {
                        if (selectedItem.LocationTransactions.Any(x => x.LocationReferenceId == model.LocationReferenceId))
                        {
                            model.OutputQuantity = SelectedItem.LocationTransactions.FirstOrDefault(x => x.LocationReferenceId == model.LocationReferenceId).OutputQuantity;
                        }
                    }
                    LocationTransactions.Add(model);
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

    public async Task LocationTransactionIncreaseAsync(GroupLocationTransactionModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);
                if (totalQuantity >= SelectedItem.ProductModel.StockQuantity)
                {
                    _userDialogs.Alert("Stok miktarından fazla ürün girişi yapamazsınız.", "Uyarı", "Tamam");
                    return;
                }
                else
                {
                    if (item.OutputQuantity < item.RemainingQuantity)
                    {
                        item.OutputQuantity += 1;
                    }
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

    public async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                if (item.OutputQuantity > 0)
                {
                    item.OutputQuantity -= 1;
                }
                if (item.OutputQuantity == 0)
                {
                    LocationTransactions.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).OutputQuantity = 0;
                    var Remove = SelectedItem.LocationTransactions.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId);
                    SelectedItem.LocationTransactions.Remove(Remove);
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

    public async Task LocationTransactionConfirmAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (LocationTransactions.Count > 0)
            {
                var count = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
                SelectedLocationTransactions.Clear();
                if (count > 0)
                {
                    foreach (var item in LocationTransactions.Where(x => x.OutputQuantity > 0))
                    {
                        if (SelectedItem.LocationTransactions.Any(x => x.LocationReferenceId == item.LocationReferenceId))
                        {
                            SelectedItem.LocationTransactions.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId).OutputQuantity = item.OutputQuantity;
                        }
                        else
                            SelectedItem.LocationTransactions.Add(item);
                    }
                }
                else
                {
                    SelectedItem.LocationTransactions.Clear();
                }

                SelectedItem.SubBOMQuantity = SelectedItem.LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
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

    public async Task LocationTransactionCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task LoadWarehouseLocationsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Locations.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                warehouseNumber: QuicklyBomProductBasketModel.WarehouseNumber, 
                productReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant ? QuicklyBomProductBasketModel.QuicklyBomProduct.MainItemReferenceId : QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId,
                variantReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant ? QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId : 0, 
                search: string.Empty, 
                skip: 0, 
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
                    foreach (var location in Locations)
                    {
                        var matchingItem = QuicklyBomProductBasketModel.MainLocations.FirstOrDefault(item => item.ReferenceId == location.ReferenceId);
                        if (matchingItem != null)
                        {
                            location.InputQuantity = matchingItem.InputQuantity;
                        }
                    }
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
        if (Locations.Count < 18)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
			   httpClient: httpClient,
			   firmNumber: _httpClientService.FirmNumber,
			   periodNumber: _httpClientService.PeriodNumber,
			   warehouseNumber: QuicklyBomProductBasketModel.WarehouseNumber,
			   productReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant ? QuicklyBomProductBasketModel.QuicklyBomProduct.MainItemReferenceId : QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId,
			   variantReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant ? QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId : 0,
			   search: string.Empty,
			   skip: Locations.Count,
			   take: 20
		   );
			if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
                    foreach (var location in Locations)
                    {
                        var matchingItem = quicklyBomProductBasketModel.MainLocations.FirstOrDefault(item => item.ReferenceId == location.ReferenceId);
                        if (matchingItem != null)
                        {
                            location.InputQuantity = matchingItem.InputQuantity;
                        }
                    }
                }
            }

            _userDialogs.HideHud();
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

    private async Task LocationIncreaseAsync(LocationModel locationModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            double count = (double)Locations.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);

            if (locationModel is not null)
            {
                locationModel.InputQuantity++;
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

    private async Task LocationDecraseAsync(LocationModel locationModel)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (locationModel is not null)
            {
                if (locationModel.InputQuantity > 0)
                    locationModel.InputQuantity--;

                if (locationModel.InputQuantity == 0)
                {
                    Locations.FirstOrDefault(x => x.Code == locationModel.Code).InputQuantity = 0;
                    var Remove = QuicklyBomProductBasketModel.MainLocations.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId);
                    QuicklyBomProductBasketModel.MainLocations.Remove(Remove);
                    locationModel.IsSelected = false;
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

    private async Task LocationConfirmAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (Locations.Count > 0)
            {
                double count = (double)Locations.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);

                if (QuicklyBomProductBasketModel is not null)
                {
                    if (count > 0)
                    {
                        foreach (var i in Locations.Where(x => x.InputQuantity > 0)) // Büyük olanları al
                        {
                            if (QuicklyBomProductBasketModel.MainLocations.Any(x => x.ReferenceId == i.ReferenceId))
                            {
                                QuicklyBomProductBasketModel.MainLocations.FirstOrDefault(x => x.ReferenceId == i.ReferenceId).InputQuantity = i.InputQuantity;
                            }
                            else
                            {
                                QuicklyBomProductBasketModel.MainLocations.Add(i);
                            }
                        }
                    }
                    else
                    {
                        QuicklyBomProductBasketModel.MainLocations.Clear();
                    }

                    QuicklyBomProductBasketModel.BOMQuantity = (double)QuicklyBomProductBasketModel.MainLocations.Sum(x => x.InputQuantity);

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
}