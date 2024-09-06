using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.LocationModels;

public class LocationTransactionModel : LocationTransaction
{
	private bool _isSelected = false;
	private double _outputQuantity = default;

	public LocationTransactionModel()
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
