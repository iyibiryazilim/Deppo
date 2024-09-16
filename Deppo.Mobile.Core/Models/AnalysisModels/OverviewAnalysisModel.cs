using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.ProductModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.AnalysisModels
{
    public partial class OverviewAnalysisModel : ObservableObject
    {
        public OverviewAnalysisModel()
        {
            
        }
        [ObservableProperty]
        private int totalProductCount;

        [ObservableProperty]
        private int totalInputProductCount;

        [ObservableProperty]
        private int totalOutputProductCount;

        [ObservableProperty]
        private int outputTransactionCount;

        [ObservableProperty]
        private int inputTransactionCount;

        [ObservableProperty]
        private int productsWithNoTransactionsCount;

        public ObservableCollection<ProductModel> ProductsWithNoTransactions { get; } = new();
    }
}
