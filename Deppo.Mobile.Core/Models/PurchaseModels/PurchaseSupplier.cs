using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.SalesModels;
using System;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public partial class PurchaseSupplier : ObservableObject
{
    [ObservableProperty]
    int _referenceId;

    [ObservableProperty]
    string _code = string.Empty;

    [ObservableProperty]
    string _name = string.Empty;

    [ObservableProperty]
    int _productReferenceCount;

    [ObservableProperty]
    string country = string.Empty;

    [ObservableProperty]
    string city = string.Empty;

    public string TitleName => Name?.Length > 2 ? Name.Substring(0, 2) : Name;

    [ObservableProperty]
    List<PurchaseSupplierProduct> products = new();

    [ObservableProperty]
    bool isSelected;
}