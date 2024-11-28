using Deppo.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models
{
    public class QuicklyBOMProduct : Product
    {
		private int _warehouseNumber = default;
		private string _warehouseName = string.Empty;
		private double _amount = default;

		public double Amount
		{
			get => _amount;
			set
			{
				if (_amount == value) return;
				_amount = value;
				NotifyPropertyChanged();
			}
		}

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
}
