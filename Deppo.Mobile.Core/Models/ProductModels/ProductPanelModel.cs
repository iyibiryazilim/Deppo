using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.ProductModels
{
    public partial class ProductPanelModel : ObservableObject
    {
        [ObservableProperty]
        private int? inputProductQuantity;

        [ObservableProperty]
        private int? outputProductQuantity;
        public ObservableCollection<ProductModel> LastProducts { get; } = new();
        public ObservableCollection<WarehouseModel> LastWarehouses { get; } = new();
        public ObservableCollection<ProductTransaction> LastTransactions { get; } = new();
        public ObservableCollection<ProductFiche> LastProductFiche { get; } = new();
    }
}
