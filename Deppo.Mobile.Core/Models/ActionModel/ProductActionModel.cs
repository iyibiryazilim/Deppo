using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ActionModel;

public partial class ProductActionModel : ObservableObject
{
    [ObservableProperty]
    string actionName = string.Empty;

    [ObservableProperty]
    string actionUrl = string.Empty;

    [ObservableProperty]
    bool isSelected = false;
}
