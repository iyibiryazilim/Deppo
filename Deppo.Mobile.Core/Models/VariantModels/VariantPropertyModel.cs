using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.VariantModels;

public class VariantPropertyModel : VariantProperty
{
    private bool _isSelected;

    public VariantPropertyModel()
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
