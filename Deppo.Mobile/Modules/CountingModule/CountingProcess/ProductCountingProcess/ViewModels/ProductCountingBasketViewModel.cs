using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

[QueryProperty(nameof(ProductCountingWarehouseModel), nameof(ProductCountingWarehouseModel))]
[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(ProductCountingBasketModel), nameof(ProductCountingBasketModel))]
public partial class ProductCountingBasketViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseCountingService _warehouseCountingService;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ISubUnitsetService _subUnitsetService;

    [ObservableProperty]
    private ProductCountingWarehouseModel productCountingWarehouseModel = null!;

    [ObservableProperty]
    private LocationModel? locationModel;

    [ObservableProperty]
    private ProductCountingBasketModel productCountingBasketModel = null!;

    [ObservableProperty]
    bool isIncrease;

    public ProductCountingBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseCountingService warehouseCountingService, ILocationTransactionService locationTransactionService, ISubUnitsetService subUnitsetService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_warehouseCountingService = warehouseCountingService;
		_locationTransactionService = locationTransactionService;
		_subUnitsetService = subUnitsetService;

		Title = "Sepet";

        QuantityTappedCommand = new Command(async () => await QuantityTappedAsync());
		IncreaseCommand = new Command(async () => await IncreaseAsync());
		DecreaseCommand = new Command(async () => await DecreaseAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());

		UnitActionTappedCommand = new Command(async () => await UnitActionTappedAsync());
        SubUnitsetTappedCommand = new Command<SubUnitset>(async (item) => await SubUnitsetTappedAsync(item));
    }

    public Page CurrentPage { get; set; } = null!;

    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();
    public Command QuantityTappedCommand { get; }
	public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }

	public Command NextViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command BackCommand { get; }

	private async Task QuantityTappedAsync()
	{
		if (ProductCountingBasketModel is null)
			return;

		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: ProductCountingBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: ProductCountingBasketModel.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}
			
			ProductCountingBasketModel.OutputQuantity = quantity;

			if (ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity < 0)
			{
				ProductCountingBasketModel.DifferenceQuantity = ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity;
				await LoadLocationTransactionsAsync();
			}
			else
			{
				ProductCountingBasketModel.DifferenceQuantity = ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity;
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


	private async Task IncreaseAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            IsIncrease = true;
            if (ProductCountingBasketModel is not null)
            {

                if (ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity < 0)
                {
                    ProductCountingBasketModel.OutputQuantity++;
                    ProductCountingBasketModel.DifferenceQuantity++;
                    
                    await LoadLocationTransactionsAsync();

                }
                else
                {
                    ProductCountingBasketModel.OutputQuantity++;
                    ProductCountingBasketModel.DifferenceQuantity++;
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

    private async Task DecreaseAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            IsIncrease = false;
            if (ProductCountingBasketModel is not null)
            {
                if (ProductCountingBasketModel.OutputQuantity > 0 && (ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity) <= 0)
                {
                    if (ProductCountingBasketModel.LocTracking == 1)
                    {
                       
                        await LoadLocationTransactionsAsync();

                    }
                    // Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
                    else if (ProductCountingBasketModel.LocTracking == 0 && (ProductCountingBasketModel.TrackingType == 1 || ProductCountingBasketModel.TrackingType == 2))
                    {
                        //await LoadSeriLotTransactionsAsync();

                    }
                    // Stok yeri ve SeriLot takipli değilse
                    else
                    {
                        ProductCountingBasketModel.OutputQuantity--;
                        ProductCountingBasketModel.DifferenceQuantity--;
                    }
                }
                else if (ProductCountingBasketModel.OutputQuantity > 0 && (ProductCountingBasketModel.OutputQuantity - ProductCountingBasketModel.StockQuantity) > 0)
                {

                    ProductCountingBasketModel.OutputQuantity--;
                    ProductCountingBasketModel.DifferenceQuantity--;
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
                productReferenceId:ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.MainItemReferenceId : ProductCountingBasketModel.ItemReferenceId,
                warehouseNumber: ProductCountingWarehouseModel.Number,
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

                    if ((ProductCountingBasketModel.DifferenceQuantity * -1) >= LocationTransactions.Sum(x => x.RemainingQuantity))
                    {
                        await _userDialogs.AlertAsync("Girilen miktarı karşılayacak giriş hareketi bulunamadı", "Uyarı", "Tamam");
                        return;
                    }
                    else
                    {
                        if (!IsIncrease)
                        {
                            ProductCountingBasketModel.DifferenceQuantity--;
                            ProductCountingBasketModel.OutputQuantity--;

                        }

                        ProductCountingBasketModel.LocationTransactions = new();

                        var orderedLocationTransactions = LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                        var tempQuantity = ProductCountingBasketModel.StockQuantity - ProductCountingBasketModel.OutputQuantity;
                        foreach (var item in orderedLocationTransactions)
                        {
                            if (item.RemainingQuantity > 0 && tempQuantity > 0)
                            {
                                item.OutputQuantity = (tempQuantity) >= item.RemainingQuantity ? item.RemainingQuantity : tempQuantity;
                                tempQuantity -= item.OutputQuantity;
                                ProductCountingBasketModel.LocationTransactions.Add(item);
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

	private async Task UnitActionTappedAsync()
	{
        if (ProductCountingBasketModel is null)
            return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await LoadSubUnitsetsAsync(ProductCountingBasketModel);
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

	private async Task LoadSubUnitsetsAsync(ProductCountingBasketModel item)
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

			if (ProductCountingBasketModel is not null)
			{
				ProductCountingBasketModel.SubUnitsetReferenceId = subUnitset.ReferenceId;
				ProductCountingBasketModel.SubUnitsetName = subUnitset.Name;
				ProductCountingBasketModel.SubUnitsetCode = subUnitset.Code;
                ProductCountingBasketModel.ConversionFactor = subUnitset.ConversionValue;
                ProductCountingBasketModel.OtherConversionFactor = subUnitset.OtherConversionValue;


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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Miktarı sayılan ürünlerle sayım işlemine devam etmek istiyor musunuz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			await Shell.Current.GoToAsync($"{nameof(ProductCountingFormView)}", new Dictionary<string, object>
			{
				[nameof(LocationModel)] = LocationModel,
				[nameof(ProductCountingBasketModel)] = ProductCountingBasketModel,
				[nameof(ProductCountingWarehouseModel)] = ProductCountingWarehouseModel
			});


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

            var confirm = await _userDialogs.ConfirmAsync("Miktarınız silinecektir. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;


			if (ProductCountingBasketModel is not null)
            {
                ProductCountingBasketModel.LocationTransactions.Clear();
                ProductCountingBasketModel.DifferenceQuantity = 0;
				ProductCountingBasketModel = null;
            }

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
