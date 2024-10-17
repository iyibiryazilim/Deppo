using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;

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
    }

    public Command OutputOutsourceTransferCommand { get; }
    public Command OutputOutsourceWorkOrderCommand { get; }
    public Command InputOutsourceTransferCommand { get; }
    public Command InputOutsourceWorkOrderCommand { get; }

    private async Task OutputOutsourceTransferAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferWarehouseListView)}");
    }

    private async Task InputOutsourceTransferAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferWarehouseListView)}");
    }
}