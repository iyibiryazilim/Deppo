using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels
{
    [QueryProperty(name: nameof(SupplierDetailModel), queryId: nameof(SupplierDetailModel))]
    public partial class SupplierDetailViewModel : BaseViewModel
    {
        [ObservableProperty]
        private SupplierDetailModel supplierDetailModel = null!;
        public SupplierDetailViewModel()
        {
            Title = "Tedarikçi Detayı";
        }
    }
}