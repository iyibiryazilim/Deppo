using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PurchaseModels
{
    public class PurchaseFicheModel : PurchaseFiche
    {
        private bool _isSelected;
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
    }
    
}
