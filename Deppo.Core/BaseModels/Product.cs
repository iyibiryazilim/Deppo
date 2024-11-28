using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using Deppo.Core.Models;

namespace Deppo.Core.BaseModels;

public class Product : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private double _vatRate;
    private int _subUnitsetReferenceId = default;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private int _unitsetReferenceId = default;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private Unitset? _unitset;
    private bool _isVariant;
    private int _trackingType;
    private int _locTracking;
    private double _stockQuantity;
    private string? _image;
    private int _brandReferenceId = default;
    private string _brandCode = string.Empty;
    private string _brandName = string.Empty;
    private string _groupCode = string.Empty;
    private bool isPurchased;

    private bool _isSelected;
    private string _locTrackingIcon;
    private string _locTrackingIconColor;
    private string _variantIcon;
    private string _variantIconColor;
    private string _trackingTypeIcon;
    private string _trackingTypeIconColor;

    public Product()
    {
    }

    [Key]
    public int ReferenceId
    {
        get => _referenceId;
        set
        {
            if (_referenceId == value) return;
            _referenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string Code
    {
        get => _code;
        set
        {
            if (_code == value) return;
            _code = value;
            NotifyPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            NotifyPropertyChanged();
        }
    }

    public double VatRate
    {
        get => _vatRate;
        set
        {
            if (_vatRate == value) return;
            _vatRate = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    public int SubUnitsetReferenceId
    {
        get => _subUnitsetReferenceId;
        set
        {
            if (_subUnitsetReferenceId == value) return;
            _subUnitsetReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string SubUnitsetCode
    {
        get => _subUnitsetCode;
        set
        {
            if (_subUnitsetCode == value) return;
            _subUnitsetCode = value;
            NotifyPropertyChanged();
        }
    }

    public string SubUnitsetName
    {
        get => _subUnitsetName;
        set
        {
            if (_subUnitsetName == value) return;
            _subUnitsetName = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    public int UnitsetReferenceId
    {
        get => _unitsetReferenceId;
        set
        {
            if (_unitsetReferenceId == value) return;
            _unitsetReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string UnitsetCode
    {
        get => _unitsetCode;
        set
        {
            if (_unitsetCode == value) return;
            _unitsetCode = value;
            NotifyPropertyChanged();
        }
    }

    public string UnitsetName
    {
        get => _unitsetName;
        set
        {
            if (_unitsetName == value) return;
            _unitsetName = value;
            NotifyPropertyChanged();
        }
    }

    public Unitset? Unitset
    {
        get => _unitset;
        set
        {
            if (_unitset == value) return;
            _unitset = value;
            NotifyPropertyChanged();
        }
    }

    public bool IsVariant
    {
        get => _isVariant;
        set
        {
            if (_isVariant == value) return;
            _isVariant = value;
            NotifyPropertyChanged();
        }
    }

    public int TrackingType
    {
        get => _trackingType;
        set
        {
            if (_trackingType == value) return;
            _trackingType = value;
            NotifyPropertyChanged();
        }
    }

    public int LocTracking
    {
        get => _locTracking;
        set
        {
            if (_locTracking == value) return;
            _locTracking = value;
            NotifyPropertyChanged();
        }
    }

    public double StockQuantity
    {
        get => _stockQuantity;
        set
        {
            if (_stockQuantity == value) return;
            _stockQuantity = value;
            NotifyPropertyChanged();
        }
    }

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

    [Browsable(false)]
    public int BrandReferenceId
    {
        get => _brandReferenceId;
        set
        {
            if (_brandReferenceId == value) return;
            _brandReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string BrandCode
    {
        get => _brandCode;
        set
        {
            if (_brandCode == value) return;
            _brandCode = value;
            NotifyPropertyChanged();
        }
    }

    public string BrandName
    {
        get => _brandName;
        set
        {
            if (_brandName == value) return;
            _brandName = value;
            NotifyPropertyChanged();
        }
    }

    public string GroupCode
    {
        get => _groupCode;
        set
        {
            if (_groupCode == value) return;
            _groupCode = value;
            NotifyPropertyChanged();
        }
    }

    public bool IsPurchased
    {
        get => isPurchased;
        set
        {
            if (isPurchased == value) return;
            isPurchased = value;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            PropertyChanged = null;
        }
    }
}