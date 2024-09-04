using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public partial class PurchaseSupplierProduct : ObservableObject
{
    [ObservableProperty]
    int _referenceId;

    [ObservableProperty]
    int _itemReferenceId;

    [ObservableProperty]
    string _itemCode = string.Empty;

    [ObservableProperty]
    string _itemName = string.Empty;

    [ObservableProperty]
    int _mainItemReferenceId;

    [ObservableProperty]
    string _mainItemCode = string.Empty;

    [ObservableProperty]
    string _mainItemName = string.Empty;

    [ObservableProperty]
    bool _isVariant;

    [ObservableProperty]
    int _unitsetReferenceId;

    [ObservableProperty]
    string _unitsetName = string.Empty;

    [ObservableProperty]
    string _unitsetCode = string.Empty;

    [ObservableProperty]
    int _subUnitsetReferenceId;

    [ObservableProperty]
    string _subUnitsetName = string.Empty;

    [ObservableProperty]
    string _subUnitsetCode = string.Empty;

    [ObservableProperty]
    double _quantity;

    [ObservableProperty]
    double _shippedQuantity;

    [ObservableProperty]
    double _waitingQuantity;

    [ObservableProperty]
    bool isSelected;

    [ObservableProperty]
    int locTracking;

    [ObservableProperty]
    int trackingType;

    [ObservableProperty]
    string image = string.Empty;

    [ObservableProperty]
    double stockQuantity;

    [ObservableProperty]
    public List<WaitingPurchaseOrder> orders = new();
}