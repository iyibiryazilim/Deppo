using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.CountingModels;

public partial class NegativeProductModel : ObservableObject
{

    [ObservableProperty]
    int productReferenceId;

    [ObservableProperty]
    string productCode = string.Empty;

    [ObservableProperty]
    string productName = string.Empty;

    [ObservableProperty]
    int unitsetReferenceId;

    [ObservableProperty]
    string unitsetName = string.Empty;

    [ObservableProperty]
    string unitsetCode = string.Empty;

    [ObservableProperty]
     int subUnitsetReferenceId;

    [ObservableProperty]
    string subUnitsetName = string.Empty;

    [ObservableProperty]
    string subUnitsetCode = string.Empty;

    [ObservableProperty]
    bool isVariant;

    [ObservableProperty]
    int trackingType;

    [ObservableProperty]
    int locTracking;

    [ObservableProperty]
    double stockQuantity;

	[ObservableProperty]
	string locTrackingIcon = "location-dot";

	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	string variantIcon = "bookmark";

	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	string trackingTypeIcon = "box-archive";

	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

    [ObservableProperty]
    bool isSelected;

    [ObservableProperty]
    ObservableCollection<NegativeWarehouseModel> negativeWarehouses = new();

}
