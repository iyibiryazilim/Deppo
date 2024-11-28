using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;
using Deppo.Mobile.Modules.TaskModule.Views;
using Deppo.Mobile.Modules.TransactionSchedulerModule.ViewModels;
using Deppo.Mobile.Modules.TransactionSchedulerModule.Views;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;

public partial class OverviewAnalysisViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly ITransactionAuditService _transactionAuditService;
	private readonly IUserDialogs _userDialogs;
	private readonly IOverviewAnalysisService _overviewAnalysisService;
	private readonly IApplicationUserService _applicationUserService;
	private readonly IProcessRateService _processRateService;
	private readonly ITaskListService _taskListService;

	[ObservableProperty]
	private ApplicationUser currentUser;

	[ObservableProperty]
	public DateTime selectedDate;

	[ObservableProperty]
	public OverviewAnalysisModel overviewAnalysisModel = new();

	[ObservableProperty]
	int taskCount = 0;
	public ObservableCollection<PerformanceAnalysisModel> Items { get; } = new();
	public ObservableCollection<TransactionAudit> TransactionAudits { get; } = new();
	public ObservableCollection<TransactionAudit> SelectedTransactionAudits { get; } = new();

	public OverviewAnalysisViewModel(
		IUserDialogs userDialogs,
	IHttpClientService httpClientService,
	IOverviewAnalysisService overviewAnalysisService,
	ITransactionAuditService transactionAuditService,
	IHttpClientSysService httpClientSysService,
	IApplicationUserService applicationUserService,
	IProcessRateService processRateService,
	ITaskListService taskListService)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_transactionAuditService = transactionAuditService;
		_httpClientSysService = httpClientSysService;
		_overviewAnalysisService = overviewAnalysisService;
		_applicationUserService = applicationUserService;
		_processRateService = processRateService;
		_taskListService = taskListService;

		Title = "Genel Analiz";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		ProfileTappedCommand = new Command(async () => await ProfileTappedAsync());
		NotificationTappedCommand = new Command(async () => await NotificationTappedAsync());
		InputTransactionCountTappedCommand = new Command(async () => await InputTransactionCountTappedAsync());
		OutputTransactionCountTappedCommand = new Command(async () => await OutputTransactionCountTappedAsync());
		PerformanceAnalysisTappedCommand = new Command<PerformanceAnalysisModel>(async (performanceAnalysisModel) => await PerformanceAnalysisTappedAsync(performanceAnalysisModel));
		CalendarTappedCommand = new Command(async () => await CalendarTappedAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command ProfileTappedCommand { get; }
	public Command NotificationTappedCommand { get; }
	public Command InputTransactionCountTappedCommand { get; }
	public Command OutputTransactionCountTappedCommand { get; }
	public Command PerformanceAnalysisTappedCommand { get; }

	public Command CalendarTappedCommand { get; }

	public Page CurrentPage { get; set; }

	public async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Yükleniyor...");
			await GetCurrentUserAsync();
			await GetCurrentUserTaskCountAsync();
			await Task.Delay(1000);
			TransactionAudits.Clear();
			var httpClient = _httpClientSysService.GetOrCreateHttpClient();

			string filter = $"expand=ApplicationUser($expand=Image,Position)&$filter=FirmNumber eq {_httpClientService.FirmNumber} and PeriodNumber eq {_httpClientService.PeriodNumber} &$orderby=TransactionDate desc";
			var result = await _transactionAuditService.GetAllAsync(httpClient, filter);
			var processRates = await _processRateService.GetAllAsync(httpClient); // Oran verilerini getiriyoruz

			Items.Clear();
			List<PerformanceAnalysisModel> analysis = new List<PerformanceAnalysisModel>();

			var groupedUser = result.GroupBy(x => x.ApplicationUser?.Oid);
			foreach (var group in groupedUser)
			{
				var applicationUser = group.FirstOrDefault()?.ApplicationUser;
				if (applicationUser != null)
				{
					var count = 0;
					List<double> rate = new List<double>();
					foreach (var item in group)
					{
						TransactionAudits.Add(item);
					}
					var analysisModel = new PerformanceAnalysisModel
					{
						ApplicationUser = applicationUser,
						TransactionCount = group.ToList().Count,


					};
					analysis.Add(analysisModel);


				}
			}

			int order = 1;
			foreach (var item in analysis.OrderByDescending(x => x.TransactionCount))
			{
				if (order == 1)
				{
					item.IconColor = "#FFD700";
					item.IconVisibility = true;
				}
				else if (order == 2)
				{
					item.IconColor = "#C0C0C0";
					item.IconVisibility = true;

				}
				else if (order == 3)
				{
					item.IconColor = "#CD7F32";
					item.IconVisibility = true;
				}
				else
				{
					item.IconVisibility = false;
				}
				order++;
				Items.Add(item);

			}



			await Task.WhenAll(GetInputTransactionCountAsync(), GetOutputTransactionCountAsync());

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}
	private async Task GetCurrentUserTaskCountAsync()
	{
		try
		{
			var httpClient = _httpClientSysService.GetOrCreateHttpClient();
			string filter = $"$expand=User&$filter=User/Oid eq {_httpClientSysService.UserOid} and Status in ('NotStarted')";
			var result = await _taskListService.GetAllAsync(httpClient, filter);
			if (result.Any())
				TaskCount = result.Count();

		}
		catch (Exception ex) 
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task GetTotalProductCountAsync()
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _overviewAnalysisService.GetTotalProductCountAsync(httpClient, _httpClientService.FirmNumber);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
					OverviewAnalysisModel.TotalProductCount = obj.TotalProductCount;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task GetTotalInputProductCountAsync()
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _overviewAnalysisService.GetTotalInputProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
					OverviewAnalysisModel.TotalInputProductCount = obj.TotalInputProductCount;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task GetTotalOutputProductCountAsync()
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _overviewAnalysisService.GetTotalOutputProductCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
					OverviewAnalysisModel.TotalOutputProductCount = obj.TotalOutputProductCount;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task GetInputTransactionCountAsync()
	{
		try
		{
			var httpClient = _httpClientSysService.GetOrCreateHttpClient();
			string filter = $"$filter=IOType in (1,2) and FirmNumber eq {_httpClientService.FirmNumber} and PeriodNumber eq {_httpClientService.PeriodNumber}&$count=true";
			var result = await _transactionAuditService.GetAllAsync(httpClient, filter);

			OverviewAnalysisModel.InputTransactionCount = result.Count();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task GetOutputTransactionCountAsync()
	{
		try
		{
			var httpClient = _httpClientSysService.GetOrCreateHttpClient();
			string filter = $"$filter=IOType in (3,4) and FirmNumber eq {_httpClientService.FirmNumber} and PeriodNumber eq {_httpClientService.PeriodNumber}&$count=true";
			var result = await _transactionAuditService.GetAllAsync(httpClient, filter);

			OverviewAnalysisModel.OutputTransactionCount = result.Count();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task GetProductsWithNoTransactionsCountAsync()
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _overviewAnalysisService.GetProductsWithNoTransactionsCountAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedDate);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<OverviewAnalysisModel>(item);
					OverviewAnalysisModel.ProductsWithNoTransactionsCount = obj.ProductsWithNoTransactionsCount;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task GetProductsWithNoTransactionsAsync()
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _overviewAnalysisService.GetProductsWithNoTransactionsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedDate, string.Empty, 0, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				OverviewAnalysisModel.ProductsWithNoTransactions.Clear();
				foreach (var item in result.Data)
					OverviewAnalysisModel.ProductsWithNoTransactions.Add(Mapping.Mapper.Map<ProductModel>(item));
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task ProfileTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(TaskListView)}");
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

	private async Task NotificationTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(NotificationListView)}");
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

	private async Task InputTransactionCountTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OverviewInputTransactionListView)}");
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

	private async Task OutputTransactionCountTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OverviewOutputTransactionListView)}");
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

	private async Task PerformanceAnalysisTappedAsync(PerformanceAnalysisModel performanceAnalysisModel)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Yükleniyor...");

			SelectedTransactionAudits.Clear();
			foreach (var item in TransactionAudits.Where(x => x.ApplicationUserOid == performanceAnalysisModel.ApplicationUser.Oid).ToList())
			{
				SelectedTransactionAudits.Add(item);
			}

			CurrentPage.FindByName<BottomSheet>("performanceAnalysisTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task CalendarTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(TransactionSchedulerView)}");
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