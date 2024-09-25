using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.QuicklyModels
{
    public class QuicklyBOMProductModel : QuicklyBOMProduct
    {
        private int _referenceId2;
        private bool _isSelected;
        private string _locTrackingIcon;
        private string _locTrackingIconColor;
        private string _variantIcon;
        private string _variantIconColor;
        private string _trackingTypeIcon;
        private string _trackingTypeIconColor;

        private int _warehouseNumber = default;
        private string _warehouseName = string.Empty;
        private double _amount = default;

        public QuicklyBOMProductModel()
        {
        }



        public double Amount 
        {
            get => _amount;
            set
            {
                if (_amount == value) return;
                _amount = value;
                NotifyPropertyChanged();
            }
        }

        public int WarehouseNumber
        {
            get => _warehouseNumber;
            set
            {
                if (_warehouseNumber == value) return;
                _warehouseNumber = value;
                NotifyPropertyChanged();
            }
        }
        public string WarehouseName
        {
            get => _warehouseName;
            set
            {
                if (_warehouseName == value) return;
                _warehouseName = value;
                NotifyPropertyChanged();
            }
        }


        public int ReferenceId2
        {
            get => _referenceId2;
            set
            {
                if (_referenceId2 == value) return;
                _referenceId2 = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string LocTrackingIcon
        {
            get => _locTrackingIcon = _locTrackingIcon ?? "location-dot";
            set
            {
                if (_locTrackingIcon == value) return;
                _locTrackingIcon = value;
                NotifyPropertyChanged();
            }
        }

        public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

        public string VariantIcon
        {
            get => _variantIcon = _variantIcon ?? "bookmark";
            set
            {
                if (_variantIcon == value) return;
                _variantIcon = value;
                NotifyPropertyChanged();
            }
        }

        public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

        public string TrackingTypeIcon
        {
            get => _trackingTypeIcon = _trackingTypeIcon ?? "box-archive";
            set
            {
                if (_trackingTypeIcon == value) return;
                _trackingTypeIcon = value;
                NotifyPropertyChanged();
            }
        }

        public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";
    }
}