using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Deppo.Core.Models;

namespace Deppo.Core.BaseModels;

/// <summary>
/// Sipariş - LG_ORFICHE
/// </summary>
public class Order : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private DateTime _orderDate;
    private string _orderNumber = string.Empty;
    private string _documentNumber = string.Empty;
    private string _documentTrackingNumber = string.Empty;
    private int _shipAddressReferenceId;
    private ShipAddress? _shipAddress;

    public Order()
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

    public string DocumentNumber
    {
        get => _documentNumber;
        set
        {
            if (_documentNumber == value) return;
            _documentNumber = value;
            NotifyPropertyChanged();
        }
    }

    public string DocumentTrackingNumber
    {
        get => _documentTrackingNumber;
        set
        {
            if (_documentTrackingNumber == value) return;
            _documentTrackingNumber = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
    public int ShipAddressReferenceId
    {
        get => _shipAddressReferenceId;
        set
        {
            if (_shipAddressReferenceId == value) return;
            _shipAddressReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public ShipAddress? ShipAddress
    {
        get => _shipAddress;
        set
        {
            if (_shipAddress == value) return;
            _shipAddress = value;
            NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
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
