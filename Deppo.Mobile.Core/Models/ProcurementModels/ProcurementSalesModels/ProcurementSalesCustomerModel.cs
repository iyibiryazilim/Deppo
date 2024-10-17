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


    }
}
