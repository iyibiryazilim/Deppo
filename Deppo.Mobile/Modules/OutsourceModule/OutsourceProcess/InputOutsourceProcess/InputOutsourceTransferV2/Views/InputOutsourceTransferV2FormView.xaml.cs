using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

public partial class InputOutsourceTransferV2FormView : ContentPage
{
	private readonly InputOutsourceTransferV2FormViewModel _viewModel;
	public InputOutsourceTransferV2FormView(InputOutsourceTransferV2FormViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}
}