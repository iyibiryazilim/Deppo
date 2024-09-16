using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ReturnSalesProductListViewModel : BaseViewModel
{

    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseTotalService _warehouseTotalService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private WarehouseModel warehouseModel= null!;
    public ObservableCollection<WarehouseTotalModel> Items { get; } = new();

    public ReturnSalesProductListViewModel(
        IHttpClientService httpClientService,
        IProductService productService,
        IWarehouseTotalService warehouseTotalService,
    IUserDialogs userDialogs,
        IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _warehouseTotalService = warehouseTotalService;
        Title = "Ürün Listesi";


        BackCommand = new Command(async () => await BackAsync());

    }
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Items.Clear();
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _warehouseTotalService.GetObjects(httpClient, _httpClientService.FirmNumber , _httpClientService.PeriodNumber ,WarehouseModel.Number ,"",0,20 );
            if(result.IsSuccess)
            {
                if(result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<WarehouseTotalModel>(item));
                    }
                }
            }
            _userDialogs.HideHud();
           
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _warehouseTotalService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, "", Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<WarehouseTotalModel>(item));
                    }
                }
            }

        }

        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");

        }
        finally
        {
            IsBusy = false;
        }
    }



    private async Task BackAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"..");
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
}