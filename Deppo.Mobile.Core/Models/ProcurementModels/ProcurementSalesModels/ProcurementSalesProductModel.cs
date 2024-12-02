using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels
{
    public partial class ProcurementSalesProductModel : ObservableObject
    {
        [ObservableProperty]
        private int referenceId;

        [ObservableProperty]
        private DateTime transactionDate;

        [ObservableProperty]
        private TimeSpan transactionTime;

        [ObservableProperty]
        private int orderReferenceId;

        [ObservableProperty]
        private string orderCode = string.Empty;

        [ObservableProperty]
        private int baseTransactionReferenceId;

        [ObservableProperty]
        private string baseTransactionCode = string.Empty;

        [ObservableProperty]
        private int mainItemReferenceId;

        [ObservableProperty]
        private string mainItemCode = string.Empty;

        [ObservableProperty]
        private string mainItemName = string.Empty;

        [ObservableProperty]
        private int itemReferenceId;

        [ObservableProperty]
        private string itemCode = string.Empty;

        [ObservableProperty]
        private string itemName = string.Empty;

        [ObservableProperty]
        private int unitsetReferenceId;

        [ObservableProperty]
        private string unitsetCode = string.Empty;

        [ObservableProperty]
        private string unitsetName = string.Empty;

        [ObservableProperty]
        private int subUnitsetReferenceId;

        [ObservableProperty]
        private string subUnitsetCode = string.Empty;

        [ObservableProperty]
        private string subUnitsetName = string.Empty;

        [ObservableProperty]
        private double weight = default;

        [ObservableProperty]
        private double volume = default;

        [ObservableProperty]
        private double quantity;

        [ObservableProperty]
        private string barcode;

        [ObservableProperty]
        private string image = string.Empty;

        [ObservableProperty]
        private bool _isVariant;

        [ObservableProperty]
        private int _trackingType;

        [ObservableProperty]
        private int _locTracking;

        [ObservableProperty]
        private string _locTrackingIcon = "location-dot";

        [ObservableProperty]
        private string _variantIcon = "bookmark";

        [ObservableProperty]
        private string _trackingTypeIcon = "box-archive";

        public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

        public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

        public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

        public byte[] ImageData
        {
            get
            {
                if (string.IsNullOrEmpty(Image))
                    return Array.Empty<byte>();
                else
                {
                    return Convert.FromBase64String(Image);
                }
            }
        }

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private double outputQuantity = 1;
	}
}
