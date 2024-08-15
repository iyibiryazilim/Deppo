using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.LoginModule.Views;

namespace Deppo.Mobile.Modules.LoginModule.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IAuthenticationService _authenticationService;

    public LoginViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IAuthenticationService authenticationService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _authenticationService = authenticationService;

        LoginCommand = new Command(async () => await LoginAsync());
        ShowParameterCommand = new Command(async () => await ShowParameterAsync());
    }

    [ObservableProperty]
    string userName = string.Empty;

    [ObservableProperty]
    string password = string.Empty;

    public Command LoginCommand { get; }
    public Command ShowParameterCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string baseUri = await SecureStorage.GetAsync("baseUri");
            if (string.IsNullOrEmpty(baseUri))            
                _httpClientService.BaseUri = "http://172.16.1.25:52789";
            else
                _httpClientService.BaseUri = baseUri;
            

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var token = await _authenticationService.Authenticate(httpClient, UserName, Password);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClientService.Token = token;
                await SecureStorage.SetAsync("token", token);
                _userDialogs.Loading("Loading...");
                await Task.Delay(1000);
                Application.Current.MainPage = new AppShell();

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            else
            {
                await _userDialogs.AlertAsync("Invalid username or password", "Error", "Ok");
            }
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Error", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ShowParameterAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(LoginParameterView)}");
    }
}
