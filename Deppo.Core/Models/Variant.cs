using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

/// <summary>
/// Varyant - LG_VARIANT
/// </summary>
public class Variant : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private int _productReferenceId;
    private string _productCode = string.Empty;
    private string _productName = string.Empty;
    private int _vatRate = default;
    private int _subUnitsetReferenceId = default;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private int _unitsetReferenceId = default;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private int _trackingType = default;
    private int _locTracking = default;
    private double _stockQuantity = default;
    private string? _image = default;
    private int _brandReferenceId = default;
    private string _brandCode = string.Empty;
    private string _brandName = string.Empty;
    private string _groupCode = string.Empty;
    private Product? _product;

    public Variant()
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

    [Browsable(false)]
    public int ProductReferenceId
    {
        get => _productReferenceId;
        set
        {
            if (_productReferenceId == value) return;
            _productReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    public string ProductCode
    {
        get => _productCode;
        set
        {
            if (_productCode == value) return;
            _productCode = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    public string ProductName
    {
        get => _productName;
        set
        {
            if (_productName == value) return;
            _productName = value;
            NotifyPropertyChanged();
        }
    }

    public int VatRate
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

    [Browsable(false)]
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

    [Browsable(false)]
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

    [Browsable(false)]
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

    [Browsable(false)]
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

    public Product? Product
    {
        get => _product;
        set
        {
            if (_product == value) return;
            _product = value;
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