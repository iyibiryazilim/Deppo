using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Helpers.MVVMHelper;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    string title = string.Empty;

    [ObservableProperty]
    bool isBusy;

    public bool IsNotBusy => !IsBusy;

}
