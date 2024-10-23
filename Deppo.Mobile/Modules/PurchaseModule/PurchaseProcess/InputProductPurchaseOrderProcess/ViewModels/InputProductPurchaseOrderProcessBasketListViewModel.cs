using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
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
    private readonly IBarcodeSearchHelper _barcodeSearchHelper;
    private readonly ISubUnitsetService _subUnitsetService;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    InputPurchaseBasketModel? selectedInputPurchaseBasketModel;

    [ObservableProperty]
    public ObservableCollection<InputPurchaseBasketModel> items = new();

	[ObservableProperty]
	public Entry barcodeEntry;

	public InputProductPurchaseOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider, IBarcodeSearchHelper barcodeSearchHelper, ISubUnitsetService subUnitsetService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;
		_barcodeSearchHelper = barcodeSearchHelper;
		_subUnitsetService = subUnitsetService;

		Title = "Satınalma Sepeti";

		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
		QuantityTappedCommand = new Command<InputPurchaseBasketModel>(async (x) => await QuantityTappedAsync(x));
		IncreaseCommand = new Command<InputPurchaseBasketModel>(async (x) => await IncreaseAsync(x));
		DecreaseCommand = new Command<InputPurchaseBasketModel>(async (x) => await DecreaseAsync(x));
		DeleteItemCommand = new Command<InputPurchaseBasketModel>(async (x) => await DeleteItemAsync(x));

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());

		PlusTappedCommand = new Command(async () => await PlusTappedAsync());
		ProductOptionTappedCommand = new Command(async () => await ProductOptionTappedAsync());
		OrderOptionTappedCommand = new Command(async () => await OrderOptionTappedAsync());

		ShowOtherProductCommand = new Command(async () => await ShowOtherProductAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());

		UnitActionTappedCommand = new Command<InputPurchaseBasketModel>(async (x) => await UnitActionTappedAsync(x));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (x) => await SubUnitsetTappedAsync(x));
	}

	public Page CurrentPage { get; set; }

    #region Commands
    public Command PerformSearchCommand { get; }
	public Command<InputPurchaseBasketModel> QuantityTappedCommand { get; }
	public Command<InputPurchaseBasketModel> DeleteItemCommand { get; }
    public Command<InputPurchaseBasketModel> IncreaseCommand { get; }
    public Command<InputPurchaseBasketModel> DecreaseCommand { get; }

    public Command NextViewCommand { get; }
    public Command BackCommand { get; }
	public Command PlusTappedCommand { get; }
	public Command ProductOptionTappedCommand { get; }
	public Command OrderOptionTappedCommand { get; }
	public Command CameraTappedCommand { get; }

	public Command ShowOtherProductCommand { get; }

    public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }

	#endregion Commands

	private async Task UnitActionTappedAsync(InputPurchaseBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedInputPurchaseBasketModel = item;
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

	private async Task LoadSubUnitsetsAsync(InputPurchaseBasketModel item)
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

			if (SelectedInputPurchaseBasketModel is not null)
			{
				SelectedInputPurchaseBasketModel.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedInputPurchaseBasketModel.SubUnitsetName = subUnitset.Name;
				SelectedInputPurchaseBasketModel.SubUnitsetCode = subUnitset.Code;
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

	private async Task PerformSearchAsync(Entry barcodeEntry)
	{
		if (IsBusy)
			return;
		try
		{
			if (string.IsNullOrEmpty(barcodeEntry.Text))
				return;

			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			await _barcodeSearchHelper.BarcodeDetectedAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				barcode: barcodeEntry.Text,
				comingPage: "InputProductPurchaseOrderProcessBasketListViewModel"
			);

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			BarcodeEntry.Text = string.Empty;
            barcodeEntry.Text = string.Empty;
            barcodeEntry.Focus();
			IsBusy = false;
		}
	}
	private async Task QuantityTappedAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
	{
		if (inputPurchaseBasketModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: inputPurchaseBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				initialValue: inputPurchaseBasketModel.InputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity <= 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			inputPurchaseBasketModel.InputQuantity = quantity;
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

				if(nextViewModel.WarehouseModel is null)
				{
					nextViewModel.WarehouseModel = WarehouseModel;
				}
				if(nextViewModel.InputPurchaseBasketModel is null)
				{
					nextViewModel.InputPurchaseBasketModel = inputPurchaseBasketModel;
				}

                await nextViewModel.LoadSelectedItemsAsync();

                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketLocationListView)}", new Dictionary<string, object>
                {
                    {nameof(WarehouseModel), WarehouseModel},
                    {nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
                });
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
        if (item is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!result)
                return;

            item.Details.Clear();

            Items.Remove(item);
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
            
            if(Items.Any(x => x.InputQuantity == 0))
            {
                await _userDialogs.AlertAsync("Sepetinizde miktarı sıfır olan ürünler bulunmakta. Lütfen yeniden düzenleme yapınız", "Uyarı", "Tamam");
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

                foreach (var item in Items)
                    item.Details.Clear();

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