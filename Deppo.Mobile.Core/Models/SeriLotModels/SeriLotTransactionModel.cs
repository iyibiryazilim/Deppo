using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SeriLotModels;

public class SeriLotTransactionModel : SeriLotTransaction
{
	private bool _isSelected;

    public SeriLotTransactionModel()
    {
        
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
}
