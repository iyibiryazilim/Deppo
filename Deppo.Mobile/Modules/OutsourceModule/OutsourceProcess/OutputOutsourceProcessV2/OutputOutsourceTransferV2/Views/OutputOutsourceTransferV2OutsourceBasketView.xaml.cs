using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;

public partial class OutputOutsourceTransferV2OutsourceBasketView : ContentPage
{
	private readonly OutputOutsourceTransferV2OutsourceBasketViewModel _viewModel;
    public OutputOutsourceTransferV2OutsourceBasketView(OutputOutsourceTransferV2OutsourceBasketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		
		
	}
} 