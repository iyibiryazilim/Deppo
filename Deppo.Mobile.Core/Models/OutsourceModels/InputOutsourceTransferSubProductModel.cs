using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels;

public partial class InputOutsourceTransferSubProductModel : ObservableObject
{
	[ObservableProperty]
	int referenceId;

	[ObservableProperty]
	int productReferenceId;

	[ObservableProperty]
	string productCode = string.Empty;

	[ObservableProperty]
	string productName = string.Empty;

	[ObservableProperty]
	int warehouseReferenceId;

	[ObservableProperty]
	int warehouseNumber;

	[ObservableProperty]
	string warehouseName = string.Empty;

	[ObservableProperty]
	int unitsetReferenceId;

	[ObservableProperty]
	string unitsetCode = string.Empty;

	[ObservableProperty]
	string unitsetName = string.Empty;

	[ObservableProperty]
	int subUnitsetReferenceId;

	[ObservableProperty]
	string subUnitsetCode = string.Empty;

	[ObservableProperty]
	string subUnitsetName = string.Empty;

	[ObservableProperty]
	double stockQuantity;

	[ObservableProperty]
	double quotientQuantity;  // Katsayi miktari

	[ObservableProperty]
	string image = string.Empty;

	public byte[] ImageData
	{
		get
		{
			if (string.IsNullOrEmpty(Image))
				return Array.Empty<byte>();
			else
			{
				return Convert.FromBase64String(Image);
			}
		}
	}

	[ObservableProperty]
	bool isVariant;

	[ObservableProperty]
	int trackingType;

	[ObservableProperty]
	int locTracking;

	[ObservableProperty]
	bool isSelected = false;

	public string LocTrackingIcon => "location-dot";
	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

	public string VariantIcon => "bookmark";
	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

	public string TrackingTypeIcon => "box-archive";
	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	double inputQuantity;

	public List<InputOutsourceTransferSubProductDetailModel> Details { get; set; } = new();
}
