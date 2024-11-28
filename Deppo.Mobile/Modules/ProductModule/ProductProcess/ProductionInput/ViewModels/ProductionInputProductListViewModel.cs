using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;

[QueryProperty(name:nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ProductionInputProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProductService _productService;
    private readonly IUserDialogs _userDialogs;


    [ObservableProperty]
    WarehouseModel warehouseModel = null!;
    public ProductionInputProductListViewModel(IHttpClientService httpClientService, IProductService productService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _productService = productService;
        _userDialogs = userDialogs;

        Title = ".. Ürün Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

    public Command LoadItemsCommand { get; set; }
    public Command<WarehouseTotalModel> ItemTappedCommand { get; set; }
    public Command LoadMoreItemsCommand { get; set; }
    public Command PerformSearchCommand { get; set; }
    public Command NextCommand { get; set; }

    public ObservableCollection<Product> Items { get; } = new();

    private async Task LoadItemsAsync()
    {
        if(IsBusy)
            return;

            try
            {
                IsBusy = true;

                _userDialogs.Loading("Loading Items...");
                Items.Clear();
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _productService.GetObjects(httpClient,  _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                        Items.Add(item);
                }
                else
                {
                    if(_userDialogs.IsHudShowing)
                        _userDialogs.Loading().Hide();

                    _userDialogs.Alert(result.Message, "Error", "OK");
                }
            }
            catch (System.Exception ex)
            {
                if(_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(ex.Message, "Error", "OK");
            }
            finally
            {
                _userDialogs.Loading().Hide();
                IsBusy = false;
            }
    }
}
