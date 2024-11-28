using System;
using System.ComponentModel;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

/// <summary>
/// Satınalma Siparişi Satırı - LG_ORFLINE
/// </summary>
public class PurchaseOrderLine : OrderLine
{
    private int _purchaseOrderReferenceId;
    private PurchaseOrder? _purchaseOrder;

    public PurchaseOrderLine()
    {

    }

    [Browsable(false)]
    public int PurchaseOrderReferenceId
    {
        get => _purchaseOrderReferenceId;
        set
        {
            if (_purchaseOrderReferenceId == value) return;
            _purchaseOrderReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public PurchaseOrder? PurchaseOrder
    {
        get => _purchaseOrder;
        set
        {
            if (_purchaseOrder == value) return;
            _purchaseOrder = value;
            NotifyPropertyChanged();
        }
    }
}
