using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.ProductModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.ActionModels.ProductActionModels;

public partial class ProductDetailActionModel : ObservableObject
{
    [ObservableProperty]
    string actionName = string.Empty;

    [ObservableProperty]
    string actionUrl = string.Empty;

    [ObservableProperty]
    int lineNumber = 0;

    [ObservableProperty]
    string icon = string.Empty;

    [ObservableProperty]
    bool isSelected = false;

    public ProductDetailActionModel()
    {

    }

}
