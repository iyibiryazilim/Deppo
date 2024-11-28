using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;

public partial class TransferFormView : ContentPage
{
	private readonly TransferFormViewModel _viewModel;
    public TransferFormView(TransferFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}