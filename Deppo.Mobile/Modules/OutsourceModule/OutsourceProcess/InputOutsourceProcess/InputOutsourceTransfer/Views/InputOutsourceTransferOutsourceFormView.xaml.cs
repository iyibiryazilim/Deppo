using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;

public partial class InputOutsourceTransferOutsourceFormView : ContentPage
{
    private readonly InputOutsourceTransferOutsourceFormViewModel _viewModel;

    public InputOutsourceTransferOutsourceFormView(InputOutsourceTransferOutsourceFormViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}