using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
[QueryProperty(name: nameof(SalesFicheModel), queryId: nameof(SalesFicheModel))]
[QueryProperty(name: nameof(SelectedSalesTransactions), queryId: nameof(SelectedSalesTransactions))]
public partial class ReturnSalesDispatchBasketViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ILocationService _locationService;
    private readonly ISeriLotService _seriLotService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISubUnitsetService _subUnitsetService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SalesCustomer salesCustomer;

    [ObservableProperty]
    private ReturnSalesBasketModel? selectedItem;

    [ObservableProperty]
    private ObservableCollection<ReturnSalesBasketModel> items;

    [ObservableProperty]
    private SalesFicheModel salesFicheModel = null!;

    [ObservableProperty]
    public ObservableCollection<SalesTransactionModel> selectedSalesTransactions;

	public ReturnSalesDispatchBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider, ISubUnitsetService subUnitsetService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;
		_subUnitsetService = subUnitsetService;

		Title = "Satış İade Sepeti";

        UnitActionTappedCommand = new Command<ReturnSalesBasketModel>(async (x) => await UnitActionTappedAsync(x));
        SubUnitsetTappedCommand = new Command<SubUnitset>(async (x) => await SubUnitsetTappedAsync(x));
		QuantityTappedCommand = new Command<ReturnSalesBasketModel>(async (x) => await QuantityTappedAsync(x));
		IncreaseCommand = new Command<ReturnSalesBasketModel>(async (x) => await IncreaseAsync(x));
		DecreaseCommand = new Command<ReturnSalesBasketModel>(async (x) => await DecreaseAsync(x));

		LoadMoreWarehouseLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
		//LocationIncreaseCommand = new Command<LocationModel>(LocationIncrease);
		//LocationDecreaseCommand = new Command<LocationModel>(LocationDecrease);
		//LocationConfirmCommand = new Command(LocationConfirm);
		LocationCloseCommand = new Command(async () => await LocationCloseAsync());

		LoadMoreSeriLotsCommand = new Command(async () => await LoadMoreSeriLotAsync());
		SeriLotIncreaseCommand = new Command<SeriLotModel>(SeriLotIncrease);
		SeriLotDecreaseCommand = new Command<SeriLotModel>(SeriLotDecrease);
		SeriLotConfirmCommand = new Command(SeriLotConfirm);
		SeriLotCloseCommand = new Command(async () => await SeriLotCloseAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
        DeleteItemCommand = new Command<ReturnSalesBasketModel>(async (item) => await DeleteItemAsync(item));

        BackCommand = new Command(async () => await BackAsync());

		//  ShowOtherProductCommand = new Command(async () => await ShowOtherProductAsync());
	}

	public Page CurrentPage { get; set; }

    public ObservableCollection<LocationModel> Locations { get; } = new();
    public ObservableCollection<SeriLotModel> SeriLots { get; } = new();

    #region Commands
    public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }
	public Command<ReturnSalesBasketModel> QuantityTappedCommand { get; }
    public Command<ReturnSalesBasketModel> DeleteItemCommand { get; }
    public Command<ReturnSalesBasketModel> IncreaseCommand { get; }
    public Command<ReturnSalesBasketModel> DecreaseCommand { get; }

    public Command LoadMoreWarehouseLocationsCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command LocationConfirmCommand { get; }
    public Command LocationCloseCommand { get; }

    public Command LoadMoreSeriLotsCommand { get; }
    public Command<SeriLotModel> SeriLotIncreaseCommand { get; }
    public Command<SeriLotModel> SeriLotDecreaseCommand { get; }
    public Command SeriLotConfirmCommand { get; }
    public Command SeriLotCloseCommand { get; }

    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public Command ShowOtherProductCommand { get; }

	#endregion Commands

	private async Task UnitActionTappedAsync(ReturnSalesBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = item;
			await LoadSubUnitsetsAsync(item);
			CurrentPage.FindByName<BottomSheet>("subUnitsetBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task LoadSubUnitsetsAsync(ReturnSalesBasketModel item)
	{
		if (item is null)
			return;
		try
		{
			item.SubUnitsets.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _subUnitsetService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: item.ItemReferenceId
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var subUnitset in result.Data)
				{
					item.SubUnitsets.Add(Mapping.Mapper.Map<SubUnitset>(subUnitset));
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task SubUnitsetTappedAsync(SubUnitset subUnitset)
	{
		if (subUnitset is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedItem is not null)
			{
				SelectedItem.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedItem.SubUnitsetName = subUnitset.Name;
				SelectedItem.SubUnitsetCode = subUnitset.Code;
			}

			CurrentPage.FindByName<BottomSheet>("subUnitsetBottomSheet").State = BottomSheetState.Hidden;
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

	private async Task QuantityTappedAsync(ReturnSalesBasketModel returnSalesBasketModel)
	{
		if (IsBusy)
			return;
		if (returnSalesBasketModel is null)
			return;
        if (returnSalesBasketModel.LocTracking == 1)
            return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: returnSalesBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: returnSalesBasketModel.InputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			returnSalesBasketModel.InputQuantity = quantity;
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

	private async Task IncreaseAsync(ReturnSalesBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedItem = item;
            if (item.LocTracking == 1)
            {
                var nextViewModel = _serviceProvider.GetRequiredService<ReturnSalesDispatchBasketLocationListViewModel>();


                nextViewModel.WarehouseModel = WarehouseModel;
				nextViewModel.ReturnSalesBasketModel = item;
                nextViewModel.SalesCustomer = SalesCustomer;

				await nextViewModel.LoadSelectedItemsAsync();

                await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketLocationListView)}", new Dictionary<string, object>
                {
                    {nameof(WarehouseModel), WarehouseModel},
                    {nameof(ReturnSalesBasketModel), item},
                    {nameof(SalesCustomer) , SalesCustomer }
                });
            }

            // Sadece SeriLot takipli ise
            else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
            {
                await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketSeriLotListView)}", new Dictionary<string, object>
                {
                     {nameof(WarehouseModel), WarehouseModel},
                    {nameof(ReturnSalesBasketModel), item}
                    ,{nameof(SeriLotModel), SeriLots}
                });
            }
            //stok yeri ve serilot takipli değilse
            else
            {
                item.Quantity++;
            }
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DecreaseAsync(ReturnSalesBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item is not null)
            {
                if (item.Quantity >= 1)
                {
                    // Stok Yeri takipli ise locationTransactionBottomSheet aç
                    if (item.LocTracking == 1)
                    {
                        var nextViewModel = _serviceProvider.GetRequiredService<ReturnSalesDispatchBasketLocationListViewModel>();

						nextViewModel.WarehouseModel = WarehouseModel;
						nextViewModel.ReturnSalesBasketModel = item;
						nextViewModel.SalesCustomer = SalesCustomer;

						await nextViewModel.LoadSelectedItemsAsync();
                        await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketLocationListView)}", new Dictionary<string, object>
                        {
                            {nameof(WarehouseModel), WarehouseModel},
                            {nameof(ReturnSalesBasketModel), item},
                            {nameof(SalesCustomer),SalesCustomer}
                        });

                    }
                    // Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
                    else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
                    {
                        await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketSeriLotListView)}", new Dictionary<string, object>
                        {
                            {nameof(WarehouseModel), WarehouseModel},
                            {nameof(ReturnSalesBasketModel), item}
                        });
                    }
                    // Stok yeri ve SeriLot takipli değilse
                    else
                    {
                        item.Quantity--;
                    }
                }
            }
        }
        catch (Exception ex)
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

    private async Task DeleteItemAsync(ReturnSalesBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!result)
                return;

            Items.Remove(item);
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

    [Obsolete("Not used")]
    private async Task LoadWarehouseLocationsAsync(ReturnSalesBasketModel returnSalesBasketModel)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Locations.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, returnSalesBasketModel.ItemReferenceId,0, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
    }

    [Obsolete("Not used")]
    private async Task LoadMoreWarehouseLocationsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SelectedItem.ItemReferenceId, search: string.Empty, skip: Locations.Count, take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
            }
        }
        catch (Exception ex)
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

    [Obsolete("Not used")]
    private async Task LocationCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    [Obsolete("Not used")]
    private async Task LocationIncreaseAsync(LocationModel locationModel)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            locationModel.InputQuantity++;
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync($"{ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [Obsolete("Not used")]
    private async Task LocationConfirmAsync(LocationModel locationModel)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            if (Locations.Count > 0)
            {
                double totalInputQuantity = 0;
                foreach (var location in Locations)
                {
                    if (location.InputQuantity > 0)
                    {
                        totalInputQuantity += location.InputQuantity;
                    }
                }
                SelectedItem.Quantity = totalInputQuantity;

                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [Obsolete("Not used")]
    private async Task LocationDecreaseAsync(LocationModel locationModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (locationModel.InputQuantity > 0)
            {
                locationModel.InputQuantity -= 1;
            }
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

    [Obsolete("Not used")]
    private async Task LoadSeriLotAsync(ReturnSalesBasketModel inputPurchaseBasketModel)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            SeriLots.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, search: string.Empty, skip: 0, take: 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        SeriLots.Add(Mapping.Mapper.Map<SeriLotModel>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
    }

    [Obsolete("Not used")]
    private async Task LoadMoreSeriLotAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, search: string.Empty, skip: SeriLots.Count, take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    SeriLots.Add(Mapping.Mapper.Map<SeriLotModel>(item));
            }
        }
        catch (Exception ex)
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

    [Obsolete("Not used")]
    private async Task SeriLotCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    [Obsolete("Not used")]
    private void SeriLotIncrease(SeriLotModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            item.InputQuantity += 1;
        }
        catch (Exception ex)
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

    [Obsolete("Not used")]
    private void SeriLotDecrease(SeriLotModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item.InputQuantity > 0)
            {
                item.InputQuantity -= 1;
            }
        }
        catch (Exception ex)
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

    [Obsolete("Not used")]
    private void SeriLotConfirm()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SeriLots.Count > 0)
            {
                double totalInputQuantity = 0;
                foreach (var seriLot in SeriLots)
                {
                    if (seriLot.InputQuantity > 0)
                        totalInputQuantity += seriLot.InputQuantity;
                }

                SelectedItem.Quantity = totalInputQuantity;

                CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception ex)
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (Items.Count == 0)
            {
                await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
                return;
            }

            if(Items.Any(x => x.InputQuantity ==0))
            {
				foreach (var item in Items.Where(x => x.InputQuantity == 0))
				{
					Items.Remove(item);
				}
			}
            
            await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchFormView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(SalesCustomer)] = SalesCustomer,
                [nameof(SalesFicheModel)] = SalesFicheModel,
                [nameof(Items)] = Items
            });
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

    private async Task ShowOtherProductAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchProductListView)}", new Dictionary<string, object>
            {
                {nameof(WarehouseModel), WarehouseModel},
                {nameof(SalesCustomer),SalesCustomer}
            });
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (Items.Count > 0)
            {
                var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                    return;

                foreach (var item in Items)
                {
                    item.IsSelected = false;
                }
                foreach(var item in SelectedSalesTransactions)
                {
                    item.IsSelected=false;
                }
                SelectedSalesTransactions.Clear();
                Items.Clear();
                await Shell.Current.GoToAsync("..");
            }
            else
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

    public async Task LoadPageAsync()
    {
        try
        {
            if (Items?.Count > 0)
                Items.Clear();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
}