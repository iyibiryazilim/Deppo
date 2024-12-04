using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;

public partial class OutputOutsourceTransferV2OutsourceFormView : ContentPage
{
	private readonly OutputOutsourceTransferV2OutsourceFormViewModel _viewModel;
    public OutputOutsourceTransferV2OutsourceFormView(OutputOutsourceTransferV2OutsourceFormViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}