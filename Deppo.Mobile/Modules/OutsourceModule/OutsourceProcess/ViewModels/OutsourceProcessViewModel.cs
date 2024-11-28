using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcessBottomSheetModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.ViewModels;

public partial class OutsourceProcessViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;

    public OutsourceProcessViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;

        Title = "Fason İşlemleri";

        OutputOutsourceTransferCommand = new Command(async () => await OutputOutsourceTransferAsync());
        InputOutsourceTransferCommand = new Command(async () => await InputOutsourceTransferAsync());

        InputOutsourceInfoBottemSheetCommand = new Command(async () => await InputOutsourceInfoBottemSheetAsync());
        OutputOutsourceInfoBottemSheetCommand = new Command(async () => await OutputOutsourceInfoBottemSheetAsync());
    }

    [ObservableProperty]
    private string infoTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ProcessBottomSheetModel processBottomSheetModel = new();

    public Page CurrentPage { get; set; }

    public Command OutputOutsourceTransferCommand { get; }
    public Command OutputOutsourceWorkOrderCommand { get; }
    public Command InputOutsourceTransferCommand { get; }
    public Command InputOutsourceWorkOrderCommand { get; }

    #region BottemSheetFunction

    public Command InputOutsourceInfoBottemSheetCommand { get; }

    public Command OutputOutsourceInfoBottemSheetCommand { get; }

    #endregion BottemSheetFunction

    private async Task OutputOutsourceTransferAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferWarehouseListView)}");
    }

    private async Task InputOutsourceTransferAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferWarehouseListView)}");
    }

    private async Task InputOutsourceInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Fason Kabul İşlemleri";
        ProcessBottomSheetModel.Description = "Diğer firmalarda işlenen ürünlerin geri alınması işlemidir.\nGiriş fişleri oluşturabilirsiniz";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task OutputOutsourceInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Fason Sevk İşlemleri";
        ProcessBottomSheetModel.Description = "Üretim için dış firmaya malzeme gönderme işlemidir.\nÇıkış fişleri oluşturabilirsiniz";
        ProcessBottomSheetModel.IconText = "arrow-right-arrow-left";
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }
}