using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ViewModels;

public partial class QuicklyProductionProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public QuicklyProductionProcessViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;

        Title = "Hızlı Üretim İşlemleri";

        QuicklyProductionManuelCommand = new Command(async () => await QuicklyProductionManuelAsync());
        QuicklyProductionWorkOrderCommand = new Command(async () => await QuicklyProductionWorkOrderAsync());
        ManuelReworkProcessCommand = new Command(async () => await ManuelReworkProcessAsync());
    }

    public Command QuicklyProductionManuelCommand { get; }
    public Command QuicklyProductionWorkOrderCommand { get; }
    public Command ManuelReworkProcessCommand { get; }

    public async Task QuicklyProductionManuelAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(ManuelProductListView)}", new Dictionary<string, object> { });
    }

    public async Task QuicklyProductionWorkOrderAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(WorkOrderProductListView)}", new Dictionary<string, object> { });
    }

    public async Task ManuelReworkProcessAsync()
	{
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessOutWarehouseListView)}");
        }
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
	}
}