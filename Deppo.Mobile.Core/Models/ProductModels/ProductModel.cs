using System;
using Deppo.Core.BaseModels;

namespace Deppo.Mobile.Core.Models.ProductModels;

public class ProductModel : Product
{
    private bool _isSelected;

    public ProductModel()
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
