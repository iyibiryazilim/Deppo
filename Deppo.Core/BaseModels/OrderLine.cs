using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deppo.Core.Models;

namespace Deppo.Core.BaseModels;

/// <summary>
/// Sipariş Satırı - LG_ORFLINE
/// </summary>
public class OrderLine : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _productReferenceId;
    private Product? _product;
    private int _unitsetReferenceId;
    private Unitset? _unitset;
    private int _subUnitsetReferenceId;
    private SubUnitset? _subUnitset;
    private int _quantity;
    private double _unitPrice;
    private double _total;
    private double _discount;
    private double _totalVat;
    private double _netTotal;
    private DateTime? _dueDate;

    public OrderLine()
    {

    }

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

    public Unitset? Unitset
    {
        get => _unitset;
        set
        {
            if (_unitset == value) return;
            _unitset = value;
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

    public SubUnitset? SubUnitset
    {
        get => _subUnitset;
        set
        {
            if (_subUnitset == value) return;
            _subUnitset = value;
            NotifyPropertyChanged();
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            NotifyPropertyChanged();
        }
    }

    public double UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (_unitPrice == value) return;
            _unitPrice = value;
            NotifyPropertyChanged();
        }
    }

    public double Total
    {
        get => _total;
        set
        {
            if (_total == value) return;
            _total = value;
            NotifyPropertyChanged();
        }
    }

    public double Discount
    {
        get => _discount;
        set
        {
            if (_discount == value) return;
            _discount = value;
            NotifyPropertyChanged();
        }
    }

    public double TotalVat
    {
        get => _totalVat;
        set
        {
            if (_totalVat == value) return;
            _totalVat = value;
            NotifyPropertyChanged();
        }
    }

    public double NetTotal
    {
        get => _netTotal;
        set
        {
            if (_netTotal == value) return;
            _netTotal = value;
            NotifyPropertyChanged();
        }
    }

    public DateTime? DueDate
    {
        get => _dueDate;
        set
        {
            if (_dueDate == value) return;
            _dueDate = value;
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
