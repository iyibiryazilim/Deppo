using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels;

public class BaseTransaction : INotifyPropertyChanged, IDisposable
{
    private int _transactionType;
    private string _transactionNumber = string.Empty;
    private string _transactionTypeName = string.Empty;
	private int _iOType;
	private string _iOTypeName = string.Empty;
	private string _iOTypeImageSource = string.Empty;
    private DateTime _transactionDate;
    private TimeSpan _transactionTime;
    private int _referenceId;
    private double _quantity;
    private string _subUnitsetCode;

    public BaseTransaction()
    {

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

    public string TransactionTypeName
    {
        get
        {
            switch(_transactionType)
            {
				case 1:
					return "Mal Alým Ýrsaliyesi";
				case 2:
					return "Perakende Satýþ Ýade Ýrsaliyesi";
				case 3:
					return "Toptan Satýþ Ýade Ýrsaliyesi";
				case 4:
					return "Konsinye Çýkýþ Ýade Ýrsaliyesi";
				case 5:
					return "Konsinye Giriþ Ýade Ýrsaliyesi";
				case 6:
					return "Alým Ýade Ýrsaliyesi";
				case 7:
					return "Perakende Satýþ Ýrsaliyesi";
				case 8:
					return "Toptan Satýþ Ýrsaliyesi";
				case 9:
					return "Konsinye Çýkýþ Ýrsaliyesi";
				case 10:
					return "Konsinye Giriþ Ýade Ýrsaliyesi";
				case 13:
					return "Üretimden Giriþ Fiþi";
				case 14:
					return "Devir Fiþi";
				case 12:
					return "Sarf Fiþi";
				case 11:
					return "Fire Fiþi";
				case 25:
					return "Ambar Fiþi";
				case 26:
					return "Mustahsil Ýrsaliyesi";
				case 50:
					return "Sayým Fazlasý Fiþi";
				case 51:
					return "Sayým Eksiði Fiþi";
				default:
					return "Diðer";
			}
        }
    }

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
					return "Giriþ";
				case 2:
					return "Giriþ";
				case 3:
					return "Çýkýþ";
				case 4:
					return "Çýkýþ";
				default:
					return "Diðer";
			}
		}
	}

	public string IOTypeImageSource
	{
		get
		{
			switch (_iOTypeName)
			{
				case "Giriþ":
					return "\uf062";
				case "Çýkýþ":
					return "\uf063";
				default:
					return "";
			}
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
