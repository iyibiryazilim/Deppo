using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PackageProductModels;

public class PackageProductModel : PackageProduct
{
    private bool _isSelected;
    private double _outputQuantity;
    private double _volume;
    private double _weight;

    public PackageProductModel()
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

    public double Volume
    {
        get => _volume;
        set
        {
            if (_volume == value) return;
            _volume = value;
            NotifyPropertyChanged();
        }
    }

    public double Weight
    {
        get => _weight;
        set
        {
            if (_weight == value) return;
            _weight = value;
            NotifyPropertyChanged();
        }
    }
}
