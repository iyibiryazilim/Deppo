using System;
using System.Collections.ObjectModel;
using Android.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.ProductModels;

public partial class ProductDetailModel : ObservableObject
{
    [ObservableProperty]
    private Product product = null!;

    [ObservableProperty]
    private double inputQuantity;

    [ObservableProperty]
    private double outputQuantity;

    public ProductDetailModel()
    {
    }


    public ObservableCollection<ProductTransaction> LastTransactions { get; } = new();
}
