using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.ProductModels;
using System;
using System.Collections.Generic;
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
        private double planningQuantity = default;

        [ObservableProperty]
        private double actualQuantity = default;

        [ObservableProperty]
        private double stockQuantity = default;

        [ObservableProperty]
        private string locTrackingIcon;

        [ObservableProperty]
        private string locTrackingIconColor;

        [ObservableProperty]
        private string variantIcon;

        [ObservableProperty]
        private string variantIconColor;

        [ObservableProperty]
        private string trackingTypeIcon;

        [ObservableProperty]
        private string trackingTypeIconColor;

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




















    }
}
