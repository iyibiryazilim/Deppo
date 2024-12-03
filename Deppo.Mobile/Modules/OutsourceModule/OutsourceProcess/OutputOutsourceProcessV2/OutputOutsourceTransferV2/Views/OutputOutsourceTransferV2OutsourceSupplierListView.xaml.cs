using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;

public partial class OutputOutsourceTransferV2OutsourceSupplierListView : ContentPage
{
	private readonly OutputOutsourceTransferV2OutsourceSupplierListViewModel _viewModel;
    public OutputOutsourceTransferV2OutsourceSupplierListView(OutputOutsourceTransferV2OutsourceSupplierListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}