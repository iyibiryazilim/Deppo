using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.AnalysisModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels
{
	public partial class OverviewInputTransactionListViewModel : BaseViewModel
	{
		private readonly IHttpClientSysService _httpClientSysService;
		private readonly IHttpClientService _httpClientService;
		private readonly ITransactionAuditService _transactionAuditService;
		private readonly IUserDialogs _userDialogs;
		private readonly IOverviewAnalysisTransactionService _overviewAnalysisTransactionService;
		public OverviewInputTransactionListViewModel(IHttpClientSysService httpClientSysService, ITransactionAuditService transactionAuditService, IUserDialogs userDialogs, IOverviewAnalysisTransactionService overviewAnalysisTransactionService, IHttpClientService httpClientService)
		{
			_httpClientSysService = httpClientSysService;
			_httpClientService = httpClientService;
			_transactionAuditService = transactionAuditService;
			_userDialogs = userDialogs;
			_overviewAnalysisTransactionService = overviewAnalysisTransactionService;

			Title = "Giriş Hareketleri";

			LoadItemsCommand = new Command(async () => await LoadItemsAsync());
			CloseCommand = new Command(async () => await CloseAsync());
			ItemTappedCommand = new Command<TransactionAudit>(async (transactionAudit) => await ItemTappedAsync(transactionAudit));
			LoadMoreTransactionCommand = new Command(async () => await LoadMoreTransactionsByFiche());

		}
		[ObservableProperty]
		TransactionAudit selectedTransactionAudit;
		public ObservableCollection<TransactionAudit> Items { get; } = new();

		public ObservableCollection<OverviewFicheTransactionModel> OverviewFicheTransactions { get; } = new();

		public Command LoadItemsCommand { get; }
		public Command CloseCommand { get; }
		public Command ItemTappedCommand { get; }
		public Command LoadMoreTransactionCommand { get; }
		public Page CurrentPage { get; set; }
		public async Task LoadItemsAsync()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				_userDialogs.ShowLoading("Yükleniyor...");
				await Task.Delay(500);
				var httpClient = _httpClientSysService.GetOrCreateHttpClient();

				string filter = $"expand=ApplicationUser($expand=Image)&$filter=IOType in (1,2)&$orderby=TransactionDate desc";
				var result = await _transactionAuditService.GetAllAsync(httpClient, filter);

				if (result.Any())
				{
					Items.Clear();
					foreach (var item in result)
					{
						Items.Add(item);
					}

				}


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

		private async Task ItemTappedAsync(TransactionAudit transactionAudit)
		{
			if (IsBusy)
				return;
			try
			{
				IsBusy = true;
				_userDialogs.ShowLoading("Yükleniyor...");
				await Task.Delay(500);
				await LoadTransactionsByFiche(transactionAudit);
				CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

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

		private async Task LoadTransactionsByFiche(TransactionAudit transactionAudit)
		{
			try
			{
				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var result = await _overviewAnalysisTransactionService.GetTransactionByFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, transactionAudit.TransactionReferenceId, string.Empty, 0, 20);
				SelectedTransactionAudit = transactionAudit;

				if (result.IsSuccess)
				{
					if (result.Data is null)
						return;
					OverviewFicheTransactions.Clear();

					foreach (var item in result.Data)
						OverviewFicheTransactions.Add(Mapping.Mapper.Map<OverviewFicheTransactionModel>(item));
				}
			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			}
			finally
			{
			}
		}
		private async Task LoadMoreTransactionsByFiche()
		{
			try
			{
				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var result = await _overviewAnalysisTransactionService.GetTransactionByFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedTransactionAudit.TransactionReferenceId, string.Empty, OverviewFicheTransactions.Count, 20);


				if (result.IsSuccess)
				{
					if (result.Data is null)
						return;

					foreach (var item in result.Data)
						OverviewFicheTransactions.Add(Mapping.Mapper.Map<OverviewFicheTransactionModel>(item));
				}
			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			}
			finally
			{
			}
		}
	}
}
