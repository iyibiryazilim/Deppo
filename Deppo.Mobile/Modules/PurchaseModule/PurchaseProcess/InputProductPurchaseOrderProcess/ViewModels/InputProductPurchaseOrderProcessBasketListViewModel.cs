using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    WarehouseModel warehouseModel;

    [ObservableProperty]
    PurchaseSupplier purchaseSupplier;

    [ObservableProperty]
    ObservableCollection<InputPurchaseBasketModel> items;

    public InputProductPurchaseOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService, ISeriLotService seriLotService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _locationService = locationService;
        _seriLotService = seriLotService;

        Title = "Satınalma Sepeti";

        IncreaseCommand = new Command<InputPurchaseBasketModel>(async (x) => await IncreaseAsync(x));
        LocationCloseCommand = new Command(async () => await LocationCloseAsync());


    }

    public Page CurrentPage { get; set; }

    public ObservableCollection<LocationModel> Locations { get; } = new();


    public Command<InputPurchaseBasketModel> DeleteItemCommand { get; }
    public Command<InputPurchaseBasketModel> IncreaseCommand { get; }
    public Command<InputPurchaseBasketModel> DecreaseCommand { get; }
    public Command BackCommand { get; }
    public Command NextViewCommand { get; }
    public Command LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }

    private async Task IncreaseAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (inputPurchaseBasketModel.LocTracking == 1)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadWarehouseLocationsAsync(inputPurchaseBasketModel);
                    CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
                });

            }
            else if (inputPurchaseBasketModel.TrackingType == 1)
            {
                await LoadWarehouseLocationsAsync(inputPurchaseBasketModel);
                CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task LocationCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
        });
    }


}