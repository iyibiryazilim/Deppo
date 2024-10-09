using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.WarehouseModels
{
    public partial class WarehouseParameterModel : ObservableObject
    {
        [ObservableProperty]
        double _minLevel;

        [ObservableProperty]
        double _maxLevel;

        [ObservableProperty]
        double _safeLevel;

    }
}
