using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.WarehouseModels;

public class WarehouseModel : Warehouse
{
    private bool _isSelected;

    public WarehouseModel()
    {
        
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            NotifyPropertyChanged();
        }
    }


}
