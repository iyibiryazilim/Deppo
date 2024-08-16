using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(ProductDetailModel), queryId: nameof(ProductDetailModel))]
public partial class ProductDetailViewModel : BaseViewModel
{
    [ObservableProperty]
    private ProductDetailModel productDetailModel = null!;
}