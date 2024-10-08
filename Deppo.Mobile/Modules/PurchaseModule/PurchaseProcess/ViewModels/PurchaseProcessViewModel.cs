using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.Views;
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

        Title = "Satınalma İşlemleri";

        ProductionInputCommand = new Command(async () => await ProductionInputAsync());
        InputPurchaseOrderProcessCommand = new Command(async () => await InputPurchaseOrderProcessAsync());
        ReturnPurchaseProcessCommand = new Command(async () => await ReturnPurchaseProcessAsync());
        ReturnPurchaseDispatchProcessCommand = new Command(async () => await ReturnPurchaseDispatchProcessAsync());
    }

    public Command ProductionInputCommand { get; }
    public Command InputPurchaseOrderProcessCommand { get; }
    public Command ReturnPurchaseProcessCommand { get;}
    public Command ReturnPurchaseDispatchProcessCommand { get; }

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
            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessWarehouseListView)}");
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

    private async Task ReturnPurchaseDispatchProcessAsync()
    {
        if (IsBusy)
            return;
		try
		{
			IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ReturnPurchaseDispatchWarehouseListView)}");
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

	private async Task ReturnPurchaseProcessAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ReturnPurchaseWarehouseListView)}");
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