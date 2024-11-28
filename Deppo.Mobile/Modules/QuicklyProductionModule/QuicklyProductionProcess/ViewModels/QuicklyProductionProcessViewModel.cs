using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcessBottomSheetModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;
using DevExpress.Maui.Controls;
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
        WorkOrderReworkProcessCommand = new Command(async () => await WorkOrderReworkProcessAsync());

        QuicklyProductionManuelBottemSheetCommand = new Command(async () => await QuicklyProductionManuelBottemSheetAsync());
        QuicklyProductionWorkOrderBottemSheetCommand = new Command(async () => await QuicklyProductionWorkOrderBottemSheetAsync());
        ManuelReworkBottemSheetCommand = new Command(async () => await ManuelReworkBottemSheetAsync());
        WorkOrderReworkBottemSheetCommand = new Command(async () => await WorkOrderReworkBottemSheetAsync());
    }

    [ObservableProperty]
    private string infoTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ProcessBottomSheetModel processBottomSheetModel = new();

    public Page CurrentPage { get; set; }

    public Command QuicklyProductionManuelCommand { get; }
    public Command QuicklyProductionWorkOrderCommand { get; }
    public Command ManuelReworkProcessCommand { get; }
    public Command WorkOrderReworkProcessCommand { get; }

    #region BottemSheetCommand

    public Command QuicklyProductionManuelBottemSheetCommand { get; }
    public Command QuicklyProductionWorkOrderBottemSheetCommand { get; }

    public Command ManuelReworkBottemSheetCommand { get; }
    public Command WorkOrderReworkBottemSheetCommand { get; }

    #endregion BottemSheetCommand

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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task WorkOrderReworkProcessAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(WorkOrderReworkProcessProductListView)}");
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

    #region BottemSheetCommand

    private async Task QuicklyProductionManuelBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Manuel Hızlı Üretim";
        ProcessBottomSheetModel.Description = "Kullanıcılar, üretim formüllerini veya ihtiyaç duyulan malzemeleri manuel olarak seçerek üretimi tamamlayabilirler.\nÜretimden giriş ve sarf fişleri oluşturulur.";
        ProcessBottomSheetModel.IconText = "hand";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ManuelReworkBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Manuel Rework";
        ProcessBottomSheetModel.Description = "Kullanıcılar, üretim formüllerini veya ihtiyaç duyulan malzemeleri manuel olarak seçerek rework işlemini tamamlayabilirler.";
        ProcessBottomSheetModel.IconText = "hand";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task QuicklyProductionWorkOrderBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "İş Emrine Bağlı Hızlı Üretim";
        ProcessBottomSheetModel.Description = "İş emrine bağlı hızlı üretim işlemi, iş emrindeki bilgilere dayanarak, belirlenen malzemelerin otomatik olarak stoktan düşmesini ve mamullerin stoklara girmesini sağlar.\nÜretimden giriş ve sarf fişleri oluşturulur.";
        ProcessBottomSheetModel.IconText = "calculator";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task WorkOrderReworkBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "İş Emrine Bağlı Hızlı Rework";
        ProcessBottomSheetModel.Description = "İş emrine bağlı hızlı rework, hatalı veya kusurlu ürünlerin yeniden işlenmesi için yapılan hızlı bir üretim sürecidir.";
        ProcessBottomSheetModel.IconText = "calculator";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    #endregion BottemSheetCommand
}