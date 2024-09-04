using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.SalesModels;
using System;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public partial class PurchaseSupplier : ObservableObject
{
    [ObservableProperty]
    public int _referenceId;

    [ObservableProperty]
    public string _code = string.Empty;

    [ObservableProperty]
    public string _name = string.Empty;

    [ObservableProperty]
    public int _productReferenceCount;

    public string TitleName => Name?.Length > 2 ? Name.Substring(0, 2) : Name;

    [ObservableProperty]
    public List<PurchaseSupplierProduct> products;

    [ObservableProperty]
    public bool isSelected;
}