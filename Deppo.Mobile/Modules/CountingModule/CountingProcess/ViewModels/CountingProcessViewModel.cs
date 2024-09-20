using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ViewModels;

public partial class CountingProcessViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    public CountingProcessViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;

        Title = "Sayım İşlemleri";
    }
}
