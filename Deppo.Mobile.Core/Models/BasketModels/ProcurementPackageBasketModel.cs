using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.PackageProductModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.BasketModels
{
    public partial class ProcurementPackageBasketModel : ObservableObject
    {
        [ObservableProperty]
        private PackageProductModel packageProductModel;

        public ObservableCollection<ProcurementSalesProductModel> PackageProducts { get; } = new();
    }
}
