using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcessBottomSheetModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ViewModels;

public partial class CountingProcessViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;

    public CountingProcessViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;

        Title = "Sayım İşlemleri";

        WarehouseCountingProcessCommand = new Command(async () => await WarehouseCountingProcessAsync());
        ProductCountingProcessCommand = new Command(async () => await ProductCountingProcessAsync());

        WarehouseCountingInfoBottemSheetCommand = new Command(async () => await WarehouseCountingInfoBottemSheetAsync());
        ProductCountingInfoBottemSheetCommand = new Command(async () => await ProductCountingInfoBottemSheetAsync());
    }

    [ObservableProperty]
    private string infoTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ProcessBottomSheetModel processBottomSheetModel = new();

    public Page CurrentPage { get; set; }

    public Command WarehouseCountingProcessCommand { get; }
    public Command ProductCountingProcessCommand { get; }

    #region InfoBottemSheetCommand

    public Command WarehouseCountingInfoBottemSheetCommand { get; }
    public Command ProductCountingInfoBottemSheetCommand { get; }

    #endregion InfoBottemSheetCommand

    private async Task WarehouseCountingProcessAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(WarehouseCountingWarehouseListView)}");
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ProductCountingProcessAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ProductCountingProductListView)}");
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #region BottemSheetFunction

    private async Task WarehouseCountingInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Ambara Göre Sayım İşlemi";
        ProcessBottomSheetModel.Description = "Belirli bir ambarın stoklarını sayarak, ambar bazında stok doğruluğunu sağlama işlemidir.\nSayım fazlası veya sayım eksiği fişleri oluşturabilirsiniz.";
        ProcessBottomSheetModel.IconText = "calculator";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ProductCountingInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Ürüne Göre Sayım İşlemi";
        ProcessBottomSheetModel.Description = "Belirli ürünlerin stok miktarını sayarak, ürün bazında doğruluğunu kontrol etme işlemidir.\nSayım fazlası veya sayım eksiği fişleri oluşturabilirsiniz.";
        ProcessBottomSheetModel.IconText = "calculator";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    #endregion BottemSheetFunction
}