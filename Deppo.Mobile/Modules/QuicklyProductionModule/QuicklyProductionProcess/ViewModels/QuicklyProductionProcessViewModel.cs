using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ViewModels;

public partial class QuicklyProductionProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public QuicklyProductionProcessViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;

        Title = "Hızlı Üretim İşlemleri";

        QuicklyProductionManuelCommand = new Command(async () => await QuicklyProductionManuelAsync());
    }

    public Command QuicklyProductionManuelCommand { get; }

    public async Task QuicklyProductionManuelAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(ManuelProductListView)}", new Dictionary<string, object>());
    }
}