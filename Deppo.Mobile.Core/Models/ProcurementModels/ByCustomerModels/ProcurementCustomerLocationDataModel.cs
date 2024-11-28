using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels
{
	public partial class ProcurementCustomerLocationDataModel : ObservableObject
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
		int brandReferenceId;

		[ObservableProperty]
		string brandName = string.Empty;

		[ObservableProperty]
		string brandCode = string.Empty;

		[ObservableProperty]
		double procurementQuantity;

		[ObservableProperty]
		int locTracking;

		[ObservableProperty]
		int trackingType;

		[ObservableProperty]
		string image = string.Empty;

		[ObservableProperty]
		bool _isSelected;

		[ObservableProperty]
		int _locationReferenceId;

		[ObservableProperty]
		string _locationCode = string.Empty;

		[ObservableProperty]
		string _locationName = string.Empty;

		[ObservableProperty]
		double _orderQuantity;

		[ObservableProperty]
		double _stockQuantity;

		[ObservableProperty]
		double _quantity;

		[ObservableProperty]
		int warehouseNumber;

		[ObservableProperty]
		string warehouseName = string.Empty;

	}
}
