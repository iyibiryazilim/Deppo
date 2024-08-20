using System.ComponentModel;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

public class WarehouseTransaction : BaseTransaction
{
	private int _warehouseReferenceId;
	private string _warehouseName = string.Empty;

	public WarehouseTransaction()
	{

	}

	[Browsable(false)]
	public int WarehouseReferenceId
	{
		get => _warehouseReferenceId;
		set
		{
			if (_warehouseReferenceId == value) return;
			_warehouseReferenceId = value;
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
