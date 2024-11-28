using CommunityToolkit.Mvvm.ComponentModel;
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
        private int negativeStockProductQuantity;

        [ObservableProperty]
        ObservableCollection<IORateModel> iORates = new();

        [ObservableProperty]
		ObservableCollection<InputOutputProductReferenceAnalysis> inputOutputProductReferenceAnalysis  = new();
	}
}
