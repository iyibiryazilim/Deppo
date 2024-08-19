using System;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

public class ProductTransaction : BaseTransaction
{
	private string _warehouseName = string.Empty;

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
