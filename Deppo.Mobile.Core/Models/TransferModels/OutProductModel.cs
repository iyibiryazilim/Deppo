using System;
using System.Collections.ObjectModel;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SeriLotModels;

namespace Deppo.Mobile.Core.Models.TransferModels;

public class OutProductModel : ProductModel
{
    public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; set; } = new ObservableCollection<SeriLotTransactionModel>();
}
