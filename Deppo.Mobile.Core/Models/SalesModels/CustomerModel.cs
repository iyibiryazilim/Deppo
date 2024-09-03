namespace Deppo.Mobile.Core.Models.SalesModels;

public class CustomerModel : Customer
{
	private bool _isSelected;
	private int _orderReferenceCount;

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

	public int OrderReferenceCount
	{
		get => _orderReferenceCount;
		set
		{
			_orderReferenceCount = value;
			NotifyPropertyChanged();
		}
	}
}
