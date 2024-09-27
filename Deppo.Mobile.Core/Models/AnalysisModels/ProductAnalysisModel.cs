using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.AnalysisModels
{
    public partial class ProductAnalysisModel : ObservableObject
    {
        public ProductAnalysisModel()
        {
            
        }

        [ObservableProperty]
        int totalProductCount;

        [ObservableProperty]
        double inStockProductCount;

        [ObservableProperty]
        double outStockProductCount;

        [ObservableProperty]
        private int inputTransactionCount;

        [ObservableProperty]
        private int outputTransactionCount;

        [ObservableProperty]
        private int negativeStockProductQuantity;

        public ObservableCollection<WarehouseModel> LastWarehouses { get; } = new();
        public ObservableCollection<IORateModel> IORates { get; } = new();
    }
}
