using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.WarehouseModels;

namespace Deppo.Mobile.Core.Models.TransferModels;

public partial class TransferBasketModel : ObservableObject
{

    [ObservableProperty]
    WarehouseModel outWarehouse = null!;

    [ObservableProperty]
    ObservableCollection<OutProductModel> outProducts = new();

    [ObservableProperty]
    WarehouseModel inWarehouse = null!;

    [ObservableProperty]
    ObservableCollection<InProductModel> inProducts = new();
}
