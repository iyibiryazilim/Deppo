using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Sys.Service.Services;
using DevExpress.Data.Async.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;

[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
[QueryProperty(name: nameof(ProcurementItem), queryId: nameof(ProcurementItem))]
[QueryProperty(name: nameof(ProcurementProductBasketModel), queryId: nameof(ProcurementProductBasketModel))]
[QueryProperty(name: nameof(SelectedCustomer), queryId: nameof(SelectedCustomer))]
public partial class ProcurementByProductFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ITransferTransactionService _transferTransactionService;
	private readonly IProcurementAuditCustomerService _procurementAuditCustomerService;

	[ObservableProperty]
	ProcurementProductBasketModel procurementProductBasketModel;

	[ObservableProperty]
	ObservableCollection<ProcurementProductBasketModel> items;

	[ObservableProperty]
	ProcurementProductBasketProductModel procurementItem;


	[ObservableProperty]
	CustomerOrderModel selectedCustomer;


	[ObservableProperty]
	private DateTime transactionDate = DateTime.Now;
	[ObservableProperty]
	private string documentNumber = string.Empty;
	[ObservableProperty]
	private string specialCode = string.Empty;
	[ObservableProperty]
	private string description = string.Empty;

	public ProcurementByProductFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider, IHttpClientSysService httpClientSysService, ILocationTransactionService locationTransactionService, ITransferTransactionService transferTransactionService, IProcurementAuditCustomerService procurementAuditCustomerService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_httpClientSysService = httpClientSysService;
		_locationTransactionService = locationTransactionService;
		_transferTransactionService = transferTransactionService;
		_procurementAuditCustomerService = procurementAuditCustomerService;

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		BackCommand = new Command(async () => await BackAsync());
	}
	public Page CurrentPage { get; set; } = null!;
	public Command LoadPageCommand { get; }
	public Command BackCommand { get; }
	public Command ConfirmCommand { get; }

	private async Task LoadPageAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var previousViewModel = _serviceProvider.GetRequiredService<ProcurementByProductQuantityDistributionListViewModel>();

			if (previousViewModel.ProcurementProductFormModels.Where(x => x.SelectedCustomer.Code == SelectedCustomer.Code).Any())
			{
				TransactionDate = previousViewModel.ProcurementProductFormModels.Where(x => x.SelectedCustomer.Code == SelectedCustomer.Code).FirstOrDefault().TransactionDate;
				DocumentNumber = previousViewModel.ProcurementProductFormModels.Where(x => x.SelectedCustomer.Code == SelectedCustomer.Code).FirstOrDefault().DocumentNumber;
				SpecialCode = previousViewModel.ProcurementProductFormModels.Where(x => x.SelectedCustomer.Code == SelectedCustomer.Code).FirstOrDefault().SpecialCode;
				Description = previousViewModel.ProcurementProductFormModels.Where(x => x.SelectedCustomer.Code == SelectedCustomer.Code).FirstOrDefault().Description;
			}
			else
			{
				TransactionDate = DateTime.Now;
				DocumentNumber = string.Empty;
				SpecialCode = string.Empty;
				Description = string.Empty;
			}
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


	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;


			var procurementProductFormModel = new ProcurementProductFormModel
			{
				Description = Description,
				DocumentNumber = DocumentNumber,
				SpecialCode = SpecialCode,
				TransactionDate = TransactionDate,
				SelectedCustomer = SelectedCustomer,
				ProcurementItem = ProcurementItem,
			};

			var previousViewModel = _serviceProvider.GetRequiredService<ProcurementByProductQuantityDistributionListViewModel>();

			if (previousViewModel.ProcurementProductFormModels.Where(x => x.SelectedCustomer.Code == SelectedCustomer.Code).Any())
			{
				var existingItem = previousViewModel.ProcurementProductFormModels.Where(x => x.SelectedCustomer.Code == SelectedCustomer.Code).FirstOrDefault();
				if(existingItem is not null)
				{
					existingItem.DocumentNumber = DocumentNumber;
					existingItem.SpecialCode = SpecialCode;
					existingItem.Description = Description;
					existingItem.TransactionDate = TransactionDate;
				}
			}
			else
			{
				previousViewModel.ProcurementProductFormModels.Add(procurementProductFormModel);
			}

			await Shell.Current.GoToAsync("..");

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
	private async Task BackAsync()
	{
		if(IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync("..");
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
}
