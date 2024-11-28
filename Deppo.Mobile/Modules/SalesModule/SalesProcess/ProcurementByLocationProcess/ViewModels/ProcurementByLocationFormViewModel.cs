using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;


[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
[QueryProperty(nameof(SelectedOrderWarehouseModel), nameof(SelectedOrderWarehouseModel))]
[QueryProperty(nameof(ProcurementLocationFormModel), nameof(ProcurementLocationFormModel))]
public partial class ProcurementByLocationFormViewModel : BaseViewModel
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;
	public ProcurementByLocationFormViewModel(IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		Title = "Form";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private WarehouseModel selectedOrderWarehouseModel = null!;

	[ObservableProperty]
	private ProcurementLocationFormModel procurementLocationFormModel = null!;


	[ObservableProperty]
	private DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	private string documentNumber = string.Empty;


	[ObservableProperty]
	private string specialCode = string.Empty;

	[ObservableProperty]
	private string description = string.Empty;

	public Command LoadItemsCommand { get; }
	public Command ConfirmCommand { get; }
	public Command BackCommand { get; }


	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			var viewModel = _serviceProvider.GetRequiredService<ProcurementByLocationCustomerFormViewModel>();

			var basket = viewModel.Items.FirstOrDefault(x => x.ProcurementCustomerModel?.CustomerReferenceId == ProcurementLocationFormModel.ProcurementCustomerModel?.CustomerReferenceId);
			if (basket is not null)
			{
				if(!string.IsNullOrEmpty(basket.DocumentNumber))
					DocumentNumber = basket.DocumentNumber;
				else
					DocumentNumber = string.Empty;

				if (basket.TransactionDate != DateTime.Now)
					TransactionDate = basket.TransactionDate;
				else
					TransactionDate = DateTime.Now;

				if (!string.IsNullOrEmpty(basket.Description))
					Description = basket.Description;
				else
					Description = string.Empty;

				if (!string.IsNullOrEmpty(basket.SpecialCode))
					SpecialCode = basket.SpecialCode;
				else
					SpecialCode = string.Empty;

			}

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

			var previousViewModel = _serviceProvider.GetRequiredService<ProcurementByLocationCustomerFormViewModel>();

			if (previousViewModel is not null)
			{
				var basket = previousViewModel.Items.FirstOrDefault(x => x.ProcurementCustomerModel?.CustomerReferenceId == ProcurementLocationFormModel.ProcurementCustomerModel?.CustomerReferenceId);
				if (basket is not null)
				{
					basket.DocumentNumber = DocumentNumber;
					basket.TransactionDate = TransactionDate;
					basket.Description = Description;
					basket.SpecialCode = SpecialCode;

					basket.IsFormCompleted = true;
				}



				await Shell.Current.GoToAsync("..");
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task BackAsync()
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
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}


