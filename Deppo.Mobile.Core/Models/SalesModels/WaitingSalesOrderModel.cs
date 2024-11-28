using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SalesModels;

public class WaitingSalesOrderModel : WaitingSalesOrder
{
    private string _shipInfoCode = string.Empty;
    private bool _isSelected;
    private string _locTrackingIcon;
    private string _locTrackingIconColor;
    private string _variantIcon;
    private string _variantIconColor;
    private string _trackingTypeIcon;
    private string _trackingTypeIconColor;
    private string? _image;

    public WaitingSalesOrderModel()
    {
    }

	public string ShipInfoCode
	{
		get => _shipInfoCode;
		set
		{
			if (_shipInfoCode == value) return;
			_shipInfoCode = value;
			NotifyPropertyChanged();
		}
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

    public string? Image
    {
        get => _image;
        set
        {
            if (_image == value) return;
            _image = value;
            NotifyPropertyChanged();
        }
    }

    public byte[] ImageData
    {
        get
        {
            if (string.IsNullOrEmpty(Image))
                return Array.Empty<byte>();
            else
            {
                return Convert.FromBase64String(Image);
            }
        }
    }
}