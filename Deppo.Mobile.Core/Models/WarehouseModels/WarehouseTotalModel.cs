using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.WarehouseModels;

public class WarehouseTotalModel : WarehouseTotal
{
    private bool _isSelected;

    public WarehouseTotalModel()
    {

    }


    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            NotifyPropertyChanged(nameof(IsSelected));
        }
    }
}
