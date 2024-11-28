using CommunityToolkit.Mvvm.ComponentModel;
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
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private BOMSubProductModel productModel = null!;

        [ObservableProperty]
        private double subBOMQuantity = default;

        [ObservableProperty]
        private double subOutputQuantity = default;

        [ObservableProperty]
        private double subAmount = default;

        [ObservableProperty]
        private ObservableCollection<GroupLocationTransactionModel> locationTransactions = new();

        [ObservableProperty]
        private string image = string.Empty;

        public byte[] ImageData
        {
            get
            {
                if (string.IsNullOrEmpty(Image))
                    return Array.Empty<byte>();
                else
                {
                    return Convert.FromBase64String(Image);
                }
            }
        }
    }
}