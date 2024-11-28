using System;
using System.ComponentModel;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

public class PurchaseTransaction : BaseTransaction
{
    private int _supplierReferenceId;
    private Supplier? _supplier;

    [Browsable(false)]
    public int SupplierReferenceId
    {
        get => _supplierReferenceId;
        set
        {
            if (_supplierReferenceId == value) return;
            _supplierReferenceId = value;
            NotifyPropertyChanged(nameof(SupplierReferenceId));
        }
    }

    public Supplier? Supplier
    {
        get => _supplier;
        set
        {
            if (_supplier == value) return;
            _supplier = value;
            NotifyPropertyChanged(nameof(Supplier));
        }
    }

}
