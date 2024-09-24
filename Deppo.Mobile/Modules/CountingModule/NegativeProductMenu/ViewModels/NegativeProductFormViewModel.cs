using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.ViewModels;


[QueryProperty(name: nameof(NegativeProductBasketModel), queryId: nameof(NegativeProductBasketModel))]
public partial class NegativeProductFormViewModel : BaseViewModel
{
    [ObservableProperty]
    NegativeProductBasketModel negativeProductBasketModel = null!;

    [ObservableProperty]
    DateTime transactionDate = DateTime.Now;
    [ObservableProperty]
    string description = string.Empty;
    [ObservableProperty]
    string documentNumber = string.Empty;
    [ObservableProperty]
    string documentTrackingNumber = string.Empty;
    [ObservableProperty]
    string specialCode = string.Empty;

    public NegativeProductFormViewModel()
    {
        
    }
}
