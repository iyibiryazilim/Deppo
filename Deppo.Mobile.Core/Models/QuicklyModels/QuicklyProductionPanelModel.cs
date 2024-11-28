using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.ProductModels;

namespace Deppo.Mobile.Core.Models.QuicklyModels;

public partial class QuicklyProductionPanelModel : ObservableObject
{
[ObservableProperty]
    double inProductCount;

    [ObservableProperty]
    double outProductCount;

    [ObservableProperty]
    double outProductCountTotalRate = default;

    [ObservableProperty]
    double inProductCountTotalRate = default;

    [ObservableProperty]
    ObservableCollection<ProductModel> lastProducts = new();

    [ObservableProperty]
    ObservableCollection<ProductionFiche> lastProductionFiche = new();

    [ObservableProperty]
    ObservableCollection<ProductionTransaction> lastProductionTransaction = new();
}
