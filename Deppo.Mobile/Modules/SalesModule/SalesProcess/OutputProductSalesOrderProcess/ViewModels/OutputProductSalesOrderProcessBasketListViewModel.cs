using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

public partial class OutputProductSalesOrderProcessBasketListViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	public OutputProductSalesOrderProcessBasketListViewModel(IUserDialogs userDialogs)
	{
		_userDialogs = userDialogs;
	}

	#region Commands

	#endregion

	#region Properties
	public ContentPage CurrentPage { get; set; } = null!;
	#endregion
}
