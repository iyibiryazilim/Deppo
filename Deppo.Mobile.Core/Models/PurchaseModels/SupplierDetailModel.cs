using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
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
    private double inputQuantity;

    [ObservableProperty]
    private double outputQuantity;

    public SupplierDetailModel()
    {
    }

    public ObservableCollection<SupplierTransaction> LastTransactions { get; } = new();
	public ObservableCollection<SupplierTransaction> Transactions { get; } = new();
	public ObservableCollection<PurchaseFiche> LastFiches { get; } = new();
}