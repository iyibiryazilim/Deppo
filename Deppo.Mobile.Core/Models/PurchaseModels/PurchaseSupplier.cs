using System;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public class PurchaseSupplier
{
    private int _referenceId;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private int _productReferenceCount;
    private List<PurchaseSupplierProduct> _products;

    public PurchaseSupplier()
    {
        //Products = new List<PurchaseSupplierProduct>();
    }
}
