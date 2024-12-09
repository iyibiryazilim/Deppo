using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;

public partial class OutputOutsourceTransferV2SubProductLocationListView : ContentPage
{
	private readonly OutputOutsourceTransferV2SubProductLocationListViewModel _viewModel;
    public OutputOutsourceTransferV2SubProductLocationListView(OutputOutsourceTransferV2SubProductLocationListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
        _viewModel.SearchText = locationSearchBar;

    }
}