using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

[QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
public partial class ManuelCalcInWarehouseListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly IUserDialogs _userDialogs;


    [ObservableProperty]
    QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;

    [ObservableProperty]
    WarehouseModel selectedWarehouseModel = null!;

    public ObservableCollection<WarehouseModel> Items { get; } = new();

    public ManuelCalcInWarehouseListViewModel(IHttpClientService httpClientService,
    IWarehouseService warehouseService,
    IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;

        Title = "Giriş Ambarı Seçiniz..";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());
    }

    public Command BackCommand { get; }
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            Items.Clear();
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, 0, 20, _httpClientService.FirmNumber);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(new WarehouseModel
                        {
                            ReferenceId = item.ReferenceId,
                            Name = item.Name,
                            Number = item.Number,
                            City = item.City,
                            Country = item.Country,
                            IsSelected = false
                        });
                }
            }

            if(_userDialogs.IsHudShowing) 
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMoreItemsAsync()
    {
        if (Items.Count < 18)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, Items.Count, 20, _httpClientService.FirmNumber);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
					_userDialogs.ShowLoading("Loading...");
					foreach (var item in result.Data)
                        Items.Add(new WarehouseModel
                        {
                            ReferenceId = item.ReferenceId,
                            Name = item.Name,
                            Number = item.Number,
                            City = item.City,
                            Country = item.Country,
                            IsSelected = false
                        });
                }
            }

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ItemTappedAsync(WarehouseModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item == SelectedWarehouseModel)
            {
                SelectedWarehouseModel.IsSelected = false;
                SelectedWarehouseModel = null;
            }
            else
            {
                if (SelectedWarehouseModel != null)
                {
                    SelectedWarehouseModel.IsSelected = false;
                }

                SelectedWarehouseModel = item;
                SelectedWarehouseModel.IsSelected = true;
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SelectedWarehouseModel is not null)
            {
                QuicklyBomProductBasketModel.WarehouseName = SelectedWarehouseModel.Name;
                QuicklyBomProductBasketModel.WarehouseNumber = SelectedWarehouseModel.Number;

                QuicklyBomProductBasketModel.QuicklyBomProduct.WarehouseName = SelectedWarehouseModel.Name;
                QuicklyBomProductBasketModel.QuicklyBomProduct.WarehouseNumber = SelectedWarehouseModel.Number;


                await Shell.Current.GoToAsync($"{nameof(ManuelCalcView)}", new Dictionary<string, object>
                {
                    [nameof(QuicklyBomProductBasketModel)] = QuicklyBomProductBasketModel
                });
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
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

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}