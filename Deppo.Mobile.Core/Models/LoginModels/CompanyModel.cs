using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.LoginModels;

public class CompanyModel : Company
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
