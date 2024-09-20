using System;
using System.Collections.ObjectModel;
using Android.Database;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SeriLotModels;

namespace Deppo.Mobile.Core.Models.VirmanModels;

public class OutVirmanProductModel : ProductModel
{
    public ObservableCollection<SeriLotTransactionModel> SeriLotTransactionModels { get; set; } = new ObservableCollection<SeriLotTransactionModel>();


   
}
