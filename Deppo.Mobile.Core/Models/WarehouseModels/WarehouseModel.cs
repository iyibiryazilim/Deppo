using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.WarehouseModels;

public class WarehouseModel : Warehouse
{
    private bool _isSelected;
    private string _locationCode = string.Empty;

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

    public string LocationCode
    {
        get => _locationCode;
        set
        {
            if (_locationCode == value) return;
            _locationCode = value;
            NotifyPropertyChanged();
        }
    }
}
