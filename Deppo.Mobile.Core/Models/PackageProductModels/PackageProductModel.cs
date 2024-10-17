using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PackageProductModels;

public class PackageProductModel : PackageProduct
{
    private bool _isSelected;
    private double _outputQuantity;

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
}
