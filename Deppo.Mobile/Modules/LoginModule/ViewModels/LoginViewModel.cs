using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.LoginModule.Views;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.LoginModule.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IAuthenticationService _authenticationService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly IAuthenticateSysService _authenticateSysService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IApplicationUserService _applicationUserService;

    public LoginViewModel(
        IHttpClientService httpClientService,
        IUserDialogs userDialogs,
        IAuthenticationService authenticationService,
        IHttpClientSysService httpClientSysService,
        IAuthenticateSysService authenticateSysService,
        IServiceProvider serviceProvider,
        IApplicationUserService applicationUserService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _authenticationService = authenticationService;
        _httpClientSysService = httpClientSysService;
        _authenticateSysService = authenticateSysService;
        _serviceProvider = serviceProvider;
        _applicationUserService = applicationUserService;

        LoginCommand = new Command(async () => await LoginAsync());
        ShowParameterCommand = new Command<BottomSheet>(ShowParameterAsync);
        SaveCommand = new Command(async () => await SaveAsync());
        LogoutCommand = new Command(async () => await LogoutAsync());
    }

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string baseUri = string.Empty;

    public Command LoginCommand { get; }
    public Command<BottomSheet> ShowParameterCommand { get; }
    public Command SaveCommand { get; }
    public Command LogoutCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (string.IsNullOrEmpty(UserName))
            {
                _userDialogs.Alert("Please enter username", "Error", "OK");
                return;
            }

            _userDialogs.Loading("Oturum açılıyor..");
            await Task.Delay(1000);
            var sysLogged = await AuthenticateSysAsync();
            if (sysLogged)
            {
                var helixLogged = await AuthenticateHelixAsync();
                if (helixLogged)
                {
                    _httpClientSysService.UserName = UserName;
                    var companyListViewModel = IPlatformApplication.Current.Services.GetRequiredService<CompanyListViewModel>();
                    Application.Current.MainPage = new CompanyListView(companyListViewModel);
                }
                else
                    _userDialogs.Alert("Sunucu ile bağlantı başarısız..", "Helix Hata");
            }
            else
                _userDialogs.Alert("Kullanıcı adınız veya parolanız geçersiz..", "Oturum Açma Hata");

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            _userDialogs.Alert($"{ex.Message}");
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

    private async Task LogoutAsync()
    {
        if (IsBusy)
            return;
        try
        {
            //SecureStorage.RemoveAll();

            _userDialogs.Loading("Oturum kapatılıyor...");
            await Task.Delay(1000);
            Application.Current.MainPage = _serviceProvider.GetRequiredService<LoginView>();

            _userDialogs.HideHud();
        }
        catch (Exception ex)
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

    private async Task<bool> AuthenticateSysAsync()
    {
        bool isLoggedIn = false;

        try
        {
            string baseUri = "http://172.16.1.3:1923";//await SecureStorage.GetAsync("baseUri");
            if (string.IsNullOrEmpty(baseUri))
                _httpClientSysService.BaseUri = "http://172.16.1.3:1923";
            else
                _httpClientSysService.BaseUri = baseUri;

            var httpClient = _httpClientSysService.GetOrCreateHttpClient();
            var token = await _authenticateSysService.AuthenticateAsync(httpClient, UserName, Password);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClientSysService.Token = token;
                var currentUser = await GetCurrentUserAsync(UserName);
                _httpClientSysService.UserOid = currentUser.Oid;
                isLoggedIn = true;
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert($"{ex.Message}", "Login Error");
        }

        return isLoggedIn;
    }

    private async Task<ApplicationUser> GetCurrentUserAsync(string userName)
    {
        try
        {
            var httpClient = _httpClientSysService.GetOrCreateHttpClient();
            string filter = $"filter=UserName eq '{userName}'&$expand=Image";
            var result = await _applicationUserService.GetAllAsync(httpClient, filter);

            if (result.Any())
                return result.FirstOrDefault();
            else
                return new ApplicationUser();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            return new ApplicationUser();
        }
    }

    private async Task<bool> AuthenticateHelixAsync()
    {
        bool isLoggedIn = false;

        try
        {
            string baseUri = "http://172.16.1.25:52789";//await SecureStorage.GetAsync("baseUri");
            if (string.IsNullOrEmpty(baseUri))
                _httpClientService.BaseUri = "http://172.16.1.25:52789";
            else
                _httpClientService.BaseUri = baseUri;

            _userDialogs.Loading("Loading...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var token = await _authenticationService.Authenticate(httpClient, "Admin", "");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClientService.Token = token;
                isLoggedIn = true;
            }
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Error", "Ok");
        }

        return isLoggedIn;
    }
}