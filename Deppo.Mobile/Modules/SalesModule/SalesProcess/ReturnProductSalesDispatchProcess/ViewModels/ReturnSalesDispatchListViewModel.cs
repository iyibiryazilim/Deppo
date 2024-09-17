using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;
//fiş
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class ReturnSalesDispatchListViewModel : BaseViewModel
{

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    SalesCustomer salesCustomer = null!;

    public ReturnSalesDispatchListViewModel()
    {
    }
}