using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.CountingModels
{
    public partial class NegativeWarehouseModel : ObservableObject
    {
        [ObservableProperty]
        public int warehouseReferenceId;

        [ObservableProperty]
        public short warehouseNumber;

        [ObservableProperty]
        public string warehouseName = string.Empty;

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
