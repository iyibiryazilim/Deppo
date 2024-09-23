using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.CountingModels
{
    public partial class NegativeProductModel : ObservableObject
    {
        [ObservableProperty]
        public int productReferenceId;

        [ObservableProperty]
        public string productCode = string.Empty;

        [ObservableProperty]
        public string productName = string.Empty;

        [ObservableProperty]
        public int unitsetReferenceId;

        [ObservableProperty]
        public string unitsetName = string.Empty;

        [ObservableProperty]
        public string unitsetCode = string.Empty;

        [ObservableProperty]
        public int subUnitsetReferenceId;

        [ObservableProperty]
        public string subUnitsetName = string.Empty;

        [ObservableProperty]
        public string subUnitsetCode = string.Empty;

        [ObservableProperty]
        public bool _isVariant;

        [ObservableProperty]
        public int _trackingType;

        [ObservableProperty]
        public int _locTracking;

        [ObservableProperty]
        public double stockQuantity;

    }
}
