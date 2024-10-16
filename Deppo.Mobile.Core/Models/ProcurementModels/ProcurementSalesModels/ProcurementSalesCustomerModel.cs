using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels
{
    public partial class ProcurementSalesCustomerModel : ObservableObject
    {

        [ObservableProperty]
        private int referenceId;

        [ObservableProperty]
        private string code = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

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

        public string TitleName => Name?.Length > 2 ? Name.Substring(0, 2) : Name;

        [ObservableProperty]
        private bool isSelected;


    }
}
