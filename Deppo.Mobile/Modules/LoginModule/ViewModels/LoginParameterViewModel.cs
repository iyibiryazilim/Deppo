using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.LoginModule.ViewModels;

public partial class LoginParameterViewModel : BaseViewModel
{
private readonly IUserDialogs _userDialogs;

    public LoginParameterViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;
    }

    [ObservableProperty]
    string baseUri = string.Empty;

    public Command SaveCommand => new Command(async () =>
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (string.IsNullOrEmpty(BaseUri))
            {
                _userDialogs.Alert("Please enter base uri", "Error", "OK");
                return;
            }
            else
            {
                // Save base uri to local storage
                await SecureStorage.SetAsync("baseUri", BaseUri);
                _userDialogs.Alert("Base uri saved successfully", "Success", "OK");
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Error", "OK");
        }
    });
}
