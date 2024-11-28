using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.WarehouseModels;

namespace Deppo.Mobile.Core.Models.VirmanModels;

public partial class VirmanBasketModel : ObservableObject
{
    [ObservableProperty]
    WarehouseModel outVirmanWarehouse = null!;

    [ObservableProperty]
    OutVirmanProductModel outVirmanProduct = null!;

    [ObservableProperty]
    double outVirmanQuantity = 0;

    [ObservableProperty]
    WarehouseModel inVirmanWarehouse = null!;

    [ObservableProperty]
    InVirmanProductModel inVirmanProduct = null!;

    [ObservableProperty]
    double inVirmanQuantity = 0;

    

}
