using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.ShipAddressModels;

public class ShipAdressModel : ShipAddress
{
	private bool _isSelected;

    public ShipAdressModel()
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
