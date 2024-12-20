using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models;

public class WarehouseTotal : INotifyPropertyChanged, IDisposable
{
    private Guid _referenceId;
    private int _productReferenceId;
    private string _productCode = string.Empty;
    private string _productName = string.Empty;
    private int _unitsetReferenceId;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private int _subUnitsetReferenceId;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private bool _isVariant;
    private int _trackingType;
    private int _locTracking;
    private int _warehouseReferenceId;
    private int _warehouseNumber;
    private string _warehouseName = string.Empty;
    private double _stockQuantity;
    private int _brandReferenceId = default;
    private string _brandCode = string.Empty;
    private string _brandName = string.Empty;
    private string _groupCode = string.Empty;
    private string? _image;

    public WarehouseTotal()
    {

    }

    public Guid ReferenceId
    {
        get => _referenceId;
        set
        {
            if (_referenceId == value) return;
            _referenceId = value;
            NotifyPropertyChanged(nameof(ReferenceId));
        }
    }


    public int ProductReferenceId
    {
        get => _productReferenceId;
        set
        {
            if (_productReferenceId == value) return;
            _productReferenceId = value;
            NotifyPropertyChanged(nameof(ProductReferenceId));
        }
    }

    public string ProductCode
    {
        get => _productCode;
        set
        {
            if (_productCode == value) return;
            _productCode = value;
            NotifyPropertyChanged(nameof(ProductCode));
        }
    }

    public string ProductName
    {
        get => _productName;
        set
        {
            if (_productName == value) return;
            _productName = value;
            NotifyPropertyChanged(nameof(ProductName));
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
            NotifyPropertyChanged(nameof(UnitsetReferenceId));
        }
    }

    public string UnitsetCode
    {
        get => _unitsetCode;
        set
        {
            if (_unitsetCode == value) return;
            _unitsetCode = value;
            NotifyPropertyChanged(nameof(UnitsetCode));
        }
    }

    public string UnitsetName
    {
        get => _unitsetName;
        set
        {
            if (_unitsetName == value) return;
            _unitsetName = value;
            NotifyPropertyChanged(nameof(UnitsetName));
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
            NotifyPropertyChanged(nameof(SubUnitsetReferenceId));
        }
    }

    public string SubUnitsetCode
    {
        get => _subUnitsetCode;
        set
        {
            if (_subUnitsetCode == value) return;
            _subUnitsetCode = value;
            NotifyPropertyChanged(nameof(SubUnitsetCode));
        }
    }

    public string SubUnitsetName
    {
        get => _subUnitsetName;
        set
        {
            if (_subUnitsetName == value) return;
            _subUnitsetName = value;
            NotifyPropertyChanged(nameof(SubUnitsetName));
        }
    }

    public bool IsVariant
    {
        get => _isVariant;
        set
        {
            if (_isVariant == value) return;
            _isVariant = value;
            NotifyPropertyChanged(nameof(IsVariant));
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

    [Browsable(false)]
    public int WarehouseReferenceId
    {
        get => _warehouseReferenceId;
        set
        {
            if (_warehouseReferenceId == value) return;
            _warehouseReferenceId = value;
            NotifyPropertyChanged(nameof(WarehouseReferenceId));
        }
    }

    public int WarehouseNumber
    {
        get => _warehouseNumber;
        set
        {
            if (_warehouseNumber == value) return;
            _warehouseNumber = value;
            NotifyPropertyChanged(nameof(WarehouseNumber));
        }
    }

    public string WarehouseName
    {
        get => _warehouseName;
        set
        {
            if (_warehouseName == value) return;
            _warehouseName = value;
            NotifyPropertyChanged(nameof(WarehouseName));
        }
    }

    public double StockQuantity
    {
        get => _stockQuantity;
        set
        {
            if (_stockQuantity == value) return;
            _stockQuantity = value;
            NotifyPropertyChanged(nameof(StockQuantity));
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
