using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;

public partial class QuicklyBomProductBasketModel : ObservableObject
{
    [ObservableProperty]
    private QuicklyBOMProductModel quicklyBomProduct = null!;

    public ObservableCollection<LocationModel> MainLocations { get; set; } = new ObservableCollection<LocationModel>();

    [ObservableProperty]
    private int warehouseNumber = default;

    [ObservableProperty]
    private string warehouseName = string.Empty;

    [ObservableProperty]
    private double bOMQuantity = default;

    [ObservableProperty]
    private double mainAmount = default;

    [ObservableProperty]
    private ObservableCollection<QuicklyBomSubProductModel> subProducts = new();

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