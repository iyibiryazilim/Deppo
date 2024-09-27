using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.OutsourceModels;

public partial class OutsourcePanelModel : ObservableObject
{

    [ObservableProperty]
    int totalProductCount;

    [ObservableProperty]
    double inProductCount;

    [ObservableProperty]
    double outProductCount;

    [ObservableProperty]
    ObservableCollection<OutsourceModel> outsources = new();

    [ObservableProperty]
    ObservableCollection<OutsourceFiche> lastOutsourceFiche = new();

    [ObservableProperty]
    ObservableCollection<SupplierTransaction> lastSupplierTransaction = new();

}
