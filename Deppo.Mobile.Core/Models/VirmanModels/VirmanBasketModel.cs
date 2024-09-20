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
    WarehouseModel inVirmanWarehouse = null!;

    [ObservableProperty]
    InVirmanProductModel inVirmanProduct = null!;

    

}
