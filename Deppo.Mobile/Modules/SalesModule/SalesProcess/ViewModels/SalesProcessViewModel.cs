using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcessBottomSheetModels;
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
using DevExpress.Maui.Controls;

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

        OutputProductSalesInfoBottemSheetCommand = new Command(async () => await OutputProductSalesInfoBottemSheetAsync());
        OutputProductSalesOrderInfoBottemSheetCommand = new Command(async () => await OutputProductSalesOrderInfoBottemSheetAsync());
        ProcurementSalesInfoBottemSheetCommand = new Command(async () => await ProcurementSalesInfoBottemSheetAsync());
        ReturnSalesInfoBottemSheetCommand = new Command(async () => await ReturnSalesInfoBottemSheetAsync());
        RetrunSalesDispatchInfoBottemSheetCommand = new Command(async () => await RetrunSalesDispatchInfoBottemSheetAsync());
        ProcurementByCustomerInfoBottemSheetCommand = new Command(async () => await ProcurementByCustomerInfoBottemSheetAsync());

        ProcurementByProductInfoBottemSheetCommand = new Command(async () => await ProcurementByProductInfoBottemSheetAsync());
        ProcurementByLocationInfoBottemSheetCommand = new Command(async () => await ProcurementByLocationInfoBottemSheetAsync());
    }

    [ObservableProperty]
    private string infoTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ProcessBottomSheetModel processBottomSheetModel = new();

    public Page CurrentPage { get; set; }

    #region Commands

    public Command OutputProductSalesProcessCommand { get; }
    public Command OutputProductSalesOrderProcessCommand { get; }
    public Command ProcurementSalesProcessCommand { get; }

    public Command ReturnSalesProcessCommand { get; }
    public Command RetrunSalesDispatchProcessCommand { get; }

    public Command ProcurementByCustomerProcessCommand { get; }
    public Command ProcurementByProductProcessCommand { get; }
    public Command ProcurementByLocationProcessCommand { get; }

    #region InfoBottemSheet

    public Command OutputProductSalesInfoBottemSheetCommand { get; }
    public Command OutputProductSalesOrderInfoBottemSheetCommand { get; }
    public Command ProcurementSalesInfoBottemSheetCommand { get; }
    public Command ReturnSalesInfoBottemSheetCommand { get; }
    public Command RetrunSalesDispatchInfoBottemSheetCommand { get; }
    public Command ProcurementByCustomerInfoBottemSheetCommand { get; }
    public Command ProcurementByProductInfoBottemSheetCommand { get; }
    public Command ProcurementByLocationInfoBottemSheetCommand { get; }

    #endregion InfoBottemSheet

    #endregion Commands

    private async Task OutputProductSalesProcessAsync()
    {
        if (IsBusy)
            return;
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
        if (IsBusy)
            return;
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
        if (IsBusy)
            return;
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
        if (IsBusy)
            return;
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
        if (IsBusy)
            return;
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
        if (IsBusy)
            return;
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
        if (IsBusy)
            return;
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
        if (IsBusy)
            return;
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

    #region BottemSheetFunction

    private async Task OutputProductSalesInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Sevk İşlemleri";
        ProcessBottomSheetModel.Description = "Sevk işlemleri ürünlerin stoktan çıkışını kaydeder.\nSevk işlemleri için toptan veya perakende satış irsaliyeleri oluşturabilirsiniz.";
        ProcessBottomSheetModel.IconText = "minus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task OutputProductSalesOrderInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Siparişe Bağlı Sevk İşlemleri";
        ProcessBottomSheetModel.Description = "Mevcut bir satış irsaliyesine dayanarak ürünlerin müşteriye sevk edilmesidir.\nSiparişe bağlı sevk İşlemlerinde toptan veya perakende satış irsaliyeleri oluşturabilirsiniz.";
        ProcessBottomSheetModel.IconText = "minus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ProcurementSalesInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Toplanan Ürünlerin Sevk İşlemleri";
        ProcessBottomSheetModel.Description = "Raftan toplanan ürünlerle satış irasaliyeleri oluşturabilirsiniz";
        ProcessBottomSheetModel.IconText = "truck";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ReturnSalesInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Satış İade İşlemleri";
        ProcessBottomSheetModel.Description = "Satış iade işlemleri için satış irsaliyeleri oluşturmak için kullanabilirsiniz";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task RetrunSalesDispatchInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "İrsaliyeye Bağlı Satış İade İşlemleri";
        ProcessBottomSheetModel.Description = "İrsaliyeye bağlı satış iade işlemleri için stoğa giriş işlemlerini kaydetmek için kullanabilirsiniz";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ProcurementByCustomerInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Müşteriye Göre Ürün Toplama";
        ProcessBottomSheetModel.Description = "Seçilen müşteriye göre ürünleri gruplayarak ürün toplama işlemi gerçekleştirebilirsiniz";
        ProcessBottomSheetModel.IconText = "user";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ProcurementByProductInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Malzemeye Göre Ürün Toplama";
        ProcessBottomSheetModel.Description = "Malzemeye göre gruplayarak raftan ürün toplama işlemi gerçekleştirebilirsiniz";
        ProcessBottomSheetModel.IconText = "cubes";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ProcurementByLocationInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Stok Yerine(Raf) Göre Ürün Toplama";
        ProcessBottomSheetModel.Description = "Belirlenen raf yerlerine göre raftan ürün toplama işlemi gerçekleştirebilirsiniz.";
        ProcessBottomSheetModel.IconText = "LocationDot";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    #endregion BottemSheetFunction
}