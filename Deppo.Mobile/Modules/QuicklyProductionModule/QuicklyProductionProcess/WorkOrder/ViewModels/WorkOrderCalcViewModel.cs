using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
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

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels;


[QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
public partial class WorkOrderCalcViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseService _warehouseService;
    private readonly IQuicklyBomService _quicklyBomService;

    [ObservableProperty]
    QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;

    public ObservableCollection<BOMSubProductModel> Items { get; } = new();


    [ObservableProperty]
    SeriLotTransactionModel? selectedSeriLotTransaction;
    [ObservableProperty]
    public ObservableCollection<LocationTransactionModel> selectedLocationTransactions = new();
    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();



    public WorkOrderCalcViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IWarehouseService warehouseService, IQuicklyBomService quicklyBomService)
    {
        _httpClientService = httpClientService;
        _locationTransactionService = locationTransactionService;
        _userDialogs = userDialogs;
        _warehouseService = warehouseService;
        Title = "Ürün Detayı";


        IncreaseCommand = new Command<QuicklyBomProductBasketModel>(async (item) => await IncreaseAsync(item));


        DecreaseCommand = new Command<QuicklyBomProductBasketModel>(async (item) => await DecreaseAsync(item));


        NextViewCommand = new Command(async () => await NextViewAsync());


        BackCommand = new Command(async () => await BackAsync());


        LoadItemsSubCommand = new Command(async () => await LoadItemsSubAsync());
        _quicklyBomService = quicklyBomService;
    }




    #region Commands
    public ContentPage CurrentPage { get; set; } = null!;
    public Command ShowProductViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command DeleteItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }




    public Command LoadItemsSubCommand { get; }
    #endregion



    private async Task IncreaseAsync(QuicklyBomProductBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                item.BOMQuantity += 1;
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

    private async Task DecreaseAsync(QuicklyBomProductBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                if (item.BOMQuantity > 0)
                {
                    item.BOMQuantity -= 1;
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

                QuicklyBomProductBasketModel = null;
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

    private async Task LoadItemsSubAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            Items.Clear();
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyBomService.GetObjectsWorkSubProducts(httpClient, firmNumber: _httpClientService.FirmNumber, mainProductReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId, periodNumber: _httpClientService.PeriodNumber , take: 1000);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<BOMSubProductModel>(product);

                    if(item is not null)
                    {
                        QuicklyBomSubProductModel subproducts = new QuicklyBomSubProductModel();
                        subproducts.SubBOMQuantity = item.Amount;
                        subproducts.ProductModel = item;
                        QuicklyBomProductBasketModel.SubProducts.Add(subproducts);
                    }
                }
               
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
   







}