using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcessBottomSheetModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.Views;
using DevExpress.Maui.Controls;
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

        ProductionInputInfoBottemSheetCommand = new Command(async () => await ProductionInputInfoBottemSheetAsync());
        InputPurchaseOrderInfoBottemSheetCommand = new Command(async () => await InputPurchaseOrderInfoBottemSheetAsync());
        ReturnPurchaseDispatchInfoBottemSheetCommand = new Command(async () => await ReturnPurchaseDispatchInfoBottemSheetAsync());
        ReturnPurchaseProcessInfoBottemSheetCommand = new Command(async () => await ReturnPurchaseProcessInfoBottemSheetAsync());
    }

    [ObservableProperty]
    private string infoTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ProcessBottomSheetModel processBottomSheetModel = new();

    public Page CurrentPage { get; set; }

    public Command ProductionInputCommand { get; }
    public Command InputPurchaseOrderProcessCommand { get; }
    public Command ReturnPurchaseProcessCommand { get; }
    public Command ReturnPurchaseDispatchProcessCommand { get; }

    #region InfoBottemSheet

    public Command ProductionInputInfoBottemSheetCommand { get; }
    public Command InputPurchaseOrderInfoBottemSheetCommand { get; }
    public Command ReturnPurchaseDispatchInfoBottemSheetCommand { get; }
    public Command ReturnPurchaseProcessInfoBottemSheetCommand { get; }

    #endregion InfoBottemSheet

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

    #region BottemSheetFunction

    private async Task ProductionInputInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Mal Kabul İşlemleri";
        ProcessBottomSheetModel.Description = "Mal kabul işlemlerinde satınalma irsaliyesi kaydetemek için kullanabilirsiniz.";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task InputPurchaseOrderInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = " Siparişe Bağlı Mal Kabul İşlemleri";
        ProcessBottomSheetModel.Description = "Siparişe bağlı mal kabul işlemlerini satınalma irsaliyelerini kaydetmek için kullanılır.";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ReturnPurchaseDispatchInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "İrsaliyeye Bağlı SatınAlma İade İşlemleri";
        ProcessBottomSheetModel.Description = "İrsaliyeye bağlı satınAlma iade İşlemleri kaydetmek için kullanılır.\nİade işlemleri için kullanabilirsiniz.";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ReturnPurchaseProcessInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Satınalma İade İşlemleri";
        ProcessBottomSheetModel.Description = "Satınalma iade işlemlerinde kullanılır.\nİade işlemleri için kullanabilirsiniz.";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    #endregion BottemSheetFunction
}