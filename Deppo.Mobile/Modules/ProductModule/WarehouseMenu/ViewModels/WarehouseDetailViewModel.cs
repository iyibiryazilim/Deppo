using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;


[QueryProperty(name: nameof(WarehouseDetailModel), queryId: nameof(WarehouseDetailModel))]
public partial class WarehouseDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;
    public WarehouseDetailViewModel(IHttpClientService httpClientService, IWarehouseService warehouseService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
    {
        Title = "Ambar Detayı";
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

	[ObservableProperty]
	WarehouseDetailModel warehouseDetailModel = null!;

    #region Commands
    public Command LoadItemsCommand { get; }
    #endregion

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
        finally
        {
           IsBusy = false;
        }
    }
}
