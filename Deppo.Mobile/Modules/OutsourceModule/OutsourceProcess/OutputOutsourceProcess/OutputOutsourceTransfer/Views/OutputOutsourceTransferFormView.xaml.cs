using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;

public partial class OutputOutsourceTransferFormView : ContentPage
{
	private readonly OutputOutsourceTransferFormViewModel _viewModel;
	public OutputOutsourceTransferFormView(OutputOutsourceTransferFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}