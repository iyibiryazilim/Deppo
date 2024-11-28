using System;
using Deppo.Core.BaseModels;

namespace Deppo.Mobile.Core.Models.ProductModels;

public class ProductModel : Product
{
    private bool _isSelected;
    private string _locTrackingIcon;
    private string _locTrackingIconColor;
    private string _variantIcon;
    private string _variantIconColor;
    private string _trackingTypeIcon;
    private string _trackingTypeIconColor;

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
        get => _locTrackingIcon = _locTrackingIcon ?? "location-dot";
        set
        {
            if (_locTrackingIcon == value) return;
            _locTrackingIcon = value;
            NotifyPropertyChanged();
        }
    }

    public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

    public string VariantIcon
	{
		get => _variantIcon = _variantIcon ?? "bookmark";
		set
		{
			if (_variantIcon == value) return;
			_variantIcon = value;
			NotifyPropertyChanged();
		}
	}

    public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

    public string TrackingTypeIcon
    {
        get => _trackingTypeIcon = _trackingTypeIcon ?? "box-archive";
        set
        {
            if (_trackingTypeIcon == value) return;
            _trackingTypeIcon = value;
            NotifyPropertyChanged();
        }
    }

	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

}
