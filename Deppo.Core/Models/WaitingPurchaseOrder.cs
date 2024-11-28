using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models;

public class WaitingPurchaseOrder : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _orderReferenceId;
    private string _orderNumber = string.Empty;
    private int _productReferenceId;
    private string _productName = string.Empty;
    private string _productCode = string.Empty;
    private string? _image = string.Empty;
    private int _unitsetReferenceId;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private int _subUnitsetReferenceId;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private int _quantity;
    private int _shippedQuantity;
    private int _waitingQuantity;
    private DateTime _orderDate;
    private DateTime _dueDate;
    private int _supplierReferenceId;
    private string _supplierName = string.Empty;
    private string _supplierCode = string.Empty;
    private bool _isVariant;
    private int _variantReferenceId;
    private string _variantCode = string.Empty;
    private string _variantName = string.Empty;
    private int _locTracking;
    private int _trackingType;

    public WaitingPurchaseOrder()
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

    public string OrderNumber
    {
        get => _orderNumber;
        set
        {
            if (_orderNumber == value) return;
            _orderNumber = value;
            NotifyPropertyChanged();
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

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            NotifyPropertyChanged(nameof(Quantity));
        }
    }

    public int ShippedQuantity
    {
        get => _shippedQuantity;
        set
        {
            if (_shippedQuantity == value) return;
            _shippedQuantity = value;
            NotifyPropertyChanged(nameof(ShippedQuantity));
        }
    }

    public int WaitingQuantity
    {
        get => _waitingQuantity;
        set
        {
            if (_waitingQuantity == value) return;
            _waitingQuantity = value;
            NotifyPropertyChanged(nameof(WaitingQuantity));
        }
    }

    public DateTime OrderDate
    {
        get => _orderDate;
        set
        {
            if (_orderDate == value) return;
            _orderDate = value;
            NotifyPropertyChanged(nameof(OrderDate));
        }
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            if (_dueDate == value) return;
            _dueDate = value;
            NotifyPropertyChanged(nameof(DueDate));
        }
    }

    public int SupplierReferenceId
    {
        get => _supplierReferenceId;
        set
        {
            if (_supplierReferenceId == value) return;
            _supplierReferenceId = value;
            NotifyPropertyChanged(nameof(SupplierReferenceId));
        }
    }

    public string SupplierCode
    {
        get => _supplierCode;
        set
        {
            if (_supplierCode == value) return;
            _supplierCode = value;
            NotifyPropertyChanged(nameof(SupplierCode));
        }
    }

    public string SupplierName
    {
        get => _supplierName;
        set
        {
            if (_supplierName == value) return;
            _supplierName = value;
            NotifyPropertyChanged(nameof(SupplierName));
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

    public int VariantReferenceId
    {
        get => _variantReferenceId;
        set
        {
            if (_variantReferenceId == value) return;
            _variantReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public string VariantCode
    {
        get => _variantCode;
        set
        {
            if (_variantCode == value) return;
            _variantCode = value;
            NotifyPropertyChanged();
        }
    }

    public string VariantName
    {
        get => _variantName;
        set
        {
            if (_variantName == value) return;
            _variantName = value;
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