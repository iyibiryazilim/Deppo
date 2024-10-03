using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]

public partial class InputProductPurchaseOrderProcessBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ILocationService _locationService;
    private readonly ISeriLotService _seriLotService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    InputPurchaseBasketModel? selectedInputPurchaseBasketModel;

    [ObservableProperty]
    public ObservableCollection<InputPurchaseBasketModel> items = new();

    public InputProductPurchaseOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _locationService = locationService;
        _seriLotService = seriLotService;
        _serviceProvider = serviceProvider;

        Title = "Satınalma Sepeti";

        IncreaseCommand = new Command<InputPurchaseBasketModel>(async (x) => await IncreaseAsync(x));

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
        BackCommand = new Command(async () => await BackAsync());
		PlusTappedCommand = new Command(async () => await PlusTappedAsync());
		ProductOptionTappedCommand = new Command(async () => await ProductOptionTappedAsync());
		OrderOptionTappedCommand = new Command(async () => await OrderOptionTappedAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());

		ShowOtherProductCommand = new Command(async () => await ShowOtherProductAsync());
    }

    public Page CurrentPage { get; set; }

    public ObservableCollection<LocationModel> Locations { get; } = new();
    public ObservableCollection<SeriLotModel> SeriLots { get; } = new();

    #region Commands

    public Command<InputPurchaseBasketModel> DeleteItemCommand { get; }
    public Command<InputPurchaseBasketModel> IncreaseCommand { get; }
    public Command<InputPurchaseBasketModel> DecreaseCommand { get; }

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
	public Command PlusTappedCommand { get; }
	public Command ProductOptionTappedCommand { get; }
	public Command OrderOptionTappedCommand { get; }
	public Command CameraTappedCommand { get; }

	public Command ShowOtherProductCommand { get; }

    #endregion Commands

    private async Task ShowProductViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(InputProductProcessProductListView)}", new Dictionary<string, object>
            {
                {nameof(WarehouseModel), WarehouseModel}
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

    private async Task IncreaseAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedInputPurchaseBasketModel = inputPurchaseBasketModel;
            if (inputPurchaseBasketModel.LocTracking == 1)
            {
                var nextViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketLocationListViewModel>();

                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketLocationListView)}", new Dictionary<string, object>
                {
                    {nameof(WarehouseModel), WarehouseModel},
                    {nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
                });
                await nextViewModel.LoadSelectedItemsAsync();
            }

            // Sadece SeriLot takipli ise
            else if (inputPurchaseBasketModel.LocTracking == 0 && (inputPurchaseBasketModel.TrackingType == 1 || inputPurchaseBasketModel.TrackingType == 2))
            {
                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketSeriLotListView)}", new Dictionary<string, object>
                {
                     {nameof(WarehouseModel), WarehouseModel},
                    {nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
                });
            }
            //stok yeri ve serilot takipli değilse
            else
            {
                inputPurchaseBasketModel.Quantity++;
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

    private async Task DecreaseAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (inputPurchaseBasketModel is not null)
            {
                if (inputPurchaseBasketModel.Quantity > 1)
                {
                    // Stok Yeri takipli ise locationTransactionBottomSheet aç
                    if (inputPurchaseBasketModel.LocTracking == 1)
                    {
                        await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessBasketLocationListView)}", new Dictionary<string, object>
                        {
                            {nameof(WarehouseModel), WarehouseModel},
                            {nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
                        });
                    }
                    // Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
                    else if (inputPurchaseBasketModel.LocTracking == 0 && (inputPurchaseBasketModel.TrackingType == 1 || inputPurchaseBasketModel.TrackingType == 2))
                    {
                        await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessBasketSeriLotListView)}", new Dictionary<string, object>
                        {
                            {nameof(WarehouseModel), WarehouseModel},
                            {nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
                        });
                    }
                    // Stok yeri ve SeriLot takipli değilse
                    else
                    {
                        inputPurchaseBasketModel.Quantity--;
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

    private async Task DeleteItemAsync(InputPurchaseBasketModel item)
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
    private async Task LoadWarehouseLocationsAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            Locations.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, inputPurchaseBasketModel.ItemReferenceId, string.Empty, 0, 20);
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
            var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SelectedInputPurchaseBasketModel.ItemReferenceId, search: string.Empty, skip: Locations.Count, take: 20);

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
                SelectedInputPurchaseBasketModel.Quantity = totalInputQuantity;

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
    private async Task LoadSeriLotAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
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

                SelectedInputPurchaseBasketModel.Quantity = totalInputQuantity;

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

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessFormView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(PurchaseSupplier)] = PurchaseSupplier,
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

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessOtherProductListView)}", new Dictionary<string, object>
            {
                {nameof(WarehouseModel), WarehouseModel},
                {nameof(PurchaseSupplier),PurchaseSupplier }
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

    private async Task CameraTappedAsync()
    {
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
			{
				["ComingPage"] = "InputProductPurchaseOrderBasket"
			});
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

	private async Task PlusTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task ProductOptionTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;

			await Task.Delay(300);
			await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(PurchaseSupplier)] = PurchaseSupplier
			});
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

	private async Task OrderOptionTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;
			await Task.Delay(300);

            Console.WriteLine("WarehouseModel: " + WarehouseModel);
            Console.WriteLine("PurchaseSupplier: " + PurchaseSupplier);

			await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessOrderListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(PurchaseSupplier)] = PurchaseSupplier
			});
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