using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;

namespace Deppo.Mobile.Core.Models.VirmanModels;

public  class InVirmanProductModel : ProductModel
{
    public ObservableCollection<LocationModel> Locations { get; set; } = new ObservableCollection<LocationModel>();
    
 

}
