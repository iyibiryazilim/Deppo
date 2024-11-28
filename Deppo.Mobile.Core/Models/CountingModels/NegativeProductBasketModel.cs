using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.CountingModels
{
    public partial class NegativeProductBasketModel : ObservableObject
    {
        [ObservableProperty]
        NegativeWarehouseModel negativeWarehouseModel = null!;

        [ObservableProperty]
        ObservableCollection<NegativeProductModel> negativeProducts = new();


    }
}
