using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;

public partial class OutputOutsourceTransferV2MainProductLocationListView : ContentPage
{
	private readonly OutputOutsourceTransferV2MainProductLocationListViewModel _viewModel;
    public OutputOutsourceTransferV2MainProductLocationListView(OutputOutsourceTransferV2MainProductLocationListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = locationSearchBar;

    }
}