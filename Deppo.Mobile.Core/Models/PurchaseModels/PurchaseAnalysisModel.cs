using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.AnalysisModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public partial class PurchaseAnalysisModel : ObservableObject
{
    [ObservableProperty]
    int dueDatePassedSuppliersCount;

    [ObservableProperty]
    int dueDatePassedProductsCount;

    [ObservableProperty]
    int returnProductReferenceCount;

    [ObservableProperty]
    int purchaseProductReferenceCount;

    public ObservableCollection<Supplier> LastSupplier { get; } = new();
    public ObservableCollection<Product> LastProduct { get; } = new();
    public ObservableCollection<PurchaseProductReferenceAnalysis> PurchaseProductReferenceAnalysis { get; } = new();

}
