using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SalesModels;

public class CustomerOrderModel : CustomerModel
{
    private ShipAddress _shipAddress = null!;

    private double distributedQuantity = default;

    
    public double DistributedQuantity
	{
		get => distributedQuantity;
		set
		{
            if (distributedQuantity == value) return;
			distributedQuantity = value;
			NotifyPropertyChanged();
		}
	}

	public ShipAddress ShipAddress
    {
        get => _shipAddress;
        set
        {
            _shipAddress = value;
            NotifyPropertyChanged();
        }
    }

}
