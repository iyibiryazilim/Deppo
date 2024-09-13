using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;

public class InputPurchaseBasketOrderModel : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _orderReferenceId;
    private int _supplierReferenceId;
    private string _supplierCode = string.Empty;
    private string _supplierName = string.Empty;
    private int _productReferenceId;
    private string _productCode = string.Empty;
    private string _productName = string.Empty;
    private int _unitsetReferenceId;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private int _subUnitsetReferenceId;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private double _quantity;
    private double _shippedQuantity;
    private double _waitingQuantity;
    private bool _isVariant;
    private int _trackingType;
    private int _locTracking;
    private bool _isSelected;
    private DateTime _orderDate;
    private DateTime _dueDate;
    private string _image;

    private string _locTrackingIcon;
    private string _locTrackingIconColor;
    private string _variantIcon;
    private string _variantIconColor;
    private string _trackingTypeIcon;
    private string _trackingTypeIconColor;

    public InputPurchaseBasketOrderModel()
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

    [Browsable(false)]
    public int OrderReferenceId
    {
        get => _orderReferenceId;
        set
        {
            if (_orderReferenceId == value) return;
            _orderReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    public int SupplierReferenceId
    {
        get => _supplierReferenceId;
        set
        {
            if (_supplierReferenceId == value) return;
            _supplierReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string SupplierCode
    {
        get => _supplierCode;
        set
        {
            if (_supplierCode == value) return;
            _supplierCode = value;
            NotifyPropertyChanged();
        }
    }

    public string SupplierName
    {
        get => _supplierName;
        set
        {
            if (_supplierName == value) return;
            _supplierName = value;
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

    public double Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            NotifyPropertyChanged();
        }
    }

    public double ShippedQuantity
    {
        get => _shippedQuantity;
        set
        {
            if (_shippedQuantity == value) return;
            _shippedQuantity = value;
            NotifyPropertyChanged();
        }
    }

    public double WaitingQuantity
    {
        get => _waitingQuantity;
        set
        {
            if (_waitingQuantity == value) return;
            _waitingQuantity = value;
            NotifyPropertyChanged();
        }
    }

    public DateTime OrderDate
    {
        get => _orderDate;
        set
        {
            if (_orderDate == value) return;
            _orderDate = value;
            NotifyPropertyChanged();
        }
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            if (_dueDate == value) return;
            _dueDate = value;
            NotifyPropertyChanged();
        }
    }

    public string Image
    {
        get => _image;
        set
        {
            if (_image == value) return;
            _image = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Varyant")]
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

    [DisplayName("Seri / Lot Takipli")]
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

    [DisplayName("Stok Yeri Takipli")]
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