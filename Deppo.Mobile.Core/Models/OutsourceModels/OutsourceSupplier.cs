using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels
{
    public partial class OutsourceSupplier : ObservableObject
    {
        [ObservableProperty]
        private int _referenceId;

        [ObservableProperty]
        private string _code = string.Empty;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private int _productReferenceCount;

        [ObservableProperty]
        private string country = string.Empty;

        [ObservableProperty]
        private string city = string.Empty;

        [ObservableProperty]
        private int shipAddressReferenceId;

        [ObservableProperty]
        private string shipAddressCode = string.Empty;

        [ObservableProperty]
        private string shipAddressName = string.Empty;

        [ObservableProperty]
        private int shipAddressCount;

        [ObservableProperty]
        private bool isEDispatch;

        public string TitleName => Name?.Length > 2 ? Name.Substring(0, 2) : Name;

        [ObservableProperty]
        private bool isSelected;

        public string ShipAddressIcon => ShipAddressCount > 0 ? "truck" : "";
    }
}