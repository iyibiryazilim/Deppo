using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesPanelModel :ObservableObject
{
    public SalesPanelModel()
    {
        
    }
    [ObservableProperty]
     int waitingOrderCount;
    [ObservableProperty]
     int amountTotal;
    [ObservableProperty]
     int shippedQuantityTotal;
     public ObservableCollection<Customer> LastCustomer { get; } = new();
     public ObservableCollection<CustomerTransaction> LastCustomerTransaction { get; }  = new();

    public ObservableCollection<SalesFiche> LastSalesFiche { get; } = new();

}
