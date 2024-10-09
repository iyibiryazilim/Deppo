using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;


[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessBasketViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;

	public ManuelReworkProcessBasketViewModel(IUserDialogs userDialogs)
	{
		_userDialogs = userDialogs;

		Title = "Sepet";

		AddProductTappedCommand = new Command(async () => await AddProductTappedAsync());
	}
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command AddProductTappedCommand { get; }
	public Command BackCommand { get; }
	public Command NextViewCommand { get; }

	private async Task AddProductTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessInWarehouseListView)}");
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
