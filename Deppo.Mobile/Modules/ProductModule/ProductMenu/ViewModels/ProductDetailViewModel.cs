using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(Product), queryId: nameof(Product))]
public partial class ProductDetailViewModel : BaseViewModel
{

    [ObservableProperty]
    Product _product = null!;
}
