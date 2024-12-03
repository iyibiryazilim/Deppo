using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

public partial class InputOutsourceTransferV2BasketView : ContentPage
{
	private readonly InputOutsourceTransferV2BasketViewModel _viewModel;
	public InputOutsourceTransferV2BasketView(InputOutsourceTransferV2BasketViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
		_viewModel.CurrentPage = this;
	}
}