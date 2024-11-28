using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.CountingModels
{
    public partial class NegativeWarehouseModel : ObservableObject
    {
        [ObservableProperty]
        int warehouseReferenceId;

        [ObservableProperty]
        short warehouseNumber;

        [ObservableProperty]
        string warehouseName = string.Empty;

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
        bool _isVariant;

        [ObservableProperty]
        int _trackingType;

        [ObservableProperty]
        int _locTracking;

        [ObservableProperty]
        double stockQuantity;

        [ObservableProperty]
        bool isSelected;

    }
}
