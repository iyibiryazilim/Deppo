using Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingPanel.Views;

public partial class CountingInputReferenceProductListView : ContentPage
{
	private readonly CountingInputReferenceProductListViewModel _viewModel;
	public CountingInputReferenceProductListView(CountingInputReferenceProductListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}