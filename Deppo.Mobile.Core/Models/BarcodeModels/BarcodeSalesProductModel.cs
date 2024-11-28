using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.BarcodeModels;

public partial class BarcodeSalesProductModel : ObservableObject
{
	[ObservableProperty]
	private int itemReferenceId;

	[ObservableProperty]
	private string itemCode = string.Empty;

	[ObservableProperty]
	private string itemName = string.Empty;

	[ObservableProperty]
	private int mainItemReferenceId;

	[ObservableProperty]
	private string mainItemCode = string.Empty;

	[ObservableProperty]
	private string mainItemName = string.Empty;

	[ObservableProperty]
	private bool isVariant;

	[ObservableProperty]
	private int unitsetReferenceId;

	[ObservableProperty]
	private string unitsetName = string.Empty;

	[ObservableProperty]
	private string unitsetCode = string.Empty;

	[ObservableProperty]
	private int subUnitsetReferenceId;

	[ObservableProperty]
	private string subUnitsetName = string.Empty;

	[ObservableProperty]
	private string subUnitsetCode = string.Empty;

	[ObservableProperty]
	private double quantity;

	[ObservableProperty]
	private double shippedQuantity;

	[ObservableProperty]
	private double waitingQuantity;

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

	public string LocTrackingIcon => "location-dot";

	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";


	public string VariantIcon => "bookmark";

	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";


	public string TrackingTypeIcon => "box-archive";

	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";
}
