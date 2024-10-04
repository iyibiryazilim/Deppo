using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;

public partial class SupplierDetailAllFicheListView : ContentPage
{
	private readonly SupplierDetailAllFicheListViewModel _viewModel;
    public SupplierDetailAllFicheListView(SupplierDetailAllFicheListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.CurrentPage = this;
    }
}