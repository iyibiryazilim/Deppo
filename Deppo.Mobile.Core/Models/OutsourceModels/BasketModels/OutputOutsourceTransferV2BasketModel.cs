using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;

public partial class OutputOutsourceTransferV2BasketModel :ObservableObject
{
    [ObservableProperty]
    WarehouseModel? outsourceWarehouseModel;

    [ObservableProperty]
    OutsourceModel? outsourceModel;

    [ObservableProperty]
    OutputOutsourceTransferV2ProductModel? outputOutsourceTransferMainProductModel;

    [ObservableProperty]
    ObservableCollection<OutputOutsourceTransferV2SubProductModel>? outputOutsourceTransferSubProducts = new();




}
