using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels;

public class BaseTransaction : INotifyPropertyChanged, IDisposable
{
	private int _referenceId;
	private int _transactionReferenceId;
	private string _transactionNumber = string.Empty;
	private DateTime _transactionDate;
	private TimeSpan _transactionTime;
	private int _transactionType;
	private int _groupCode;
	private int _iOType;
	private string _iOTypeName = string.Empty;
	private int _itemReferenceId;
	private string _itemCode = string.Empty;
	private string _itemName = string.Empty;
	private int _unitsetReferenceId;
	private string _unitsetCode = string.Empty;
	private string _unitsetName = string.Empty;
	private int _subUnitsetReferenceId;
	private string _subUnitsetCode = string.Empty;
	private string _subUnitsetName = string.Empty;
	private int _warehouseNumber;
	private double _quantity;
	private double _length;
	private double _width;
	private double _height;
	private double _weight;
	private double _volume;
	private string _barcode = string.Empty;

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
	public int TransactionReferenceId
	{
		get => _transactionReferenceId;
		set
		{
			if (_transactionReferenceId == value) return;
			_transactionReferenceId = value;
			NotifyPropertyChanged(nameof(TransactionReferenceId));
		}
	}

	public string TransactionNumber
	{
		get => _transactionNumber;
		set
		{
			if (_transactionNumber == value) return;
			_transactionNumber = value;
			NotifyPropertyChanged(nameof(TransactionNumber));
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
					return "Mal Al�m �rsaliyesi";
				case 2:
					return "Perakende Sat�� �ade �rsaliyesi";
				case 3:
					return "Toptan Sat�� �ade �rsaliyesi";
				case 4:
					return "Konsinye ��k�� �ade �rsaliyesi";
				case 5:
					return "Konsinye Giri� �ade �rsaliyesi";
				case 6:
					return "Al�m �ade �rsaliyesi";
				case 7:
					return "Perakende Sat�� �rsaliyesi";
				case 8:
					return "Toptan Sat�� �rsaliyesi";
				case 9:
					return "Konsinye ��k�� �rsaliyesi";
				case 10:
					return "Konsinye Giri� �ade �rsaliyesi";
				case 13:
					return "�retimden Giri� Fi�i";
				case 14:
					return "Devir Fi�i";
				case 12:
					return "Sarf Fi�i";
				case 11:
					return "Fire Fi�i";
				case 25:
					return "Ambar Fi�i";
				case 26:
					return "Mustahsil �rsaliyesi";
				case 50:
					return "Say�m Fazlas� Fi�i";
				case 51:
					return "Say�m Eksi�i Fi�i";
				default:
					return "Di�er";
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
			NotifyPropertyChanged(nameof(IOTypeImageSource));
		}
	}

	public string IOTypeName
	{
		get
		{
			switch (_iOType)
			{
				case 1:
					return "Giri�";
				case 2:
					return "Giri�";
				case 3:
					return "��k��";
				case 4:
					return "��k��";
				default:
					return "Di�er";
			}
		}
	}

	public string IOTypeImageSource
	{
		get
		{
			switch (_iOTypeName)
			{
				case "Giri�":
					return "\uf062";
				case "��k��":
					return "\uf063";
				default:
					return "";
			}
		}
	}

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
	public int ItemReferenceId
	{
		get => _itemReferenceId;
		set
		{
			if (_itemReferenceId == value) return;
			_itemReferenceId = value;
			NotifyPropertyChanged(nameof(ItemReferenceId));
		}
	}

	public string ItemCode
	{
		get => _itemCode;
		set
		{
			if (_itemCode == value) return;
			_itemCode = value;
			NotifyPropertyChanged(nameof(ItemCode));
		}
	}

	public string ItemName
	{
		get => _itemName;
		set
		{
			if (_itemName == value) return;
			_itemName = value;
			NotifyPropertyChanged(nameof(ItemName));
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
