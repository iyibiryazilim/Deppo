using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.ProductModels;

namespace Deppo.Mobile.Core.Models.CountingModels;

public partial class CountingPanelModel : ObservableObject
{
    [ObservableProperty]
    int totalProductCount;

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
    ObservableCollection<CountingFiche> lastCountingFiche = new();

    [ObservableProperty]
    ObservableCollection<CountingTransaction> lastCountingTransaction = new();
}
