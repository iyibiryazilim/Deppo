using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models;

public class WaitingSalesProductOrder : WaitingProductOrder
{
	private int _supplierReferenceId;
	private string _supplierCode = string.Empty;
	private string _supplierName = string.Empty;
	public WaitingSalesProductOrder()
	{
	}

	public int SupplierReferenceId
	{
		get => _supplierReferenceId;
		set
		{
			if (_supplierReferenceId == value) return;
			_supplierReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	public string SupplierCode
	{
		get => _supplierCode;
		set
		{
			if (_supplierCode == value) return;
			_supplierCode = value;
			NotifyPropertyChanged();
		}
	}

	public string SupplierName
	{
		get => _supplierName;
		set
		{
			if (_supplierName == value) return;
			_supplierName = value;
			NotifyPropertyChanged();
		}
	}


}
