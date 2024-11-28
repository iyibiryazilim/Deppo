using AndroidX.AppCompat.View.Menu;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.TaskModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.TaskModule.ViewModels;

public partial class TaskListViewModel : BaseViewModel
{
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly ITaskListService _taskListService;
	private readonly IUserDialogs _userDialogs;

	public ObservableCollection<TaskListModel> Items { get; } = new();
	public ObservableCollection<StatusModel> StatusItems { get; } = new();


	[ObservableProperty]
	TaskListModel selectedItem;

	public TaskListViewModel(IHttpClientSysService httpClientSysService, IUserDialogs userDialogs, ITaskListService taskListService)
	{
		_httpClientSysService = httpClientSysService;
		_userDialogs = userDialogs;
		_taskListService = taskListService;

		Title = "Görev Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		FilterIconTappedCommand = new Command(async () => await FilterIconTappedAsync());
		CloseFilterBottomSheetCommand = new Command(async () => await CloseFilterBottomSheetAsync());
		SwipeItemCommand = new Command<TaskListModel>(async (item) => await SwipeItemAsync(item));
		CloseChangeStatusBottomSheetCommand = new Command(async () => await CloseChangeStatusBottomSheetAsync());
		StatusItemTappedCommand = new Command<StatusModel>(async (item) => await StatusItemTappedAsync(item));

		AllItemsFilterTappedCommand = new Command(async () => await AllItemsFilterTappedAsync());
		WaitingFilterTappedCommand = new Command(async () => await WaitingFilterTappedAsync());
		NotStartedFilterTappedCommand = new Command(async () => await NotStartedFilterTappedAsync());
		InProgressFilterTappedCommand = new Command(async () => await InProgressFilterTappedAsync());
		CompletedFilterTappedCommand = new Command(async () => await CompletedFilterTappedAsync());
		CancelledFilterTappedCommand = new Command(async () => await CancelledFilterTappedAsync());
	}
	public Page CurrentPage { get; set; } = null!;
	public Command LoadItemsCommand { get;}
	public Command FilterIconTappedCommand { get; }
	public Command CloseFilterBottomSheetCommand { get; }
	public Command SwipeItemCommand { get; }
	public Command CloseChangeStatusBottomSheetCommand { get; }
	public Command StatusItemTappedCommand { get; }

	public Command AllItemsFilterTappedCommand { get; }
	public Command WaitingFilterTappedCommand { get; }
	public Command NotStartedFilterTappedCommand { get; }
	public Command InProgressFilterTappedCommand { get; }
	public Command CompletedFilterTappedCommand { get; }
	public Command CancelledFilterTappedCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Yükleniyor");
			Items.Clear();
			await Task.Delay(1000);

			var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $@"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid}";
			var result = await _taskListService.GetAllAsync(httpSysClient, filter);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = Mapping.Mapper.Map<TaskListModel>(item);
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

	private async Task FilterIconTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.HalfExpanded;
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task CloseFilterBottomSheetAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task CloseChangeStatusBottomSheetAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("changeStatusBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task LoadStatusItemsAsync(TaskListModel item)
	{
		try
		{
			StatusItems.Clear();

			var statusList = new List<StatusModel>
			{
				new StatusModel { Status = "NotStarted" },
				new StatusModel { Status = "InProgress" },
				new StatusModel { Status = "Waiting" },
				new StatusModel { Status = "Completed" },
				new StatusModel { Status = "Cancelled" }
			};

			foreach (var status in statusList)
			{
				StatusItems.Add(status);
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task SwipeItemAsync(TaskListModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = item;

			await LoadStatusItemsAsync(item);
			CurrentPage.FindByName<BottomSheet>("changeStatusBottomSheet").State = BottomSheetState.HalfExpanded;
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task StatusItemTappedAsync(StatusModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem.Status = item.Status;

			TaskList taskList = Mapping.Mapper.Map<TaskList>(SelectedItem);

			//var httpClientSysService = _httpClientSysService.GetOrCreateHttpClient();
			//var result = await _taskListService.PatchObjectAsync(httpClientSysService, taskList, taskList.Oid);

			CurrentPage.FindByName<BottomSheet>("changeStatusBottomSheet").State = BottomSheetState.Hidden;
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task AllItemsFilterTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			_userDialogs.Loading("Yükleniyor");
			Items.Clear();
			await Task.Delay(1000);

			var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $@"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid}";
			var result = await _taskListService.GetAllAsync(httpSysClient, filter);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = Mapping.Mapper.Map<TaskListModel>(item);
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

	private async Task WaitingFilterTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			_userDialogs.Loading("Yükleniyor");
			Items.Clear();
			await Task.Delay(1000);

			var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $@"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid} and Status eq 'Waiting'";
			var result = await _taskListService.GetAllAsync(httpSysClient, filter);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = Mapping.Mapper.Map<TaskListModel>(item);
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
	private async Task NotStartedFilterTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			_userDialogs.Loading("Yükleniyor");
			Items.Clear();
			await Task.Delay(1000);

			var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $@"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid} and Status eq 'NotStarted'";
			var result = await _taskListService.GetAllAsync(httpSysClient, filter);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = Mapping.Mapper.Map<TaskListModel>(item);
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

	private async Task InProgressFilterTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			_userDialogs.Loading("Yükleniyor");
			Items.Clear();
			await Task.Delay(1000);

			var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $@"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid} and Status eq 'InProgress'";
			var result = await _taskListService.GetAllAsync(httpSysClient, filter);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = Mapping.Mapper.Map<TaskListModel>(item);
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

	private async Task CompletedFilterTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			_userDialogs.Loading("Yükleniyor");
			Items.Clear();
			await Task.Delay(1000);

			var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $@"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid} and Status eq 'Completed'";
			var result = await _taskListService.GetAllAsync(httpSysClient, filter);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = Mapping.Mapper.Map<TaskListModel>(item);
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

	private async Task CancelledFilterTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("filterBottomSheet").State = BottomSheetState.Hidden;

			_userDialogs.Loading("Yükleniyor");
			Items.Clear();
			await Task.Delay(1000);

			var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $@"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid} and Status eq 'Cancelled'";
			var result = await _taskListService.GetAllAsync(httpSysClient, filter);

			if (result.Any())
			{
				foreach (var item in result)
				{
					var obj = Mapping.Mapper.Map<TaskListModel>(item);
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
}


public partial class StatusModel : ObservableObject
{
	[ObservableProperty]
	bool isSelected = false;

	[ObservableProperty]
	string status = string.Empty;

	public string StatusName
	{
		get
		{
			switch (Status)
			{
				case "NotStarted": // NotStarted
					return "Başlamadı";
				case "InProgress": // InProgress
					return "Devam ediyor";
				case "Waiting": // Waiting
					return "Bekliyor";
				case "Completed": // Completed
					return "Tamamlandı";
				case "Cancelled": // Cancelled
					return "İptal";
				default:
					return "-";
			}
		}
	}
}
