using System;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public class PurchaseSupplierProduct
{
    private int _referenceId;
    private int _itemReferenceId;
    private string _itemCode = string.Empty;
    private string _itemName = string.Empty;
    private int _mainItemReferenceId;
    private string _mainItemCode = string.Empty;
    private string _mainItemName = string.Empty;
    private bool _isVariant;
    private double _orderQuantity;
    private double _shippedQuantity;
    private double _waitingQuantity;
    private List<WaitingPurchaseOrder> _orders;

    public PurchaseSupplierProduct()
    {
        //Orders = new List<PurchaseSupplierProductOrder>();
    }
}
