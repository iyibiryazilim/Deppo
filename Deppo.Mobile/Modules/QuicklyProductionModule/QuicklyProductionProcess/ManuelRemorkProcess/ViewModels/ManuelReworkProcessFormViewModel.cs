using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IProductionTransactionService _productionTransactionService;
	private readonly IConsumableTransactionService _consumableTransactionService;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;


	[ObservableProperty]
	string documentNumber = string.Empty;

	[ObservableProperty]
	DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	string description = string.Empty;

	[ObservableProperty]
	string documentTrackingNumber = string.Empty;

	[ObservableProperty]
	string specialCode = string.Empty;

	public ManuelReworkProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProductionTransactionService productionTransactionService, IConsumableTransactionService consumableTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_productionTransactionService = productionTransactionService;
		_consumableTransactionService = consumableTransactionService;

		Title = "Form Sayfası";

		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		LoadPageCommand = new Command(async () => await LoadPageAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Page CurrentPage { get; set; } = null!;
	public Command ShowBasketItemCommand { get; }
	public Command LoadPageCommand { get; }

	public Command SaveCommand { get; }
	public Command BackCommand { get; }

	private async Task ShowBasketItemAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task LoadPageAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading");
			CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var result = await _userDialogs.ConfirmAsync("Form verileri silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!result)
				return;

			await ClearFormAsync();

			await Shell.Current.GoToAsync("..");
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

	private async Task ClearFormAsync()
	{
		try
		{
			DocumentNumber = string.Empty;
			TransactionDate = DateTime.Now;
			Description = string.Empty;
			DocumentTrackingNumber = string.Empty;
			SpecialCode = string.Empty;
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
			
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
