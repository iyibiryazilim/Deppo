using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesCustomerProduct : ObservableObject
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
	double stockQuantity;

	[ObservableProperty]
	public List<WaitingSalesOrder> orders = new();


	[ObservableProperty]
	string locTrackingIcon = "location-dot";

	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	string variantIcon = "bookmark";

	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	string trackingTypeIcon = "box-archive";

	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";
}
