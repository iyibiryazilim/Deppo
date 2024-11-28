using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly IApplicationUserService _applicationUserService;

	[ObservableProperty]
	ApplicationUser currentUser;

	public ProfileViewModel(IUserDialogs userDialogs, IHttpClientSysService httpClientSysService, IApplicationUserService applicationUserService)
	{
		_userDialogs = userDialogs;
		_httpClientSysService = httpClientSysService;
		_applicationUserService = applicationUserService;

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		CloseCommand = new Command(async () => await CloseAsync());
	}
	public Command LoadPageCommand { get; }
	public Command CloseCommand { get; }

	private async Task LoadPageAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			await GetCurrentUserAsync();

			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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


	private async Task GetCurrentUserAsync()
	{
		try
		{
			var httpClient = _httpClientSysService.GetOrCreateHttpClient();
			string filter = $"filter=UserName eq '{_httpClientSysService.UserName}'&$expand=Image";
			var result = await _applicationUserService.GetAllAsync(httpClient, filter);

			if (result.Any())
				CurrentUser = result.FirstOrDefault();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task CloseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync("..");
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
