using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.BaseModels;

public class BaseTransaction : INotifyPropertyChanged, IDisposable
{
    private int _transactionType;
    private string _transactionTypeName = string.Empty;
    private DateTime _transactionDate;
    private TimeSpan _transactionTime;
    private int _referenceId;
    private double _quantity;

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
        }
    }

    public string TransactionTypeName
    {
        get => _transactionTypeName;
        set
        {
            if (_transactionTypeName == value) return;
            _transactionTypeName = value;
            NotifyPropertyChanged(nameof(TransactionTypeName));
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
