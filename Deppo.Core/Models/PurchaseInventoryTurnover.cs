using Deppo.Core.BaseModels;

namespace Deppo.Core.Models
{
    public class PurchaseInventoryTurnover : InventoryTurnover
    {
        private double _purchaseQuantity;
        public double PurchaseQuantity
        {
            get => _purchaseQuantity;
            set
            {
                if (_purchaseQuantity == value) return;
                _purchaseQuantity = value;
                NotifyPropertyChanged();
            }
        }
    }
}
