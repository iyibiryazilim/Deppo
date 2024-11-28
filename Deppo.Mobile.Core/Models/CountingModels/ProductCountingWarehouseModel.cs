using Deppo.Mobile.Core.Models.WarehouseModels;

namespace Deppo.Mobile.Core.Models.CountingModels
{
    public class ProductCountingWarehouseModel : WarehouseModel
    {
        private int _locationCount;
        public int LocationCount
        {
            get => _locationCount;
            set
            {
                if (_locationCount == value) return;
                _locationCount = value;
                NotifyPropertyChanged(nameof(LocationCount));
            }
        }
    }
}
