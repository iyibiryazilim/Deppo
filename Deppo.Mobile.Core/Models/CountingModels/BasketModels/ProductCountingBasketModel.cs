using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.CountingModels.BasketModels;

public partial class ProductCountingBasketModel : ObservableObject
{
	[ObservableProperty]
	int productReferenceId;

	[ObservableProperty]
	string productCode = string.Empty;

	[ObservableProperty]
	string productName = string.Empty;

	[ObservableProperty]
	int subUnitsetReferenceId;

	[ObservableProperty]
	string subUnitsetName = string.Empty;

	[ObservableProperty]
	string subUnitsetCode = string.Empty;

	[ObservableProperty]
	int unitsetReferenceId;
	[ObservableProperty]
	string unitsetName = string.Empty;
	[ObservableProperty]
	string unitsetCode = string.Empty;

	[ObservableProperty]
	double stockQuantity;

	[ObservableProperty]
	double outputQuantity;

	[ObservableProperty]
	string? image;

	[ObservableProperty]
	bool isVariant;

	[ObservableProperty]
	int trackingType;

	[ObservableProperty]
	int locTracking;

	[ObservableProperty]
	ObservableCollection<LocationTransactionModel> locationTransactions = new();

	[ObservableProperty]
	double differenceQuantity = 0;

}
