﻿using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.QuicklyModels
{
    public partial class QuicklyBomSubProductModel : ObservableObject
    {

        [ObservableProperty]
         WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        BOMSubProductModel productModel = null!;

        [ObservableProperty]
         double subBOMQuantity = default;

        [ObservableProperty]
        double subOutputQuantity = default;

        [ObservableProperty]
        double subAmount = default;

        [ObservableProperty]
        ObservableCollection<GroupLocationTransactionModel> locationTransactions = new();
    }
}
