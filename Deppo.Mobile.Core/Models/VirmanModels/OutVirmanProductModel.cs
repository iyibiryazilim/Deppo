using System;
using System.Collections.ObjectModel;
using Android.Database;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SeriLotModels;

namespace Deppo.Mobile.Core.Models.VirmanModels;

public class OutVirmanProductModel : ProductModel
{
    public ObservableCollection<LocationTransactionModel> LocationTransactionModels { get; set; } = new ObservableCollection<LocationTransactionModel>();
}