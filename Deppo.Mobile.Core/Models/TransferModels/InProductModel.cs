using System;
using System.Collections.ObjectModel;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;

namespace Deppo.Mobile.Core.Models.TransferModels;

public class InProductModel : ProductModel
{
    public ObservableCollection<LocationModel> Locations { get; set; } = new ObservableCollection<LocationModel>();
}
