using System;
using Deppo.Core.BaseModels;

namespace Deppo.Mobile.Core.Models.ProductModels;

public class ProductModel : Product
{
    private bool _isSelected;
    private string _locTrackingIcon;
    private string _locTrackingIconColor;

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

    public string LocTrackingIcon
    {
        get => _locTrackingIcon = _locTrackingIcon ?? "ic_location";
        set
        {
            if (_locTrackingIcon == value) return;
            _locTrackingIcon = value;
            NotifyPropertyChanged();
        }
    }

    public string LocTrackingIconColor => LocTracking == 1 ? "Green" : "Gray200";
}
