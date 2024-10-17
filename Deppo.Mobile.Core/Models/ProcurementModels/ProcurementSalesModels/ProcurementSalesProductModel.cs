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
        private double quantity;

        [ObservableProperty]
        private string image = string.Empty;

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
    }
}
