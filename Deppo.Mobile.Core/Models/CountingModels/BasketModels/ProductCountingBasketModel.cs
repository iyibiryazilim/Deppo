using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
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
    private int subUnitsetReferenceId;

    [ObservableProperty]
    private string subUnitsetName = string.Empty;

    [ObservableProperty]
    private string subUnitsetCode = string.Empty;

    [ObservableProperty]
    private int unitsetReferenceId;

    [ObservableProperty]
    private string unitsetName = string.Empty;

    [ObservableProperty]
    private string unitsetCode = string.Empty;

    [ObservableProperty]
    private double stockQuantity;

    [ObservableProperty]
    private double outputQuantity;

    [ObservableProperty]
    private byte[]? image;

    [ObservableProperty]
    private bool isVariant;
	[ObservableProperty]
	private string _variantIcon = "bookmark";
	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
    private int trackingType;
	[ObservableProperty]
	private string _trackingTypeIcon = "box-archive";
	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
    private int locTracking;
	[ObservableProperty]
	private string _locTrackingIcon = "location-dot";
	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
    private ObservableCollection<LocationTransactionModel> locationTransactions = new();

    [ObservableProperty]
    private ObservableCollection<SubUnitset> subUnitsets = new();

    [ObservableProperty]
    private double differenceQuantity = 0;

    [ObservableProperty]
    private double conversionFactor = 1;

    [ObservableProperty]
    private double otherConversionFactor = 1;
}