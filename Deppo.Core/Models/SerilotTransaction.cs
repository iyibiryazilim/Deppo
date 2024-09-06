using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models;

public class SeriLotTransaction : INotifyPropertyChanged, IDisposable
{
	private int _referenceId;
	private int _transactionReferenceId;
	private int _transactionFicheReferenceId;
	private int _serilotReferenceId;
	private int _inTransactionReferenceId;
	private int _inSerilotTransactionReferenceId;
	private string _serilotCode = string.Empty;
	private string _serilotName = string.Empty;
	private int _subUnitsetReferenceId;
	private string _subUnitsetCode = string.Empty;
	private string _subUnitsetName = string.Empty;
	private int _unitsetReferenceId;
	private string _unitsetCode = string.Empty;
	private string _unitsetName = string.Empty;
	private int itemReferenceId;
	private string _itemCode = string.Empty;
	private string _itemName = string.Empty;
	private double _quantity;
	private double _remainingQuantity;
	private double _remainingUnitQuantity;


    public SeriLotTransaction()
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
	public int TransactionReferenceId
	{
		get => _transactionReferenceId;
		set
		{
			if (_transactionReferenceId == value) return;
			_transactionReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	[Browsable(false)]
	public int TransactionFicheReferenceId
	{
		get => _transactionFicheReferenceId;
		set
		{
			if (_transactionFicheReferenceId == value) return;
			_transactionFicheReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	[Browsable(false)]
	public int SerilotReferenceId
	{
		get => _serilotReferenceId;
		set
		{
			if (_serilotReferenceId == value) return;
			_serilotReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	[Browsable(false)]
	public int InTransactionReferenceId
	{
		get => _inTransactionReferenceId;
		set
		{
			if (_inTransactionReferenceId == value) return;
			_inTransactionReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	[Browsable(false)]
	public int InSerilotTransactionReferenceId
	{
		get => _inSerilotTransactionReferenceId;
		set
		{
			if (_inSerilotTransactionReferenceId == value) return;
			_inSerilotTransactionReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	public string SerilotCode
	{
		get => _serilotCode;
		set
		{
			if (_serilotCode == value) return;
			_serilotCode = value;
			NotifyPropertyChanged();
		}
	}

	public string SerilotName
	{
		get => _serilotName;
		set
		{
			if (_serilotName == value) return;
			_serilotName = value;
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

	[Browsable(false)]
	public int ItemReferenceId
	{
		get => itemReferenceId;
		set
		{
			if (itemReferenceId == value) return;
			itemReferenceId = value;
			NotifyPropertyChanged();
		}
	}

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

	public double RemainingQuantity
	{
		get => _remainingQuantity;
		set
		{
			if (_remainingQuantity == value) return;
			_remainingQuantity = value;
			NotifyPropertyChanged();
		}
	}

	public double RemainingUnitQuantity
	{
		get => _remainingUnitQuantity;
		set
		{
			if (_remainingUnitQuantity == value) return;
			_remainingUnitQuantity = value;
			NotifyPropertyChanged();
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

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

	protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
