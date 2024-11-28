using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels;

public class BaseTransaction : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _baseTransactionReferenceId;
    private string _baseTransactionCode = string.Empty;
    private DateTime _transactionDate;
    private TimeSpan _transactionTime;
    private int _transactionType;
    private int _groupCode;
    private int _iOType;
    private string _iOTypeName = string.Empty;
    private int _productReferenceId;
    private string _productCode = string.Empty;
    private string _productName = string.Empty;
    private int _unitsetReferenceId;
    private string _unitsetCode = string.Empty;
    private string _unitsetName = string.Empty;
    private int _subUnitsetReferenceId;
    private string _subUnitsetCode = string.Empty;
    private string _subUnitsetName = string.Empty;
    private int _warehouseNumber;
    private string _warehouseName = string.Empty;
    private double _quantity;
    private double _length;
    private double _width;
    private double _height;
    private double _weight;
    private double _volume;
    private string _barcode = string.Empty;
    private string? _image;

    public BaseTransaction()
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
            NotifyPropertyChanged(nameof(ReferenceId));
        }
    }

    [Browsable(false)]
    public int BaseTransactionReferenceId
    {
        get => _baseTransactionReferenceId;
        set
        {
            if (_baseTransactionReferenceId == value) return;
            _baseTransactionReferenceId = value;
            NotifyPropertyChanged(nameof(BaseTransactionReferenceId));
        }
    }

    public string BaseTransactionCode
    {
        get => _baseTransactionCode;
        set
        {
            if (_baseTransactionCode == value) return;
            _baseTransactionCode = value;
            NotifyPropertyChanged(nameof(BaseTransactionCode));
        }
    }

    public DateTime TransactionDate
    {
        get => _transactionDate;
        set
        {
            if (_transactionDate == value) return;
            _transactionDate = value;
            NotifyPropertyChanged(nameof(TransactionDate));
        }
    }

    public TimeSpan TransactionTime
    {
        get => _transactionTime;
        set
        {
            if (_transactionTime == value) return;
            _transactionTime = value;
            NotifyPropertyChanged(nameof(TransactionTime));
        }
    }

    [Browsable(false)]
    public int TransactionType
    {
        get => _transactionType;
        set
        {
            if (_transactionType == value) return;
            _transactionType = value;
            NotifyPropertyChanged(nameof(TransactionType));
            NotifyPropertyChanged(nameof(TransactionTypeName));
        }
    }

    public string TransactionTypeName
    {
        get
        {
            switch (_transactionType)
            {
                case 1:
                    return "Mal Alım İrsaliyesi";

                case 2:
                    return "Perakende Satış İade İrsaliyesi";

                case 3:
                    return "Toptan Satış İade İrsaliyesi";

                case 4:
                    return "Konsinye Çıkış İade İrsaliyesi";

                case 5:
                    return "Konsinye Giriş İade İrsaliyesi";

                case 6:
                    return "Satınalma İade İrsaliyesi";

                case 7:
                    return "Perakende Satış İrsaliyesi";

                case 8:
                    return "Toptan Satış İrsaliyesi";

                case 9:
                    return "Konsinye Çıkış İrsaliyesi";

                case 10:
                    return "Konsinye Giriş İade İrsaliyesi";

                case 13:
                    return "Üretimden Giriş Fişi";

                case 14:
                    return "Devir Fişi";

                case 12:
                    return "Sarf Fişi";

                case 11:
                    return "Fire Fişi";

                case 25:
                    return "Ambar Fişi";

                case 26:
                    return "Mustahsil İrsaliyesi";

                case 50:
                    return "Sayım Fazlası Fişi";

                case 51:
                    return "Sayım Eksiği Fişi";

                default:
                    return "Diğer";
            }
        }
    }

    [Browsable(false)]
    public int IOType
    {
        get => _iOType;
        set
        {
            if (_iOType == value) return;
            _iOType = value;
            NotifyPropertyChanged(nameof(IOType));
            NotifyPropertyChanged(nameof(IOTypeName));
        }
    }

    public string IOTypeName
    {
        get
        {
            switch (_iOType)
            {
                case 1:
                    return "Giriş";

                case 2:
                    return "Giriş";

                case 3:
                    return "Çıkış";

                case 4:
                    return "Çıkış";

                default:
                    return "Diğer";
            }
        }
    }

    public string IOTypeImageSource => IOTypeName switch
    {
        "Giriş" => "arrow-down",
        "Çıkış" => "arrow-up",
        _ => ""
    };

    public string IOTypeColor => IOTypeName switch
    {
        "Giriş" => "#44B0C0",
        "Çıkış" => "#F5004F",
        _ => "#141414"
    };

    [Browsable(false)]
    public int GroupCode
    {
        get => _groupCode;
        set
        {
            if (_groupCode == value) return;
            _groupCode = value;
            NotifyPropertyChanged(nameof(GroupCode));
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

    public double Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            NotifyPropertyChanged(nameof(Quantity));
        }
    }

    public double Length
    {
        get => _length;
        set
        {
            if (_length == value) return;
            _length = value;
            NotifyPropertyChanged(nameof(Length));
        }
    }

    public double Width
    {
        get => _width;
        set
        {
            if (_width == value) return;
            _width = value;
            NotifyPropertyChanged(nameof(Width));
        }
    }

    public double Height
    {
        get => _height;
        set
        {
            if (_height == value) return;
            _height = value;
            NotifyPropertyChanged(nameof(Height));
        }
    }

    public double Weight
    {
        get => _weight;
        set
        {
            if (_weight == value) return;
            _weight = value;
            NotifyPropertyChanged(nameof(Weight));
        }
    }

    public double Volume
    {
        get => _volume;
        set
        {
            if (_volume == value) return;
            _volume = value;
            NotifyPropertyChanged(nameof(Volume));
        }
    }

    public string Barcode
    {
        get => _barcode;
        set
        {
            if (_barcode == value) return;
            _barcode = value;
            NotifyPropertyChanged(nameof(Barcode));
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