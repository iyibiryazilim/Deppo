using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SalesModels;

public class CustomerModel : Customer
{
	private bool _isSelected;
	
	public CustomerModel()
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
