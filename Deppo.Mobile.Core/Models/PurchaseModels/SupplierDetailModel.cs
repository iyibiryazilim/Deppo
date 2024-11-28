using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public partial class SupplierDetailModel : ObservableObject
{
    [ObservableProperty]
    private Supplier supplier = null!;

    [ObservableProperty]
    private double inputQuantity; // Satınalınan ve Satış iade Yapılan Malzeme Referans sayısı

    [ObservableProperty]
    private double outputQuantity; // Satın yapılan ve Satınalma iade Yapılan Malzeme Referans sayısı

    [ObservableProperty]
    private double waitingProductReferenceCount;

    public SupplierDetailModel()
    {
    }

    public ObservableCollection<SupplierTransaction> Transactions { get; } = new();
    public ObservableCollection<PurchaseFiche> LastFiches { get; } = new();

    public ObservableCollection<SupplierDetailInputOutputModel> SupplierDetailInputOutputModels { get; } = new();
}