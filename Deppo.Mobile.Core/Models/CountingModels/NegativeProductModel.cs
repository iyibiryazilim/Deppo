using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.CountingModels;

public partial class NegativeProductModel : ObservableObject
{
    [ObservableProperty]
    private int productReferenceId;

    [ObservableProperty]
    private string productCode = string.Empty;

    [ObservableProperty]
    private string productName = string.Empty;

    [ObservableProperty]
    private int unitsetReferenceId;

    [ObservableProperty]
    private string unitsetName = string.Empty;

    [ObservableProperty]
    private string unitsetCode = string.Empty;

    [ObservableProperty]
    private int subUnitsetReferenceId;

    [ObservableProperty]
    private string subUnitsetName = string.Empty;

    [ObservableProperty]
    private string subUnitsetCode = string.Empty;

    [ObservableProperty]
    private bool isVariant;

    [ObservableProperty]
    private int trackingType;

    [ObservableProperty]
    private int locTracking;

    [ObservableProperty]
    private double stockQuantity;

    [ObservableProperty]
    private string locTrackingIcon = "location-dot";

    public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

    [ObservableProperty]
    private string variantIcon = "bookmark";

    public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

    [ObservableProperty]
    private string trackingTypeIcon = "box-archive";

    public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private ObservableCollection<NegativeWarehouseModel> negativeWarehouses = new();

    // Image property with direct notification of ImageData change
    private string? _image;

    public string? Image
    {
        get => _image;
        set
        {
            if (_image == value) return;
            _image = value;
            OnPropertyChanged(); // Notify that Image has changed
            OnPropertyChanged(nameof(ImageData)); // Notify that ImageData has also changed
        }
    }

    // ImageData property
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