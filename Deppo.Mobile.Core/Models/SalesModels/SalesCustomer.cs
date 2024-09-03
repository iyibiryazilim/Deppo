using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesCustomer : ObservableObject
{
    [ObservableProperty]
    private int _referenceId;

    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _productReferenceCount;

    [ObservableProperty]
    private List<SalesCustomerProduct> _products;

    public SalesCustomer()
    {
        //Products = new List<PurchaseSupplierProduct>();
    }


}
