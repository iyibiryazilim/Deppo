using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.LocationModels
{
    public partial class GroupLocationTransactionModel : ObservableObject
    {
        [ObservableProperty]
        Guid referenceId;

        [ObservableProperty]
        int serilotReferenceId;

        [ObservableProperty]
        string serilotCode = string.Empty;

        [ObservableProperty]
        string serilotName = string.Empty;

        [ObservableProperty]
        int locationReferenceId;

        [ObservableProperty]
        string locationCode = string.Empty;

        [ObservableProperty]
        string locationName = string.Empty;

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
        int itemReferenceId;

        [ObservableProperty]
        string itemCode = string.Empty;

        [ObservableProperty]
        string itemName = string.Empty;

        [ObservableProperty]
        double remainingQuantity;

        [ObservableProperty]
        double outputQuantity;

        [ObservableProperty]
        bool isSelected;


    }
}
