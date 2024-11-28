using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SeriLotModels;

public class SeriLotTransactionModel : SeriLotTransaction
{
	private bool _isSelected;
    private double _outputQuantity;

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

    public double OutputQuantity
	{
		get => _outputQuantity;
		set
		{
			if (_outputQuantity == value) return;
			_outputQuantity = value;
			NotifyPropertyChanged();
		}
	}
}
