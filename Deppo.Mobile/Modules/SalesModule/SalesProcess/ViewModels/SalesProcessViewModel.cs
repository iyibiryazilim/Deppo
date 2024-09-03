using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ViewModels;

public partial class SalesProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public SalesProcessViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "İşlemler";

        OutputProductSalesOrderProcessCommand = new Command(async () => await OutputProductSalesOrderProcessAsync());
    }

    #region Commands
    public Command OutputProductSalesOrderProcessCommand { get; }
    #endregion

    async Task OutputProductSalesOrderProcessAsync()
    {
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessWarehouseListView)}");
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
