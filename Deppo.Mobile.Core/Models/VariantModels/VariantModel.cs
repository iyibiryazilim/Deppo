using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.VariantModels;

public class VariantModel : Variant
{
    private bool _isSelected;

    public VariantModel()
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
