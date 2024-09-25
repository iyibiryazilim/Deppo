using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;

[QueryProperty(nameof(WarehouseCountingWarehouseModel), nameof(WarehouseCountingWarehouseModel))]
[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
public partial class WarehouseCountingBasketViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IWarehouseCountingService _warehouseCountingService;
    private readonly ILocationTransactionService _locationTransactionService;

    [ObservableProperty]
    private WarehouseCountingWarehouseModel warehouseCountingWarehouseModel = null!;

    [ObservableProperty]
    private LocationModel locationModel = null!;

    [ObservableProperty]
    private WarehouseCountingBasketModel selectedItem = null!;


    public WarehouseCountingBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseCountingService warehouseCountingService, ILocationTransactionService locationTransactionService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _warehouseCountingService = warehouseCountingService;
        _locationTransactionService = locationTransactionService;

        Title = "Ürünler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        IncreaseCommand = new Command<WarehouseCountingBasketModel>(async (item) => await IncreaseAsync(item));
        DecreaseCommand = new Command<WarehouseCountingBasketModel>(async (item) => await DecreaseAsync(item));
        SwipeItemCommand = new Command<WarehouseCountingBasketModel>(async (item) => await SwipeItemAsync(item));
        NextViewCommand = new Command(async () => await NextViewAsync());

    }

    public Page CurrentPage { get; set; } = null!;

    public ObservableCollection<WarehouseCountingBasketModel> Items { get; } = new();

    public ObservableCollection<WarehouseCountingBasketModel> SelectedItems { get; } = new();

    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();


    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command NextViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command SwipeItemCommand { get; }
    public Command BackCommand { get; }
    



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
            var result = await _warehouseCountingService.GetProductsByWarehouseAndLocation(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, LocationModel.ReferenceId, string.Empty, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                        obj.OutputQuantity = obj.StockQuantity;
                        Items.Add(obj);
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
            var result = await _warehouseCountingService.GetProductsByWarehouseAndLocation(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseCountingWarehouseModel.Number, LocationModel.ReferenceId, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<WarehouseCountingBasketModel>(item);
                        obj.OutputQuantity = obj.StockQuantity;
                        Items.Add(obj);
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

    private async Task IncreaseAsync(WarehouseCountingBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                SelectedItem = item;

                item.OutputQuantity++;
                item.DifferenceQuantity++;

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
            SelectedItem = item;

            if (item is not null)
            {
                if (item.OutputQuantity > 1 && (item.OutputQuantity - item.StockQuantity) <= 0)
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
                else if (item.OutputQuantity > 1 && (item.OutputQuantity - item.StockQuantity) > 0)
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
                productReferenceId: SelectedItem.ProductReferenceId,
                warehouseNumber: WarehouseCountingWarehouseModel.Number,
                skip:0,
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

				SelectedItem.DifferenceQuantity--;


				if (LocationTransactions.Sum(x => x.RemainingQuantity) > 0)
                {
                    SelectedItem.DifferenceQuantity--;
                    if((SelectedItem.DifferenceQuantity * -1) > LocationTransactions.Sum(x => x.RemainingQuantity))
                    {
						await _userDialogs.AlertAsync("Girilen miktarı karşılayacak giriş hareketi bulunamadı", "Uyarı", "Tamam");
						return;
					}
					else
					{
						SelectedItem.OutputQuantity--;


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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if(Items.All(x => x.IsCompleted == false))
            {
                await _userDialogs.AlertAsync("Hiç ürün sayımı yapılmadı", "Uyarı", "Tamam");
                return;
            }
            else
            {
                foreach (var item in Items.Where(x=>x.IsCompleted).ToList())
                {
                   SelectedItems.Add(item);
                }

                await Shell.Current.GoToAsync($"{nameof(WarehouseCountingFormView)}", new Dictionary<string, object>
                {
                    [nameof(LocationModel)] = LocationModel,
                    [nameof(WarehouseCountingWarehouseModel)] = WarehouseCountingWarehouseModel,
                    [nameof(WarehouseCountingBasketModel)] = SelectedItems
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

}
