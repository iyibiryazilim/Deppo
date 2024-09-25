using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.CountingModels.BasketModels;

public partial class WarehouseCountingBasketModel : ObservableObject
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
	bool isCompleted;

	public string CountingText => IsCompleted ? "Sayıldı" : "Bekliyor";

   public string CountingTextColor => IsCompleted ? "#008000" : "#E6BE0C";


}
