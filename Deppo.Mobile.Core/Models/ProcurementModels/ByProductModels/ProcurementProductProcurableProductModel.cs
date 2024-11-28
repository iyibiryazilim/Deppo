using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;

/// <summary>
/// Toplanabilir Ürün Listesinde kullanılan model
/// </summary>
public partial class ProcurementProductProcurableProductModel : ObservableObject
{
	[ObservableProperty]
	int referenceId;

	[ObservableProperty]
	int itemReferenceId;

	[ObservableProperty]
	string itemCode = string.Empty;

	[ObservableProperty]
	string itemName = string.Empty;

	[ObservableProperty]
	int mainItemReferenceId;

	[ObservableProperty]
	string mainItemCode = string.Empty;

	[ObservableProperty]
	string mainItemName = string.Empty;

	[ObservableProperty]
	bool isVariant;

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
	double quantity;

	[ObservableProperty]
	double shippedQuantity;

	[ObservableProperty]
	double waitingQuantity;

	[ObservableProperty]
	bool isSelected;

	[ObservableProperty]
	int locTracking;

	[ObservableProperty]
	int trackingType;

	[ObservableProperty]
	string image = string.Empty;

	[ObservableProperty]
	int locationReferenceId;

	[ObservableProperty]
	string locationCode = string.Empty;

	[ObservableProperty]
	string locationName = string.Empty;

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
	string locTrackingIcon = "location-dot";

	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	string variantIcon = "bookmark";

	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	string trackingTypeIcon = "box-archive";

	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

	public string ProcurementText => StockQuantity >= WaitingQuantity ? "circle-check" : StockQuantity > 0 ? "triangle-exclamation" : "triangle-exclamation";
	public string ProcurementTextColor => StockQuantity >= WaitingQuantity ? "Green" : StockQuantity > 0 ? "Gold" : "Red";
	public string ProcurementToolTipText => StockQuantity >= WaitingQuantity ? "Stokta yeterli miktar var." : StockQuantity > 0 ? "Stokta kısmi miktar var." : "Stokta hiç miktar yok.";
}
