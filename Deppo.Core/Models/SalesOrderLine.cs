using System;
using System.ComponentModel;
using Deppo.Core.BaseModels;

namespace Deppo.Core.Models;

/// <summary>
/// Satış Sipariş Satırı - LG_ORFLINE
/// </summary>
public class SalesOrderLine : OrderLine
{
    private int _salesOrderReferenceId;
    private SalesOrder? _salesOrder;

    public SalesOrderLine()
    {

    }

    [Browsable(false)]
    public int SalesOrderReferenceId
    {
        get => _salesOrderReferenceId;
        set
        {
            if (_salesOrderReferenceId == value) return;
            _salesOrderReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public SalesOrder? SalesOrder
    {
        get => _salesOrder;
        set
        {
            if (_salesOrder == value) return;
            _salesOrder = value;
            NotifyPropertyChanged();
        }
    }

}
