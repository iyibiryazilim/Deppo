using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ViewModels;

public partial class PurchaseProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public PurchaseProcessViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "İşlemler";

        ProductionInputCommand = new Command(async () => await ProductionInputAsync());
        InputPurchaseOrderProcessCommand = new Command(async () => await InputPurchaseOrderProcessAsync());
    }

    public Command ProductionInputCommand { get; }

    public Command InputPurchaseOrderProcessCommand { get; }

    private async Task ProductionInputAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessWarehouseListView)}", new Dictionary<string, object>
        {
            {nameof(InputProductProcessType), InputProductProcessType.ProductionInputProcess}
        });
    }

    private async Task InputPurchaseOrderProcessAsync()
    {
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessWarehouseListView)}", new Dictionary<string, object>
        {
            {nameof(InputProductProcessType), InputProductProcessType.ProductionInputProcess}
        });
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