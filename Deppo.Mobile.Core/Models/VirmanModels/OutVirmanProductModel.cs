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


    private double _virmanQuantity = 0; 

    public double VirmanQuantity
    {
        get => _virmanQuantity;
        set
        {
            if (_virmanQuantity == value) return;
            _virmanQuantity = value;
            NotifyPropertyChanged();
        }
    }
}
