using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;

public partial class QuicklyBomProductBasketModel : ObservableObject
{
    [ObservableProperty]
    public QuicklyBOMProductModel quicklyBomProduct = null!;

    [ObservableProperty]
    public double bOMQuantity =default;


    [ObservableProperty]
    public ObservableCollection<QuicklyBomSubProductModel> subProducts  = new();
}
