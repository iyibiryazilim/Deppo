using Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;

namespace Deppo.Mobile.Modules.CountingModule.CountingPanel.Views;

public partial class CountingTransactionsListView : ContentPage
{
	private readonly CountingTransactionsListViewModel _viewModel;
	public CountingTransactionsListView(CountingTransactionsListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}