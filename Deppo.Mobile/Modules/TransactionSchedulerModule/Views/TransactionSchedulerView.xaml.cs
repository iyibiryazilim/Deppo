using Deppo.Mobile.Modules.TransactionSchedulerModule.ViewModels;

namespace Deppo.Mobile.Modules.TransactionSchedulerModule.Views;

public partial class TransactionSchedulerView : ContentPage
{
    private readonly TransactionSchedulerViewModel _viewModel;

    public TransactionSchedulerView(TransactionSchedulerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}