using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

public partial class ProductionInputResultViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;

    public ProductionInputResultViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;

        Title = "Üretim Giriş Sonuçları";
    }

}
