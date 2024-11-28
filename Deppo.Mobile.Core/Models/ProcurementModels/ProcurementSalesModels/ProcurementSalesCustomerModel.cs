using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels
{
    public partial class ProcurementSalesCustomerModel : ObservableObject
    {
        [ObservableProperty]
        private int referenceId;

        [ObservableProperty]
        private int customerReferenceId;

        [ObservableProperty]
        private string customerCode = string.Empty;

        [ObservableProperty]
        private string customerName = string.Empty;

        [ObservableProperty]
        private int productReferenceCount;

        [ObservableProperty]
        private string country = string.Empty;

        [ObservableProperty]
        private string city = string.Empty;

        [ObservableProperty]
        private int shipAddressReferenceId = 0;

        [ObservableProperty]
        private string shipAddressCode = string.Empty;
        [ObservableProperty]
        private string shipAddressName = string.Empty;

        public string TitleName => CustomerName?.Length > 2 ? CustomerName.Substring(0, 2) : CustomerName;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private bool isEDispatch;

        [ObservableProperty]
        private string documentNumber = string.Empty;

        [ObservableProperty]
        private string documentTrackingNumber = string.Empty;

		[ObservableProperty]
        private string speCode = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime ficheDate;

		[ObservableProperty]
        private TimeSpan ficheTime;

        [ObservableProperty]
        private string ficheNumber = string.Empty;


	}
}
