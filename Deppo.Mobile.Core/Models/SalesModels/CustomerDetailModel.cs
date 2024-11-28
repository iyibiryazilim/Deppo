using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.PurchaseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class CustomerDetailModel : ObservableObject
{
    [ObservableProperty]
    private Customer customer = null!;

    [ObservableProperty]
    private double inputQuantity;

    [ObservableProperty]
    private double outputQuantity;

    [ObservableProperty]
    double waitingProductReferenceCount;

    public CustomerDetailModel()
    {
    }

    public ObservableCollection<CustomerTransaction> LastTransactions { get; } = new();
    public ObservableCollection<SalesFiche> LastFiches { get; } = new();

	public ObservableCollection<CustomerDetailInputOutputModel> CustomerDetailInputOutputModels { get; } = new();
}