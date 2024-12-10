using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels
{
    public partial class OutputOutsourceTransferV2ProductModel :ObservableObject
    {
        [ObservableProperty]
        private int referenceId;

        [ObservableProperty]
        private int productionReferenceId;

        [ObservableProperty]
        private int productReferenceId;

        [ObservableProperty]
        private string productCode = string.Empty;

        [ObservableProperty]
        private string productName = string.Empty;

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

        [ObservableProperty]
        private string operationCode = string.Empty;

        [ObservableProperty]
        private string operationName = string.Empty;

        [ObservableProperty]
        private double planningQuantity;

        [ObservableProperty]
        private double actualQuantity;

        [ObservableProperty]
        private double stockQuantity;

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

        [ObservableProperty]
        public ObservableCollection<OutputOutsourceTransferV2SubProductModel> subProducts = new();

        [ObservableProperty]
        private double bomQuantity;

        [ObservableProperty]
        private double quantity ;



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
        public string LocTrackingIcon => "location-dot";
        public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

        public string VariantIcon => "bookmark";
        public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

        public string TrackingTypeIcon => "box-archive";
        public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

        public List<OutputOutsourceTransferProductDetailModel> Details { get; set; } = new();

        [ObservableProperty]
        double inputQuantity = default;


    }
}
