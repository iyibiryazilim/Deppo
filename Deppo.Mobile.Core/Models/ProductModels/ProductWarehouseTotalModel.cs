using Deppo.Mobile.Core.Models.WarehouseModels;

namespace Deppo.Mobile.Core.Models.ProductModels
{
    public class ProductWarehouseTotalModel : WarehouseTotalModel
    {
        private double _safeLevel;

        public ProductWarehouseTotalModel()
        {
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
