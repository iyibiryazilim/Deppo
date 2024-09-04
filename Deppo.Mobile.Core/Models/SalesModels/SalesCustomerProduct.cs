using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesCustomerProduct : ObservableObject
{
	//[ObservableProperty]
	//int _referenceId;

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
	public List<WaitingSalesOrder> orders = new();

}
