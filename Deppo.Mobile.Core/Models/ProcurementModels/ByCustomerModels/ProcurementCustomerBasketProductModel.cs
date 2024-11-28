using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.LocationModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;

public class ProcurementCustomerBasketProductModel : INotifyPropertyChanged, IDisposable
{
    private int _itemReferenceId;
    private string _itemCode = string.Empty;
    private string _itemName = string.Empty;
    private int _mainItemReferenceId;
    private string _mainItemCode = string.Empty;
    private string _mainItemName = string.Empty;
    private int _subUnitsetReferenceId;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private int _unitsetReferenceId;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private double _stockQuantity;
    private double _orderQuantity;
    private double _procurementQuantity;
    private double _quantity;
    private double _remainingQuantity; // ProcurementQuantity - Quantity
    private bool _isVariant;
    private int _trackingType;
    private int _locTracking;
    private bool _isSelected;
    private string _image;
    private string _locTrackingIcon;
    private string _locTrackingIconColor;
    private string _variantIcon;
    private string _variantIconColor;
    private string _trackingTypeIcon;
    private string _trackingTypeIconColor;

    private Guid rejectionOid = Guid.Empty;
    private string rejectionCode = string.Empty;
    private string rejectionName = string.Empty;

    private string _destinationLocationCode = string.Empty;
    private int _destinationLocationReferenceId;
    public ProcurementCustomerBasketProductModel()
    {
    }

	public string DestinationLocationCode
	{
		get => _destinationLocationCode;
		set
		{
			if (_destinationLocationCode == value) return;
			_destinationLocationCode = value;
			NotifyPropertyChanged();
		}
	}

	[Browsable(false)]
    public int ItemReferenceId
    {
        get => _itemReferenceId;
        set
        {
            if (_itemReferenceId == value) return;
            _itemReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Ürün / Varyant Kodu")]
    public string ItemCode
    {
        get => _itemCode;
        set
        {
            if (_itemCode == value) return;
            _itemCode = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Ürün / Varyant Adı")]
    public string ItemName
    {
        get => _itemName;
        set
        {
            if (_itemName == value) return;
            _itemName = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    [Description("Ürün Varyantlı ise Varyant Reference Id, Varyantlı değilse Ürün Reference Id")]
    public int MainItemReferenceId
    {
        get => _mainItemReferenceId;
        set
        {
            if (_mainItemReferenceId == value) return;
            _mainItemReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Ürün Kodu"), Description("Ürün Varyantlı ise Varyant Kodu, Varyantlı değilse Ürün Kodu")]
    [Browsable(false)]
    public string MainItemCode
    {
        get => _mainItemCode;
        set
        {
            if (_mainItemCode == value) return;
            _mainItemCode = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Ürün Adı"), Description("Ürün Varyantlı ise Varyant Adı, Varyantlı değilse Ürün Adı")]
    [Browsable(false)]
    public string MainItemName
    {
        get => _mainItemName;
        set
        {
            if (_mainItemName == value) return;
            _mainItemName = value;
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

    [DisplayName("Birim Kodu")]
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

    [DisplayName("Birim Adı")]
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

    [DisplayName("Birim Seti Kodu")]
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

    [DisplayName("Birim Seti Adı")]
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

    [DisplayName("Stok Miktarı")]
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

    [DisplayName("Sipariş Miktarı")]
    public double OrderQuantity
    {
        get => _orderQuantity;
        set
        {
            if (_orderQuantity == value) return;
            _orderQuantity = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Toplanacak Miktar")]
    public double ProcurementQuantity
    {
        get => _procurementQuantity;
        set
        {
            if (_procurementQuantity == value) return;
            _procurementQuantity = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(RemainingQuantity));
            NotifyPropertyChanged(nameof(ProcurementStatusText));
            NotifyPropertyChanged(nameof(ProcurementStatusTextColor));
        }
    }

    [DisplayName("Toplanan Miktar")]
    public double Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(RemainingQuantity));
            NotifyPropertyChanged(nameof(ProcurementStatusText));
            NotifyPropertyChanged(nameof(ProcurementStatusTextColor));
        }
    }

    [DisplayName("Kalan Toplanacak Miktar")]
    public double RemainingQuantity
    {
        get => ProcurementQuantity - Quantity;
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

	public int DestinationLocationReferenceId
	{
		get => _destinationLocationReferenceId;
		set
		{
			if (_destinationLocationReferenceId == value) return;
			_destinationLocationReferenceId = value;
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

    private string _procurementStatusText = "Bekliyor";

    public string ProcurementStatusText
    {
        get
        {
            if (!string.IsNullOrEmpty(RejectionCode))
            {
                return "Hata";
            }
            return ProcurementQuantity == Quantity ? "Tamamlandı" : "Bekliyor";
        }
        set
        {
            if (_procurementStatusText == value) return;
            _procurementStatusText = value;
            NotifyPropertyChanged();
        }
    }

    private string _procurementStatusTextColor = "#E6BE0C";

    public string ProcurementStatusTextColor
    {
        get
        {
            if (!string.IsNullOrEmpty(RejectionCode))
            {
                return "Red";
            }
            return ProcurementQuantity == Quantity ? "Green" : "#E6BE0C";
        }
        set
        {
            if (_procurementStatusTextColor == value) return;
            _procurementStatusTextColor = value;
            NotifyPropertyChanged();
        }
    }

    public Guid RejectionOid
    {
        get => rejectionOid;
        set
        {
            if (rejectionOid == value) return;
            rejectionOid = value;
            NotifyPropertyChanged();
        }
    }

    public string RejectionCode
    {
        get => rejectionCode;
        set
        {
            if (rejectionCode == value) return;
            rejectionCode = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(ProcurementStatusText));
            NotifyPropertyChanged(nameof(ProcurementStatusTextColor));
        }
    }

    public string RejectionName
    {
        get => rejectionName;
        set
        {
            if (rejectionName == value) return;
            rejectionName = value;
            NotifyPropertyChanged();
        }
    }

	public List<WaitingSalesOrder> Orders = new();
	public List<LocationModel> Locations= new();

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