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
    private int _unitsetReferenceId;

    [ObservableProperty]
    private string _unitsetName = string.Empty;

    [ObservableProperty]
    private string _unitsetCode = string.Empty;

    [ObservableProperty]
    private int _subUnitsetReferenceId;

    [ObservableProperty]
    private string _subUnitsetName = string.Empty;

    [ObservableProperty]
    private string _subUnitsetCode = string.Empty;

    [ObservableProperty]
    private double _quantity;

    [ObservableProperty]
    private double _shippedQuantity;

    [ObservableProperty]
    private double _waitingQuantity;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private int locTracking;

    [ObservableProperty]
    private int trackingType;

    [ObservableProperty]
    private string image = string.Empty;

	public byte[] ImageData
	{
		get
		{
			if (string.IsNullOrEmpty(Image))
				return Array.Empty<byte>();
			else
				return Convert.FromBase64String(Image);
		}
	}

	[ObservableProperty]
    private double stockQuantity;

    [ObservableProperty]
    public List<WaitingPurchaseOrder> orders = new();

	public string LocTrackingIcon => "location-dot";

	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";


	public string VariantIcon => "bookmark";

	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";


	public string TrackingTypeIcon => "box-archive";

	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";
}