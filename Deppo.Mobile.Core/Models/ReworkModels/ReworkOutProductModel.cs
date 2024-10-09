using Deppo.Mobile.Core.Models.WarehouseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ReworkModels;

public class ReworkOutProductModel : WarehouseTotalModel
{
	private double _outputQuantity;

    public ReworkOutProductModel()
    {
        
    }

    public double OutputQuantity
	{
		get => _outputQuantity;
		set
		{
			if (_outputQuantity == value) return;
			_outputQuantity = value;
			NotifyPropertyChanged();
		}
	}
}
