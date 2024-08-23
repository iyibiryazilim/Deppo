using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;

public partial class ProductProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public ProductProcessViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "İşlemler";

        ProductionInputCommand = new Command(async () => await ProductionInputAsync());
    }

    public Command ProductionInputCommand { get; }

    private async Task ProductionInputAsync()
    {
        
        await Shell.Current.GoToAsync($"{nameof(ProductionInputWarehouseListView)}");
    }


}
