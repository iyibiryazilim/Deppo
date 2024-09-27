using System;
using System.Collections.ObjectModel;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;

namespace Deppo.Mobile.Core.Models.TransferModels;

public class InProductModel : ProductModel
{
    private double _outputQuantity;
	private bool _isCompleted;
    public ObservableCollection<LocationModel> Locations { get; set; } = new ObservableCollection<LocationModel>();

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

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (_isCompleted == value) return;
            _isCompleted = value;
            NotifyPropertyChanged();
        }
    }



}
