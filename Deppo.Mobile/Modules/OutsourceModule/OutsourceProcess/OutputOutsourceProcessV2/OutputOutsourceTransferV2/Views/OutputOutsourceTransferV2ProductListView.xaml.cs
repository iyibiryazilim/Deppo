using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;

public partial class OutputOutsourceTransferV2ProductListView : ContentPage
{
	private readonly OutputOutsourceTransferV2ProductListViewModel _viewModel;
    public OutputOutsourceTransferV2ProductListView(OutputOutsourceTransferV2ProductListViewModel viewModel )
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		
	}
}