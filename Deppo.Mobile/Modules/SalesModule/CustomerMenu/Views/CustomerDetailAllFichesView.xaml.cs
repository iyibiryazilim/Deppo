using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;

public partial class CustomerDetailAllFichesView : ContentPage
{
	private readonly CustomerDetailAllFichesViewModel _viewModel;
    public CustomerDetailAllFichesView(CustomerDetailAllFichesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel= viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage= this;
	}
}