using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name: nameof(ProcurementCustomerBasketModel), queryId: nameof(ProcurementCustomerBasketModel))]
public partial class ProcurementByCustomerBasketViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementByCustomerBasketService _procurementByCustomerBasketService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    ProcurementCustomerBasketModel procurementCustomerBasketModel;

    [ObservableProperty]
    int currentPosition;

    [ObservableProperty]
    int totalPosition;

    public ProcurementByCustomerBasketViewModel(
        IHttpClientService httpClientService,
        IProcurementByCustomerBasketService procurementByCustomerBasketService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementByCustomerBasketService = procurementByCustomerBasketService;
        _userDialogs = userDialogs;

        Title = "Ürün Toplama Sepeti";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        NextPositionCommand = new Command(NextPositionAsync);
        PreviousPositionCommand = new Command(PreviousPositionAsync);
        ProcurementInfoCommand = new Command(async () => await ProcurementInfoAsync());

        IncreaseCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await DecreaseAsync(item));
		QuantityTappedCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await QuantityTappedAsync(item));
	}

    public Page CurrentPage { get; set; }
    public bool IsPreviousButtonVisible => CurrentPosition == 0 ? false : true;
    public bool IsNextButtonVisible => CurrentPosition == TotalPosition ? false : true;
    public bool IsCompleteButtonVisible => Items.Count > 0 && CurrentPosition == TotalPosition ? true : false;
    public bool IsPageIndicatorVisible => !IsCompleteButtonVisible;


    public ObservableCollection<ProcurementCustomerBasketModel> Items { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command NextPositionCommand { get; }
    public Command PreviousPositionCommand { get; }
    public Command ProcurementInfoCommand { get; }

    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command QuantityTappedCommand { get; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {

            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Items.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var referenceIds = ProcurementCustomerBasketModel.ProcurementProductList.Select(x => x.ItemReferenceId).ToArray();

            var result = await _procurementByCustomerBasketService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: ProcurementCustomerBasketModel.WarehouseNumber,
                itemsReferenceId: referenceIds);

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    var groupByLocation = result.Data
                        .OrderBy(x => x.LocationName)
                        .GroupBy(x => new
                        {
                            LocationReferenceId = x.LocationReferenceId,
                            LocationCode = x.LocationCode,
                            LocationName = x.LocationName,
                            WarehouseNumber = x.WarehouseNumber,
                            WarehouseName = x.WarehouseName
                        });

                    foreach (var group in groupByLocation)
                    {
                        var procurementCustomerBasketModel = new ProcurementCustomerBasketModel
                        {
                            LocationReferenceId = group.Key.LocationReferenceId,
                            LocationCode = group.Key.LocationCode,
                            LocationName = group.Key.LocationName,
                            WarehouseNumber = ProcurementCustomerBasketModel.WarehouseNumber,
                            WarehouseName = ProcurementCustomerBasketModel.WarehouseName,
                        };

                        foreach (var item in group)
                        {
                            var procurementCustomerBasketProductModel = new ProcurementCustomerBasketProductModel
                            {
                                ItemReferenceId = item.ItemReferenceId,
                                ItemCode = item.ItemCode,
                                ItemName = item.ItemName,
                                UnitsetReferenceId = item.UnitsetReferenceId,
                                UnitsetCode = item.UnitsetCode,
                                UnitsetName = item.UnitsetName,
                                SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                                SubUnitsetCode = item.SubUnitsetCode,
                                SubUnitsetName = item.SubUnitsetName,
                                IsVariant = item.IsVariant,
                                Quantity = 0,
                                StockQuantity = ProcurementCustomerBasketModel.ProcurementProductList.Sum(x => x.StockQuantity),
                                OrderQuantity = ProcurementCustomerBasketModel.ProcurementProductList.Sum(x => x.WaitingQuantity),
                                ProcurementQuantity = item.ProcurementQuantity,
                                IsSelected = false,
                                LocTracking = item.LocTracking,
                                TrackingType = item.TrackingType
                            };

                            procurementCustomerBasketModel.Products.Add(procurementCustomerBasketProductModel);

                        }

                        Items.Add(procurementCustomerBasketModel);

                    }

                }

                TotalPosition = Items.Count - 1;
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task IncreaseAsync(ProcurementCustomerBasketProductModel item)
    {
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
        {
            IsBusy = true;

			if (item.ProcurementQuantity > item.Quantity)
            {
                item.Quantity++;
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

    private async Task DecreaseAsync(ProcurementCustomerBasketProductModel item)
    {
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
        {
            if(item.Quantity > 0)
            {
				item.Quantity--;
			}
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

    private async Task QuantityTappedAsync(ProcurementCustomerBasketProductModel item)
    {
        if(item is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				initialValue: item.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity <= 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.ProcurementQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, ürünün toplanabilir miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			item.Quantity = quantity;

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

    private void NextPositionAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (CurrentPosition != TotalPosition)
                CurrentPosition = CurrentPage.FindByName<CarouselView>("carouselView").Position + 1;
        }
        catch (System.Exception)
        {
            _userDialogs.Alert("Bir hata oluştu.", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void PreviousPositionAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (CurrentPosition == 0)
                return;

            CurrentPosition = CurrentPage.FindByName<CarouselView>("carouselView").Position - 1;
        }
        catch (System.Exception)
        {
            _userDialogs.Alert("Bir hata oluştu.", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task ProcurementInfoAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("procurementInfoBottomSheet").State = BottomSheetState.HalfExpanded;
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
