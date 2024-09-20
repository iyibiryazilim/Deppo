using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using DevExpress.Maui.Controls;
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

        IncreaseCommand = new Command<VirmanBasketModel>(async (item) => await IncreaseAsync(item));
        DecreaseCommand = new Command(async () => await DecreaseAsync());
        // DeleteItemCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await DeleteItemAsync(item));
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());

        LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
        LocationTransactionIncreaseCommand = new Command<LocationTransactionModel>(item => LocationTransactionIncreaseAsync(item));
        LocationTransactionDecreaseCommand = new Command<LocationTransactionModel>(item => LocationTransactionDecreaseAsync(item));
        ConfirmLocationTransactionCommand = new Command(ConfirmLocationTransactionAsync);
        LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

        //LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
        //LocationCloseCommand = new Command(async () => await LocationCloseAsync());
        //LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
        //LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
        //LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));
    }

    public Page CurrentPage { get; set; } = null!;

    public Command ShowProductViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command DeleteItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public Command LoadMoreLocationTransactionsCommand { get; }
    public Command LocationTransactionIncreaseCommand { get; }
    public Command LocationTransactionDecreaseCommand { get; }
    public Command ConfirmLocationTransactionCommand { get; }
    public Command LocationTransactionCloseCommand { get; }

    public Command LoadMoreLocationsCommand { get; }
    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command<LocationModel> LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }

    private async Task DecreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            //if (item.OutVirmanQuantity > 0)
            //{
            //    if (item.OutVirmanProduct.TrackingType == 1 || item.OutVirmanProduct.TrackingType == 2)
            //    {
            //        await LoadLocationTransactionsAsync();
            //        CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
            //    }
            //    else
            //    {
            //        item.OutVirmanQuantity = item.OutVirmanQuantity + 1;
            //        item.InVirmanQuantity = item.OutVirmanQuantity;
            //    }
            //}
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

    private async Task IncreaseAsync(VirmanBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (item.OutVirmanProduct.TrackingType == 1 || item.OutVirmanProduct.TrackingType == 2)
            {
                await LoadLocationTransactionsAsync();
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else
            {
                item.OutVirmanQuantity = item.OutVirmanQuantity + 1;
                item.InVirmanQuantity = item.OutVirmanQuantity;
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
            SelectedLocationTransactions.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetOutputObjectsAsync(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: VirmanBasketModel.OutVirmanProduct.ReferenceId, warehouseNumber: VirmanBasketModel.OutVirmanWarehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SelectedLocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
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
        try
        {
            _userDialogs.ShowLoading("Load Location Items...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetOutputObjectsAsync(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, productReferenceId: VirmanBasketModel.OutVirmanProduct.ReferenceId, warehouseNumber: VirmanBasketModel.OutVirmanWarehouse.Number);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SelectedLocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
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

    private void LocationTransactionIncreaseAsync(LocationTransactionModel item)
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
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LocationTransactionDecreaseAsync(LocationTransactionModel item)
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
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ConfirmLocationTransactionAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (SelectedLocationTransactions.Count > 0)
            {
                SelectedLocationTransactions.ToList().AddRange(LocationTransactions.Where(x => x.OutputQuantity > 0));

                if (virmanBasketModel.OutVirmanProduct.LocTracking == 0)
                {
                    foreach (var item in SelectedLocationTransactions)
                    {
                        virmanBasketModel.OutVirmanProduct.LocationTransactionModels.Add(new LocationTransactionModel
                        {
                            ReferenceId = item.ReferenceId,
                            SerilotCode = item.SerilotCode,
                            SerilotName = item.SerilotName,
                            TransactionFicheReferenceId = item.TransactionFicheReferenceId,
                            TransactionReferenceId = item.TransactionReferenceId,
                            InTransactionReferenceId = item.InTransactionReferenceId,
                            Quantity = item.OutputQuantity,
                            InSerilotTransactionReferenceId = item.InSerilotTransactionReferenceId,
                            RemainingQuantity = item.Quantity - item.OutputQuantity
                        });
                    }

                    var totalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
                    VirmanBasketModel.OutVirmanQuantity = totalOutputQuantity;
                    VirmanBasketModel.InVirmanQuantity = totalOutputQuantity;

                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
                }
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

            await Shell.Current.GoToAsync($"{nameof(ReturnPurchaseDispatchFormView)}", new Dictionary<string, object>
            {
                [nameof(VirmanBasketModel)] = VirmanBasketModel
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

            //     VirmanBasketModel.IsSelected = false;

            //   foreach (var item in SelectedPurchaseTransactions)
            //  {
            //    item.IsSelected = false;
            ///  }
            //  SelectedLocationTransactions.Clear();
            virmanBasketModel.OutVirmanQuantity = 0;
            virmanBasketModel.InVirmanQuantity = 0;
            virmanBasketModel.OutVirmanProduct = null!;
            virmanBasketModel.InVirmanProduct = null!;
            virmanBasketModel.OutVirmanWarehouse = null!;
            virmanBasketModel.InVirmanWarehouse = null!;
            virmanBasketModel = null;
            SelectedLocationTransactions.Clear();
            //  SelectedPurchaseTransactions.Clear();
            //   Items.Clear();
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

    /*
    private async Task DeleteItemAsync(ReturnPurchaseBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

            if (!result)
                return;

            if (SelectedItem == item)
            {
                SelectedItem.IsSelected = false;
                SelectedItem = null;
            }

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
    }*/
}