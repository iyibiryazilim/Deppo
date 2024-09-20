using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.ViewModels;

public partial class OutsourceProcessViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    public OutsourceProcessViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;

        Title = "Fason İşlemleri";
    }
}
