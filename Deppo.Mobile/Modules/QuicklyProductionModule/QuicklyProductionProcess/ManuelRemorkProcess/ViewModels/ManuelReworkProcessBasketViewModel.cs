using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;


[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessBasketViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;

	public ManuelReworkProcessBasketViewModel(IUserDialogs userDialogs)
	{

		Title = "Sepet";
		_userDialogs = userDialogs;
	}
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command BackCommand { get; }
	public Command NextViewCommand { get; }
}
