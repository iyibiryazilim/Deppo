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
    private ProductModel productModel = null!;

    [ObservableProperty]
    private double inputQuantity;

    [ObservableProperty]
    private double outputQuantity;

    [ObservableProperty]
    private InventoryTurnover inventoryTurnover = new();

    public ProductDetailModel()
    {
    }


    public ObservableCollection<ProductTransaction> LastTransactions { get; } = new();

    public ObservableCollection<ProductFiche> Transactions { get; } = new();

    public ObservableCollection<ProductDetailInputOutputModel> ProductInputOutputModels { get; } = new();


}
