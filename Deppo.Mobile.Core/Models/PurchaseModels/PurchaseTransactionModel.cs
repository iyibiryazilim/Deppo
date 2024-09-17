using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PurchaseModels
{
    public class PurchaseTransactionModel : PurchaseTransaction
    {
        private bool _isSelected;

        public PurchaseTransactionModel()
        {          
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
