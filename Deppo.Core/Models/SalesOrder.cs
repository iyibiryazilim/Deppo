using System;
using System.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Core.Models;

namespace Deppo.Core.Models;

/// <summary>
/// Satış Siparişi - LG_ORFICHE
/// </summary>
public class SalesOrder : Order
{
    private int _customerReferenceId;
    private Customer? _customer;

    public SalesOrder()
    {

    }

    [Browsable(false)]
    public int CustomerReferenceId
    {
        get => _customerReferenceId;
        set
        {
            if (_customerReferenceId == value) return;
            _customerReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public Customer? Customer
    {
        get => _customer;
        set
        {
            if (_customer == value) return;
            _customer = value;
            NotifyPropertyChanged();
        }
    }

}
