using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

public partial class NotificationListViewModel : BaseViewModel
{
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly INotificationService _notificationService;
	private readonly INotificationStatusService _notificationStatusService;
	private readonly IUserDialogs _userDialogs;

	public ObservableCollection<Notification> Items { get; } = new();

	public NotificationListViewModel(IUserDialogs userDialogs, IHttpClientSysService httpClientSysService, INotificationService notificationService, INotificationStatusService notificationStatusService)
	{
		_userDialogs = userDialogs;
		_httpClientSysService = httpClientSysService;
		_notificationService = notificationService;
		_notificationStatusService = notificationStatusService;

		Title = "Bildirimler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		CloseCommand = new Command(async () => await CloseAsync());
	}

	public Command CloseCommand { get; }
	public Command LoadItemsCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Yükleniyor...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClientSys = _httpClientSysService.GetOrCreateHttpClient();
			string filter = $"$expand=NotificationStatus...&$filter=NotificationStatus../IsRead eq false";
			var result = await _notificationService.GetAllAsync(httpClientSys);
			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = new Notification
					{
						Oid = item.Oid,
						Description = item.Description,
						Title = item.Title
					};

					Items.Add(obj);
				}
			}

			if (_userDialogs.IsHudShowing)
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
			IsBusy = true;
		}
	}
}
