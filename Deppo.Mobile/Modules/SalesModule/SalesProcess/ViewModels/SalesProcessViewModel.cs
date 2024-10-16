using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
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
        ProcurementSalesProcessCommand = new Command(async () => await ProcurementSalesProcessAsync());
        ReturnSalesProcessCommand = new Command(async () => await ReturnSalesProcessAsync());
        RetrunSalesDispatchProcessCommand = new Command(async () => await RetrunSalesDispatchProcessAsync());
        ProcurementByCustomerProcessCommand = new Command(async () => await ProcurementByCustomerProcessAsync());
        ProcurementByProductProcessCommand = new Command(async () => await ProcurementByProductProcessAsync());
        ProcurementByLocationProcessCommand = new Command(async () => await ProcurementByLocationProcessAsync());
    }

    #region Commands

    public Command OutputProductSalesProcessCommand { get; }
    public Command OutputProductSalesOrderProcessCommand { get; }
    public Command ProcurementSalesProcessCommand { get; }

    public Command ReturnSalesProcessCommand { get; }
    public Command RetrunSalesDispatchProcessCommand { get; }

    public Command ProcurementByCustomerProcessCommand { get; }
    public Command ProcurementByProductProcessCommand { get; }
    public Command ProcurementByLocationProcessCommand { get; }

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

    private async Task ProcurementSalesProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ProcurementSalesProcessWarehouseListView)}");
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

    private async Task ProcurementByCustomerProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ProcurementByCustomerWarehouseListView)}");
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

    private async Task ProcurementByProductProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ProcurementByProductWarehouseListView)}");
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

    private async Task ProcurementByLocationProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationWarehouseListView)}");
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