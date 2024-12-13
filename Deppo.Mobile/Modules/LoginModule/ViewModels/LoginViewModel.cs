using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;
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
    private readonly IConnectionParameterService _connectionParameterService;

	[ObservableProperty]
    private ApplicationUser applicationUser;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string baseUri = string.Empty;

    [ObservableProperty]
    string portNumber = string.Empty;
    [ObservableProperty]
    string url = string.Empty;
    [ObservableProperty]
    string urlProtocol = string.Empty;

	public LoginViewModel(
		IHttpClientService httpClientService,
		IUserDialogs userDialogs,
		IAuthenticationService authenticationService,
		IHttpClientSysService httpClientSysService,
		IAuthenticateSysService authenticateSysService,
		IServiceProvider serviceProvider,
		IApplicationUserService applicationUserService,
		IConnectionParameterService connectionParameterService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_authenticationService = authenticationService;
		_httpClientSysService = httpClientSysService;
		_authenticateSysService = authenticateSysService;
		_serviceProvider = serviceProvider;
		_applicationUserService = applicationUserService;
		_connectionParameterService = connectionParameterService;

		LoginCommand = new Command(async () => await LoginAsync());
		ShowParameterCommand = new Command<BottomSheet>(async (bottomSheet) => await ShowParameterAsync(bottomSheet));
		SaveCommand = new Command(async () => await SaveAsync());
		LogoutCommand = new Command(async () => await LogoutAsync());
		ProfileTappedCommand = new Command(async () => await ProfileTappedAsync());
	}
	public Page CurrentPage { get; set; } = null!;


    public Command LoginCommand { get; }
    public Command<BottomSheet> ShowParameterCommand { get; }
    public Command SaveCommand { get; }
    public Command LogoutCommand { get; }
    public Command ProfileTappedCommand { get; }

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

	private async Task ShowParameterAsync(BottomSheet bottomSheet)
	{
		var urlProtocol = await SecureStorage.GetAsync("urlProtocol");
		var url = await SecureStorage.GetAsync("url");
		var portNumber = await SecureStorage.GetAsync("portNumber");
		if (string.IsNullOrEmpty(urlProtocol))
		{
			urlProtocol = "";
		}
		if (string.IsNullOrEmpty(url))
		{
			url = "";
		}
		if (string.IsNullOrEmpty(portNumber))
		{
			portNumber = "";
		}


		UrlProtocol = urlProtocol;
		Url = url;
		PortNumber = portNumber;

		bottomSheet.State = BottomSheetState.HalfExpanded;
	}

	private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
			BaseUri = $@"{UrlProtocol}://{Url}:{PortNumber}";
			if (string.IsNullOrEmpty(BaseUri))
            {
                _userDialogs.Alert("Please enter base uri", "Error", "OK");
                return;
            }
            else
            {
				await SecureStorage.SetAsync("urlProtocol", UrlProtocol);
				await SecureStorage.SetAsync("portNumber", PortNumber);
				await SecureStorage.SetAsync("url", Url);
				await SecureStorage.SetAsync("baseUri", BaseUri);

                CurrentPage.FindByName<BottomSheet>("bottomSheet").State = BottomSheetState.Hidden;
				_userDialogs.ShowToast($"Başarıyla kaydedildi: {BaseUri}");

            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Error", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LogoutAsync()
    {
        if (IsBusy)
            return;
        try
        {
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
            string baseUri = await SecureStorage.GetAsync("baseUri");

             _httpClientSysService.BaseUri = baseUri;
            var httpClient = _httpClientSysService.GetOrCreateHttpClient();
            var token = await _authenticateSysService.AuthenticateAsync(httpClient, UserName, Password);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClientSysService.Token = token;
                ApplicationUser = await GetCurrentUserAsync(UserName);
                if (ApplicationUser is not null)
                {
                    _httpClientSysService.UserOid = ApplicationUser.Oid;
                    _httpClientSysService.UserName = UserName;
                    _httpClientSysService.Password = Password;
					isLoggedIn = true;
                }
                else
                {
                    isLoggedIn = false;
                }
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
            string filter = $"$filter=UserName eq '{userName}'&$expand=Image,Position";
            var result = await _applicationUserService.GetAllAsync(httpClient, filter);

            if (result.Any())
            {
                return result.FirstOrDefault();
            }
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
            var gatewayUri = "";
            var gatewayPort = "";

            var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();
            var result = await _connectionParameterService.GetAllAsync(httpSysClient);
            if (result.Any())
            {
                foreach (var item in result)
                {
                    gatewayUri = item.GatewayUri;
                    gatewayPort = item.GatewayPort;
                }
            }

            string baseUri = $"{gatewayUri}:{gatewayPort}";          
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

    private async Task ProfileTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ProfileView)}");
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
}