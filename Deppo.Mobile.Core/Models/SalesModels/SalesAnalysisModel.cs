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

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesAnalysisModel : ObservableObject
{

    [ObservableProperty]
    int dueDatePassedCustomersCount;

    [ObservableProperty]
    int dueDatePassedProductsCount;

    [ObservableProperty]
    int returnProductReferenceCount;

    [ObservableProperty]
    int soldProductReferenceCount;




    public ObservableCollection<Customer> LastCustomer { get; } = new();
    public ObservableCollection<Product> LastProduct { get;} = new();
    public ObservableCollection<SalesProductReferenceAnalysis> SalesProductReferenceAnalysis { get; set;} = new();


}
