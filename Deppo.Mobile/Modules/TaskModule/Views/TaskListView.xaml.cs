using Deppo.Mobile.Modules.TaskModule.ViewModels;

namespace Deppo.Mobile.Modules.TaskModule.Views;

public partial class TaskListView : ContentPage
{
	private readonly TaskListViewModel _viewModel;
	public TaskListView(TaskListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
	}
}