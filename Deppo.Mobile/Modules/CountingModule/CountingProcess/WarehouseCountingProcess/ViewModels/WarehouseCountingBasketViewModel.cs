using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(nameof(WarehouseCountingWarehouseModel), nameof(WarehouseCountingWarehouseModel))]
[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(ProductVariantType), nameof(ProductVariantType))]
public partial class WarehouseCountingBasketViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseCountingService _warehouseCountingService;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ISubUnitsetService _subUnitsetService;
    private readonly IBarcodeSearchHelper _barcodeSearchHelper;

    [ObservableProperty]
    private WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;

    [ObservableProperty]
    private LocationModel? locationModel;

    [ObservableProperty]
    private WarehouseCountingBasketModel selectedItem = null!;

    [ObservableProperty]
    private ProductVariantType productVariantType;

	public WarehouseCountingBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseCountingService warehouseCountingService, ILocationTransactionService locationTransactionService, ISubUnitsetService subUnitsetService, IBarcodeSearchHelper barcodeSearchHelper)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_warehouseCountingService = warehouseCountingService;
		_locationTransactionService = locationTransactionService;
		_subUnitsetService = subUnitsetService;
		_barcodeSearchHelper = barcodeSearchHelper;

		Title = "Ürün Sepeti";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		UnitActionTappedCommand = new Command<WarehouseCountingBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (item) => await SubUnitsetTappedAsync(item));
		QuantityTappedCommand = new Command<WarehouseCountingBasketModel>(async (item) => await QuantityTappedAsync(item));
		IncreaseCommand = new Command<WarehouseCountingBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<WarehouseCountingBasketModel>(async (item) => await DecreaseAsync(item));
		SwipeItemCommand = new Command<WarehouseCountingBasketModel>(async (item) => await SwipeItemAsync(item));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		PerformSearchCommand = new Command<Entry>(async (barcodeEntry) => await PerformSearchAsync(barcodeEntry));
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());
	}


	[ObservableProperty]
    bool isIncrease;

    public Page CurrentPage { get; set; } = null!;

    public ObservableCollection<WarehouseCountingBasketModel> Items { get; } = new();

    public ObservableCollection<WarehouseCountingBasketModel> SelectedItems { get; } = new();

    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();


    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command UnitActionTappedCommand { get; }
    public Command SubUnitsetTappedCommand { get; }

    public Command ShowProductViewCommand { get; }
	public Command QuantityTappedCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command SwipeItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command CameraTappedCommand { get; }



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
 
            if(ProductVariantType == ProductVariantType.Variant)
            {
                Title = "Varyant Sepeti";
                var result = await _warehouseCountingService.GetVariantsByWarehouseAndLocation(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, LocationModel.ReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                            obj.OutputQuantity = obj.StockQuantity;
                            if(obj.StockQuantity > 0)
                            {
                                Items.Add(obj);
                            }
                        }
                    }
                }
            }
            else
            {
                var result = await _warehouseCountingService.GetProductsByWarehouseAndLocation(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, LocationModel.ReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                            obj.OutputQuantity = obj.StockQuantity;
							if (obj.StockQuantity > 0)
							{
								Items.Add(obj);
							}
                        }
                    }
                }
            }

            if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			var barcode = CurrentPage.FindByName<Entry>("barcodeEntry");
            if(barcode is not null && !barcode.IsFocused)
            {
                barcode.Focus();
            }
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
        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            if (ProductVariantType == ProductVariantType.Variant)
            {
                var result = await _warehouseCountingService.GetVariantsByWarehouseAndLocation(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, LocationModel.ReferenceId, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                            obj.OutputQuantity = obj.StockQuantity;
							if (obj.StockQuantity > 0)
							{
								Items.Add(obj);
							}
                        }
                    }
                }
            }
            else
            {
                var result = await _warehouseCountingService.GetProductsByWarehouseAndLocation(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, LocationModel.ReferenceId, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                            obj.OutputQuantity = obj.StockQuantity;
							if (obj.StockQuantity > 0)
							{
								Items.Add(obj);
							}
						}
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

            var result = await _barcodeSearchHelper.BarcodeDetectedAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                barcode: barcodeEntry.Text
            );

            if(result is not null)
            {
                Type resultType = result.GetType();
                if(resultType == typeof(BarcodeInProductModel))
                {
                    if(Items.Any(x => x.ItemCode == result.Code))
                    {
						_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
						return;
					}
                    else
                    {
						var basketItem = new WarehouseCountingBasketModel
						{
							ItemReferenceId = result.ReferenceId,
							ItemCode = result.Code,
							ItemName = result.Name,
							MainItemReferenceId = 0,
							MainItemCode = "",
							MainItemName = "",
							IsVariant = result.IsVariant,
							LocTracking = result.LocTracking,
							TrackingType = result.TrackingType,
							SubUnitsetCode = result.SubUnitsetCode,
							SubUnitsetReferenceId = result.SubUnitsetReferenceId,
							SubUnitsetName = result.SubUnitsetName,
							UnitsetReferenceId = result.UnitsetReferenceId,
							UnitsetCode = result.UnitsetCode,
							UnitsetName = result.UnitsetName,
							ConversionFactor = 1,
							OtherConversionFactor = 1,
							StockQuantity = result.StockQuantity,
							OutputQuantity = result.StockQuantity,
							Image = result.Image,
						};

                        Items.Add(basketItem);
					}
                }
            }
			else
			{
				_userDialogs.ShowToast($"{barcodeEntry.Text} barkodunda herhangi bir ürün bulunamadı");
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
			barcodeEntry.Text = string.Empty;
			barcodeEntry.Focus();
			IsBusy = false;
		}
    }

	private async Task UnitActionTappedAsync(WarehouseCountingBasketModel item)
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

	private async Task LoadSubUnitsetsAsync(WarehouseCountingBasketModel item)
	{
		if (item is null)
			return;
		try
		{
            if(item.SubUnitsets is null)
            {
                item.SubUnitsets = new();
			}

            if(item.SubUnitsets.Count > 0) 
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
                SelectedItem.ConversionFactor = subUnitset.ConversionValue;
                SelectedItem.OtherConversionFactor = subUnitset.OtherConversionValue;
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

	private async Task QuantityTappedAsync(WarehouseCountingBasketModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			SelectedItem = item;

			if (SelectedItem is not null)
			{
				
				SelectedItem.OutputQuantity = quantity;

				if (SelectedItem.OutputQuantity - SelectedItem.StockQuantity < 0)
				{
					// OutputQuantity Stok miktarından azsa
					SelectedItem.DifferenceQuantity = SelectedItem.OutputQuantity - SelectedItem.StockQuantity;
					await LoadLocationTransactionsAsync();
				}
				else
				{
					// OutputQuantity Stok miktarından fazla ve eşitse
					SelectedItem.DifferenceQuantity = SelectedItem.OutputQuantity - SelectedItem.StockQuantity;
				}
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
	private async Task IncreaseAsync(WarehouseCountingBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            IsIncrease = true;


            SelectedItem = item;
            if (SelectedItem is not null)
            {
                if (SelectedItem.OutputQuantity - SelectedItem.StockQuantity < 0)
                {

                    SelectedItem.OutputQuantity++;
                    SelectedItem.DifferenceQuantity++;
                    await LoadLocationTransactionsAsync();

                }
                else
                {
                    SelectedItem.OutputQuantity++;
                    SelectedItem.DifferenceQuantity++;
                }


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

    private async Task DecreaseAsync(WarehouseCountingBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            IsIncrease = false;
            SelectedItem = item;

            if (item is not null)
            {
                if (item.OutputQuantity > 0 && (item.OutputQuantity - item.StockQuantity) <= 0)
                {
                    if (item.LocTracking == 1)
                    {

                        await LoadLocationTransactionsAsync();

                    }
                    // Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
                    else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
                    {
                        //await LoadSeriLotTransactionsAsync();

                    }
                    // Stok yeri ve SeriLot takipli değilse
                    else
                    {
                        item.OutputQuantity--;
                        item.DifferenceQuantity--;
                    }
                }
                else if (item.OutputQuantity > 0 && (item.OutputQuantity - item.StockQuantity) > 0)
                {
                    item.OutputQuantity--;
                    item.DifferenceQuantity--;
                }

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

    private async Task LoadLocationTransactionsAsync()
    {
        try
        {
            LocationTransactions.Clear();


            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId:SelectedItem.IsVariant ? SelectedItem.MainItemReferenceId : SelectedItem.ItemReferenceId,
                variantReferenceId: SelectedItem.IsVariant ? SelectedItem.ItemReferenceId : 0,
                warehouseNumber: WarehouseCountingWarehouseModel.Number,
                locationRef: LocationModel.ReferenceId,
                skip: 0,
                take: 9999999
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
                }



                if (LocationTransactions.Sum(x => x.RemainingQuantity) > 0)
                {
                    if ((SelectedItem.DifferenceQuantity * -1) > LocationTransactions.Sum(x => x.RemainingQuantity))
                    {
                        await _userDialogs.AlertAsync("Girilen miktarı karşılayacak giriş hareketi bulunamadı", "Uyarı", "Tamam");
                        return;
                    }
                    else
                    {
                        if (!IsIncrease)
                        {
                            SelectedItem.DifferenceQuantity--;
                            SelectedItem.OutputQuantity--;

                        }

                        SelectedItem.LocationTransactions = new();

                        var orderedLocationTransactions = LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                        var tempQuantity = SelectedItem.StockQuantity - SelectedItem.OutputQuantity;
                        foreach (var item in orderedLocationTransactions)
                        {
                            if (item.RemainingQuantity > 0 && tempQuantity > 0)
                            {
                                item.OutputQuantity = (tempQuantity) >= item.RemainingQuantity ? item.RemainingQuantity : tempQuantity;
                                //SelectedItem.OutputQuantity--;
                                tempQuantity -= item.OutputQuantity;
                                SelectedItem.LocationTransactions.Add(item);
                            }
                        }
                    }

                }
                else
                {
                    await _userDialogs.AlertAsync("Giriş miktarı bulunamadı", "Uyarı", "Tamam");
                    return;
                }



            }

            _userDialogs.Loading().Hide();
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


    private async Task SwipeItemAsync(WarehouseCountingBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item.IsCompleted)
            {
                item.IsCompleted = false;
            }
            else
            {
                item.IsCompleted = true;
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

    private bool CheckStatus()
    {        
        return !Items.Any(x => !x.IsCompleted);
    }

    private async Task ShowProductViewAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;


		   await Shell.Current.GoToAsync($"{nameof(WarehouseCountingShowProductListView)}", new Dictionary<string, object>
           {
                [nameof(WarehouseCountingWarehouseModel)] = WarehouseCountingWarehouseModel,
                [nameof(LocationModel)] = LocationModel,
				[nameof(ProductVariantType)] = ProductVariantType
			});

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

    private async Task CameraTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CameraScanModel cameraScanModel = new CameraScanModel
            {
                ComingPage = "WarehouseCountingBasketViewModel"
            };

			await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
			{
				[nameof(CameraScanModel)] = cameraScanModel
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (CheckStatus())
            {
                SelectedItems.Clear();

                foreach (var item in Items.Where(x => (x.OutputQuantity - x.StockQuantity) != 0))
                {
                    SelectedItems.Add(item);
                }

                if (SelectedItems.Count == 0)
                {
                    await _userDialogs.AlertAsync("Sayım yapılacak ürün bulunamadı", "Uyarı", "Tamam");
                    return;
                }

                var confirm = await _userDialogs.ConfirmAsync("Miktarı sayılan ürünlerle sayım işlemine devam etmek istiyor musunuz?", "Onay", "Evet", "Hayır");
                if (!confirm)
                    return;

                await Shell.Current.GoToAsync($"{nameof(WarehouseCountingFormView)}", new Dictionary<string, object>
                {
                    [nameof(LocationModel)] = LocationModel,
                    [nameof(WarehouseCountingWarehouseModel)] = WarehouseCountingWarehouseModel,
                    [nameof(WarehouseCountingBasketModel)] = SelectedItems
                });
            }
            else
            {
                _userDialogs.ShowToast("Sepetinizde bekliyor durumunda olan ürünler var.");
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

            SelectedItems.Clear();
            await Shell.Current.GoToAsync("..");


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
