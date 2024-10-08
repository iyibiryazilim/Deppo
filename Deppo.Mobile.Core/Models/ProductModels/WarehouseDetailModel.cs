using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.ProductModels;

public partial class WarehouseDetailModel : ObservableObject
{
    [ObservableProperty]
    private Warehouse warehouse = null!;

    [ObservableProperty]
    private double inputQuantity;

    [ObservableProperty]
    private double outputQuantity;

    public ObservableCollection<WarehouseTransaction> LastTransactions { get; } = new();

    public ObservableCollection<WarehouseFiche> LastFiches { get; } = new();

    public ObservableCollection<WarehouseDetailProductReferenceModel> ProductReferences { get; } = new();
}