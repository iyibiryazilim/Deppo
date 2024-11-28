using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.ShipAddressModels;

public class ShipAddressModel : ShipAddress
{
	private bool _isSelected;

    public ShipAddressModel()
    {
        
    }

    public bool IsSelected
	{
		get => _isSelected;
		set
		{
			_isSelected = value;
			NotifyPropertyChanged();
		}
	}
}
