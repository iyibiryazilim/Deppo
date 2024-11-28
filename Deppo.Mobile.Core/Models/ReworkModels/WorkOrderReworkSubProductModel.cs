using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.ReworkModels;

public partial class WorkOrderReworkSubProductModel : ObservableObject
{
    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private BOMSubProductModel productModel = null!;

    [ObservableProperty]
    private double subBOMQuantity = default;

    [ObservableProperty]
    private double subAmount = default;

    [ObservableProperty]
    private ObservableCollection<LocationModel> locations = new();

    [ObservableProperty]
    private string image = string.Empty;

    public byte[] ImageData
    {
        get
        {
            if (string.IsNullOrEmpty(Image))
                return Array.Empty<byte>();
            else
            {
                return Convert.FromBase64String(Image);
            }
        }
    }
}