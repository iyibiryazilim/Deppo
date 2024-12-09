using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels
{
    public partial class OutputOutsourceTransferV2SubProductModel : ObservableObject
    {
        //[ObservableProperty]
        //private int referenceId;

        [ObservableProperty]
        private int productReferenceId;

        [ObservableProperty]
        private string productCode = string.Empty;

        [ObservableProperty]
        private string productName = string.Empty;

        [ObservableProperty]
        private int inWarehouseNumber;

        [ObservableProperty]
        private string inWarehouseName = string.Empty;

        [ObservableProperty]
        private int outWarehouseNumber;

        [ObservableProperty]
        private string outWarehouseName = string.Empty;

        [ObservableProperty]
        private double bOMQuantity;

        [ObservableProperty]
        private double quantity;


        [ObservableProperty]
        private int unitsetReferenceId;

        [ObservableProperty]
        private string unitsetName = string.Empty;

        [ObservableProperty]
        private string unitsetCode = string.Empty;

        [ObservableProperty]
        private int subUnitsetReferenceId;

        [ObservableProperty]
        private string subUnitsetCode = string.Empty;

        [ObservableProperty]
        private string subUnitsetName = string.Empty;

        [ObservableProperty]
        private int outsourceReferenceId;

        [ObservableProperty]
        private string outsourceCode = string.Empty;

        [ObservableProperty]
        private string outsourceName = string.Empty;

        [ObservableProperty]
        private int operationReferenceId;

        private string operationName = string.Empty;

        [ObservableProperty]
        private double planningQuantity = default;

        [ObservableProperty]
        private double actualQuantity = default;

        [ObservableProperty]
        private double stockQuantity = default;

        [ObservableProperty]
        bool isVariant;

        [ObservableProperty]
        int trackingType;

        [ObservableProperty]
        int locTracking;

        [ObservableProperty]
        public bool _isSelected;

        [ObservableProperty]
        string image = string.Empty;

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
        private ObservableCollection<GroupLocationTransactionModel> locationTransactions = new();

        [ObservableProperty]
        private ObservableCollection<LocationModel> locations = new();

        public string LocTrackingIcon => "location-dot";
        public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

        public string VariantIcon => "bookmark";
        public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

        public string TrackingTypeIcon => "box-archive";
        public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

        public List<OutputOutsourceTransferSubProductDetailModel> Details { get; set; } = new();

    }
}
