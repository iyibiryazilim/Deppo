using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ViewModels;

public partial class SalesProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public SalesProcessViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Satış İşlemleri";

        OutputProductSalesProcessCommand = new Command(async () => await OutputProductSalesProcessAsync());
        OutputProductSalesOrderProcessCommand = new Command(async () => await OutputProductSalesOrderProcessAsync());
        ReturnSalesProcessCommand = new Command(async () => await ReturnSalesProcessAsync());
        RetrunSalesDispatchProcessCommand = new Command(async () => await RetrunSalesDispatchProcessAsync());
    }

    #region Commands

    public Command OutputProductSalesProcessCommand { get; }
    public Command OutputProductSalesOrderProcessCommand { get; }

    public Command ReturnSalesProcessCommand { get; }
    public Command RetrunSalesDispatchProcessCommand { get; }

    #endregion Commands

    private async Task OutputProductSalesProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(OutputProductSalesProcessWarehouseListView)}");
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

    private async Task OutputProductSalesOrderProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessWarehouseListView)}");
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

    private async Task ReturnSalesProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ReturnSalesWarehouseListView)}");
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

    private async Task RetrunSalesDispatchProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchWarehouseListView)}");
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