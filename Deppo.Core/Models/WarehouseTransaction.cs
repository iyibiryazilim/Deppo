using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

public class WarehouseTransaction : BaseTransaction
{
	private int _warehouseNumber;
	private string _warehouseName = string.Empty;

	public int WarehouseNumber
	{
		get => _warehouseNumber;
		set
		{
			if (_warehouseNumber == value) return;
			_warehouseNumber = value;
			NotifyPropertyChanged();
		}
	}

	public string WarehouseName
	{
		get => _warehouseName;
		set
		{
			if (_warehouseName == value) return;
			_warehouseName = value;
			NotifyPropertyChanged();
		}
	}

}
