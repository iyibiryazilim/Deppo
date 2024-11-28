using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesCustomerProduct : ObservableObject
{
    [ObservableProperty]
    private int referenceId;

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
    private string unitsetCode = string.Empty;

    [ObservableProperty]
    private string unitsetName = string.Empty;

    [ObservableProperty]
    private int subUnitsetReferenceId;

    [ObservableProperty]
    private string subUnitsetCode = string.Empty;

    [ObservableProperty]
    private string subUnitsetName = string.Empty;

    [ObservableProperty]
    private double quantity;

    [ObservableProperty]
    private double shippedQuantity;

    [ObservableProperty]
    private double waitingQuantity;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private int locTracking;

    [ObservableProperty]
    private int trackingType;

    [ObservableProperty]
    private double stockQuantity;

    [ObservableProperty]
    public List<WaitingSalesOrder> orders = new();

    public string LocTrackingIcon => "location-dot";

    public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

   
    public string VariantIcon => "bookmark";

    public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

    
    public string TrackingTypeIcon => "box-archive";

    public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

    [ObservableProperty]
    private string _image = string.Empty;

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
}