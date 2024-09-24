using Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.Views;

public partial class NegativeProductFormView : ContentPage
{
	private readonly NegativeProductFormViewModel _viewModel;
    public NegativeProductFormView(NegativeProductFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        
    }
}