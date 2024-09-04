using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public partial class PurchaseSupplierProduct : ObservableObject
{
    [ObservableProperty]
    private int _referenceId;

    [ObservableProperty]
    private int _itemReferenceId;

    [ObservableProperty]
    private string _itemCode = string.Empty;

    [ObservableProperty]
    private string _itemName = string.Empty;

    [ObservableProperty]
    private int _mainItemReferenceId;

    [ObservableProperty]
    private string _mainItemCode = string.Empty;

    [ObservableProperty]
    private string _mainItemName = string.Empty;

    [ObservableProperty]
    private bool _isVariant;

    [ObservableProperty]
    private double _quantity;

    [ObservableProperty]
    private double _shippedQuantity;

    [ObservableProperty]
    private double _waitingQuantity;

    [ObservableProperty]
    public List<WaitingPurchaseOrder> orders = new();
}