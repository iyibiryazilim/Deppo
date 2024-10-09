using Deppo.Mobile.Core.Models.WarehouseModels;

namespace Deppo.Mobile.Core.Models.ProductModels
{
    public class VariantWarehouseTotalModel : WarehouseTotalModel
    {
        private int _variantReferenceId;
        private string _variantName;
        private string _variantCode;
        private double _safeLevel;

        public VariantWarehouseTotalModel()
        {
        }
        public int VariantReferenceId
        {
            get => _variantReferenceId;
            set
            {
                if (_variantReferenceId == value) return;
                _variantReferenceId = value;
                NotifyPropertyChanged();
            }
        }

        public string VariantName
        {
            get => _variantName;
            set
            {
                if (_variantName == value) return;
                _variantName = value;
                NotifyPropertyChanged();
            }
        }

        public string VariantCode
        {
            get => _variantCode;
            set
            {
                if (_variantCode == value) return;
                _variantCode = value;
                NotifyPropertyChanged();
            }
        }

        public double SafeLevel
        {
            get => _safeLevel;
            set
            {
                if (_safeLevel == value) return;
                _safeLevel = value;
                NotifyPropertyChanged();
            }
        }
    }
}
