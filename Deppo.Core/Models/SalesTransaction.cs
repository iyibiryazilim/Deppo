using System;
using System.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Core.Models;

namespace Deppo.Core.Models;

public class SalesTransaction : BaseTransaction
{
    private int _customerReferenceId;
    private Customer? _customer;

    [Browsable(false)]
    public int CustomerReferenceId
    {
        get => _customerReferenceId;
        set
        {
            if (_customerReferenceId == value) return;
            _customerReferenceId = value;
            NotifyPropertyChanged(nameof(CustomerReferenceId));
        }
    }

    public Customer? Customer
    {
        get => _customer;
        set
        {
            if (_customer == value) return;
            _customer = value;
            NotifyPropertyChanged(nameof(Customer));
        }
    }
}
