using Deppo.Core.BaseModels;

namespace Deppo.Core.Models
{
    public class SalesInventoryTurnover : InventoryTurnover
    {
        private double _salesQuantity;
        public double SalesQuantity
        {
            get => _salesQuantity;
            set
            {
                if (_salesQuantity == value) return;
                _salesQuantity = value;
                NotifyPropertyChanged();
            }
        }
    }
}
