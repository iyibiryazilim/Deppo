using Deppo.Mobile.Core.Models.WarehouseModels;

namespace Deppo.Mobile.Core.Models.CountingModels
{
    public class WarehouseCountingWarehouseModel : WarehouseModel
    {
        private int _locationCount;
        public int LocationCount
        {
            get => _locationCount;
            set
            {
                _locationCount = value;
                NotifyPropertyChanged();
            }
        }
    }
}
