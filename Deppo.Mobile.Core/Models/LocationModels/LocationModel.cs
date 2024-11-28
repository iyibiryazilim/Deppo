using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.LocationModels;

public class LocationModel : Deppo.Core.Models.Location
{
    private bool _isSelected =false;
    private double _inputQuantity = default;

    public LocationModel()
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

    public double InputQuantity
    {
        get => _inputQuantity;
        set
        {
            if (_inputQuantity == value) return;
            _inputQuantity = value;
            NotifyPropertyChanged();
        }
    }

}
