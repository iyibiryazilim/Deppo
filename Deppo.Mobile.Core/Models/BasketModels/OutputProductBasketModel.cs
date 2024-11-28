using Deppo.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Mobile.Core.Models.BasketModels;

public class OutputProductBasketModel : INotifyPropertyChanged, IDisposable
{
    private Guid? _referenceId;
    private int _itemReferenceId;
    private string _itemCode = string.Empty;
    private string _itemName = string.Empty;
    private int _mainItemReferenceId;
    private string _mainItemCode = string.Empty;
    private string _mainItemName = string.Empty;
    private int _subUnitsetReferenceId;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private double _conversionFactor = 1;
    private double _otherConversionFactor = 1;
    private int _unitsetReferenceId;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private double _stockQuantity;
    private double _quantity;
    private bool _isVariant;
    private int _trackingType;
    private int _locTracking;
    private bool _isSelected;
    private byte[]? _image;

    private List<OutputProductBasketDetailModel> _details = new();
    private ObservableCollection<SubUnitset> _subUnitsets = new();

	private string _locTrackingIcon;
	private string _locTrackingIconColor;
	private string _variantIcon;
	private string _variantIconColor;
	private string _trackingTypeIcon;
	private string _trackingTypeIconColor;

	public OutputProductBasketModel()
    {
        ReferenceId = Guid.NewGuid();
    }

    [Browsable(false)]
    public Guid? ReferenceId
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

	public double ConversionFactor
	{
		get => _conversionFactor;
		set
		{
			if (_conversionFactor == value) return;
			_conversionFactor = value;
			NotifyPropertyChanged();
		}
	}

	public double OtherConversionFactor
	{
		get => _otherConversionFactor;
		set
		{
			if (_otherConversionFactor == value) return;
			_otherConversionFactor = value;
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

    [DisplayName("Miktar")]
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

    public byte[]? Image
    {
        get => _image;
        set
        {
            if (_image == value) return;
            _image = value;
            NotifyPropertyChanged();
        }
    }

    public List<OutputProductBasketDetailModel> Details
    {
        get => _details;
        set
        {
            if (_details == value) return;
            _details = value;
            NotifyPropertyChanged();
        }
    }

    public ObservableCollection<SubUnitset> SubUnitsets
	{
		get => _subUnitsets;
		set
		{
			if (_subUnitsets == value) return;
			_subUnitsets = value;
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
