﻿using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

[QueryProperty(name: nameof(TransferBasketModel), queryId: nameof(TransferBasketModel))]
public partial class TransferInWarehouseViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly IUserDialogs _userDialogs;
    public TransferInWarehouseViewModel(IHttpClientService httpClientService, IWarehouseService warehouseService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;

        Title = "Giriş Ambarı Seçimi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());
    }

    #region Commands
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }

    public Command BackCommand { get; }
    #endregion

    #region Collections
    public ObservableCollection<WarehouseModel> Items { get; } = new();
    #endregion

    #region Properties

    [ObservableProperty]
    WarehouseModel selectedWarehouseModel = null!;
    #endregion

    [ObservableProperty]
    TransferBasketModel transferBasketModel = null!;

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
                    {
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
            }

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
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, Items.Count, 20, _httpClientService.FirmNumber);
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

                TransferBasketModel.InWarehouse = SelectedWarehouseModel;
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

            foreach (var item in TransferBasketModel.OutProducts)
            {
                TransferBasketModel.InProducts.Add(new InProductModel
                {
                    ReferenceId = item.ReferenceId,
                    ItemReferenceId = item.ItemReferenceId,
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    Image = item.Image,
                    MainItemReferenceId = item.MainItemReferenceId,
                    MainItemCode = item.MainItemCode,
                    MainItemName = item.MainItemName,
                    OutputQuantity = item.OutputQuantity,
                    IsVariant = item.IsVariant,
                    LocTracking = item.LocTracking,
                    StockQuantity = item.StockQuantity,
                    SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                    SubUnitsetCode = item.SubUnitsetCode,
                    SubUnitsetName = item.SubUnitsetName,
                    UnitsetReferenceId = item.UnitsetReferenceId,
                    UnitsetCode = item.UnitsetCode,
                    UnitsetName = item.UnitsetName,
                    LocTrackingIcon = item.LocTrackingIcon,
                    TrackingType = item.TrackingType,
                    TrackingTypeIcon = item.TrackingTypeIcon,
                    VariantIcon = item.VariantIcon,
                });

			}

            

            await Shell.Current.GoToAsync($"{nameof(TransferInBasketView)}", new Dictionary<string, object>
            {
                [nameof(TransferBasketModel)] = TransferBasketModel,
            });
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            
            if(TransferBasketModel.InWarehouse != null)
            {
                TransferBasketModel.InWarehouse.IsSelected = false;

                TransferBasketModel.InWarehouse = null;
            }

            await Shell.Current.GoToAsync("..");

        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
