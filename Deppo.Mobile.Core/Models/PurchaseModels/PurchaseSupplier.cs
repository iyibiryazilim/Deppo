using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.SalesModels;
using System;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public partial class PurchaseSupplier : ObservableObject
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
    public List<PurchaseSupplierProduct> products;

    [ObservableProperty]
    private bool isSelected;
}