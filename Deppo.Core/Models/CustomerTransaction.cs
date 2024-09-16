using Deppo.Core.BaseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models;

public class CustomerTransaction : BaseTransaction
{
    private int _customerReferenceId;
    private string _customerCode = string.Empty;
    private string _customerName = string.Empty;

    public CustomerTransaction()
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

    public string CustomerCode
    {
        get => _customerCode;
        set
        {
            if (_customerCode == value) return;
            _customerCode = value;
            NotifyPropertyChanged();
        }
    }

    public string CustomerName
    {
        get => _customerName;
        set
        {
            if (_customerName == value) return;
            _customerName = value;
            NotifyPropertyChanged();
        }
    }
}