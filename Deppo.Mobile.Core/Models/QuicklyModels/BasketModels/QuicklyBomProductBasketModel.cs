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
     QuicklyBOMProductModel quicklyBomProduct = null!;

    [ObservableProperty]
    int warehouseNumber = default;

    [ObservableProperty]
    string warehouseName= string.Empty;

    [ObservableProperty]
     double bOMQuantity =default;


    [ObservableProperty]
    ObservableCollection<QuicklyBomSubProductModel> subProducts  = new();
}
