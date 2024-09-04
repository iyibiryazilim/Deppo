using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesCustomer : ObservableObject
{
    [ObservableProperty]
    int referenceId;

    [ObservableProperty]
    string code = string.Empty;

    [ObservableProperty]
    string name = string.Empty;

    [ObservableProperty]
    int productReferenceCount;

    [ObservableProperty]
    public List<SalesCustomerProduct> products;

    [ObservableProperty]
    bool isSelected;

    public SalesCustomer()
    {
        //Products = new List<PurchaseSupplierProduct>();
    }


}
