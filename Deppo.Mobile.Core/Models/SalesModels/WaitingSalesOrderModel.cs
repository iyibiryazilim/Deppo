using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SalesModels;

public class WaitingSalesOrderModel : WaitingSalesOrder
{
	private bool _isSelected;

	public WaitingSalesOrderModel()
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
