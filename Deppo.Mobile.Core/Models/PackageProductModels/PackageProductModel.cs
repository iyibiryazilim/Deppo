using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PackageProductModels;

public class PackageProductModel : PackageProduct
{
    private bool _isSelected;

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
}
