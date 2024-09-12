using CommunityToolkit.Mvvm.ComponentModel;

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
    }
}
