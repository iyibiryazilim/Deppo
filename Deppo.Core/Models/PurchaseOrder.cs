using System;
using System.ComponentModel;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

/// <summary>
/// Satınalma Siparişi - LG_ORFICHE
/// </summary>
public class PurchaseOrder : Order
{
    private int _supplierReferenceId;
    private Supplier? _supplier;

    public PurchaseOrder()
    {

    }

    [Browsable(false)]
    public int SupplierReferenceId
    {
        get => _supplierReferenceId;
        set
        {
            if (_supplierReferenceId == value) return;
            _supplierReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public Supplier? Supplier
    {
        get => _supplier;
        set
        {
            if (_supplier == value) return;
            _supplier = value;
            NotifyPropertyChanged();
        }
    }

}
