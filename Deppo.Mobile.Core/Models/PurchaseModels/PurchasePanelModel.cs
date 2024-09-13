using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels
{
    public partial class PurchasePanelModel : ObservableObject
    {
        public PurchasePanelModel()
        {

        }
        [ObservableProperty]
        int waitingOrderCount;
        [ObservableProperty]
        int amountTotal;
        [ObservableProperty]
        int shippedQuantityTotal;
        public ObservableCollection<Supplier> LastSuplier { get; } = new();
        public ObservableCollection<SupplierTransaction> LastSupplierTransaction { get; } = new();
        public ObservableCollection<PurchaseFiche> LastPurchaseFiche { get; } = new();
    }
}
