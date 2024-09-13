using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.LoginModule.Views;
using DevExpress.Maui.Controls;

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
        ShowParameterCommand = new Command<BottomSheet>(ShowParameterAsync);
        SaveCommand = new Command(async () => await SaveAsync());
    }

    [ObservableProperty]
    string userName = string.Empty;

    [ObservableProperty]
    string password = string.Empty;

    [ObservableProperty]
    string baseUri = string.Empty;

    public Command LoginCommand { get; }
    public Command<BottomSheet> ShowParameterCommand { get; }
    public Command SaveCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string baseUri = "http://172.16.1.25:52789";//await SecureStorage.GetAsync("baseUri");
            if (string.IsNullOrEmpty(baseUri))
                _httpClientService.BaseUri = "http://172.16.1.25:52789";
            else
                _httpClientService.BaseUri = baseUri;

            if (string.IsNullOrEmpty(UserName))
            {
                _userDialogs.Alert("Please enter username", "Error", "OK");
                return;
            }
            _userDialogs.Loading("Loading...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var token = await _authenticationService.Authenticate(httpClient, UserName, Password);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClientService.Token = token;
                //await SecureStorage.SetAsync("token", token);
                var companyListViewModel = IPlatformApplication.Current.Services.GetRequiredService<CompanyListViewModel>();
                Application.Current.MainPage = new CompanyListView(companyListViewModel);

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync("Invalid username or password", "Error", "Ok");
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Error", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ShowParameterAsync(BottomSheet bottomSheet)
    {
        bottomSheet.State = BottomSheetState.HalfExpanded;
        //await Shell.Current.GoToAsync($"{nameof(LoginParameterView)}");
    }

    private async Task SaveAsync()
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
    }
}
