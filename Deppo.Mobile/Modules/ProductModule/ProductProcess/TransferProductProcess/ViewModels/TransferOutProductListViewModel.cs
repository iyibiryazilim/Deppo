using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class TransferOutProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseTotalService _warehouseTotalService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IVariantWarehouseTotalService _variantWarehouseTotalService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
    public ObservableCollection<WarehouseTotalModel> SelectedItems { get; } = new();
    public ObservableCollection<VariantWarehouseTotalModel> ItemVariants { get; } = new();

    [ObservableProperty]
    public ObservableCollection<OutProductModel> selectedProducts = new();

    [ObservableProperty]
    private WarehouseTotalModel selectedProduct = null!;

    public ContentPage CurrentPage { get; set; } = null!;

    public TransferOutProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IServiceProvider serviceProvider, IVariantWarehouseTotalService variantWarehouseTotalService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseTotalService = warehouseTotalService;
        _serviceProvider = serviceProvider;
        _variantWarehouseTotalService = variantWarehouseTotalService;
        _userDialogs = userDialogs;

        Title = "Ürün Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseTotalModel>(async (parameter) => await ItemTappedAsync(parameter));
        LoadVariantItemsCommand = new Command(async () => await LoadVariantItemsAsync());
        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantWarehouseTotalModel>(async (parameter) => await VariantTappedAsync(parameter));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        BackCommand = new Command(async () => await BackAsync());
        ConfirmCommand = new Command(async () => await ConfirmAsync());
	}

    #region Commands

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command LoadVariantItemsCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }

    public Command ConfirmCommand { get; }
    public Command PerformSearchCommand { get; }

    public Command PerformEmptySearchCommand { get; }
    public Command BackCommand { get; }
    public Command NextViewCommand { get; }

    #endregion Commands

    [ObservableProperty]
    public SearchBar searchText;

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            Items.Clear();
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                warehouseNumber: WarehouseModel.Number, 
                search: SearchText.Text,
                skip: 0,
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                    var matcedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    if (matcedItem is not null)
                        item.IsSelected = matcedItem.IsSelected;
                    else
                        item.IsSelected = false;

                    Items.Add(item);
                }
            }

            if (_userDialogs.IsHudShowing)
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

        if (Items.Count < 18)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading More Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                warehouseNumber: WarehouseModel.Number, 
                skip: Items.Count, 
                take: 20, 
                search: SearchText.Text,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                    var matcedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    if (matcedItem is not null)
                        item.IsSelected = matcedItem.IsSelected;
                    else
                        item.IsSelected = false;

                    Items.Add(item);
                }
            }

            if (_userDialogs.IsHudShowing)
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

    private async Task ItemTappedAsync(WarehouseTotalModel item)
    {
        if (IsBusy)
            return;

        if (item is null)
            return;

        try
        {
            IsBusy = true;

			if (item.StockQuantity < 0)
			{
				await _userDialogs.AlertAsync("Negatif stoğu bulunan malzeme transfer edilemez.", "Uyarı", "Tamam");
				item.IsSelected = false;
				return;
			}

			if (!item.IsSelected)
            {
                if (item.IsVariant)
                {
                    SelectedProduct = item;
                    await LoadVariantItemsAsync();
                    CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = true;

                    var outProductModel = new OutProductModel()
                    {
                        ReferenceId = item.ReferenceId,
                        ItemReferenceId = item.ProductReferenceId,
                        ItemCode = item.ProductCode,
                        ItemName = item.ProductName,
                        UnitsetReferenceId = item.UnitsetReferenceId,
                        UnitsetCode = item.UnitsetCode,
                        UnitsetName = item.UnitsetName,
                        SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SubUnitsetName = item.SubUnitsetName,
                        StockQuantity = item.StockQuantity,
                        MainItemReferenceId = item.ProductReferenceId,
                        MainItemCode = item.ProductCode,
                        MainItemName = item.ProductName,
                        IsVariant = item.IsVariant,
                        LocTracking = item.LocTracking,
                        TrackingType = item.TrackingType,
                        LocTrackingIcon = item.LocTrackingIcon,
                        VariantIcon = item.VariantIcon,
                        TrackingTypeIcon = item.TrackingTypeIcon,
                        OutputQuantity = item.LocTracking == 0 ? 1 : 0,
                        IsSelected = item.IsSelected,
                        Image = item.Image
                    };

                    SelectedProducts.Add(outProductModel);
                    SelectedItems.Add(item);
                }
            }
            else
            {
                var selectedItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ProductReferenceId);

				if (item.IsVariant)
                {
                    selectedItem = SelectedProducts.FirstOrDefault(x => x.MainItemReferenceId == item.ProductReferenceId);
                }


                if (selectedItem is not null)
                {
					SelectedProducts.Remove(selectedItem);
                    Items.ToList().FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId).IsSelected = false;
					SelectedItems.Remove(item);
                }
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

    private async Task LoadVariantItemsAsync()
    {
        try
        {
            _userDialogs.Loading("Loading Variant Items");
            ItemVariants.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantWarehouseTotalService.GetObjects(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                productReferenceId: SelectedProduct.ProductReferenceId, 
                warehouseNumber: WarehouseModel.Number, 
                skip: 0, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;
                foreach (var variant in result.Data)
                {
                    var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
                    ItemVariants.Add(obj);
                }
            }
            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
        }
    }

    private async Task LoadMoreVariantItemsAsync()
    {
        if (IsBusy)
            return;
        if (ItemVariants.Count < 18)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantWarehouseTotalService.GetObjects(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                warehouseNumber: WarehouseModel.Number, 
                productReferenceId: SelectedProduct.ProductReferenceId, 
                search: "",
                skip: ItemVariants.Count, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var variant in result.Data)
                {
                    var obj = Mapping.Mapper.Map<VariantWarehouseTotalModel>(variant);
                    ItemVariants.Add(obj);
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task VariantTappedAsync(VariantWarehouseTotalModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

			if (item.IsSelected)
			{
				item.IsSelected = false;
			}
			else
			{
				item.IsSelected = true;
				ItemVariants.Where(x => x.VariantReferenceId != item.VariantReferenceId).ToList().ForEach(x => x.IsSelected = false);
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

    private async Task ConfirmVariantAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var item = ItemVariants.FirstOrDefault(x => x.IsSelected);

            if(item is null)
            {
				CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;
                return;
			}

            var basketItem = new OutProductModel()
            {
                ReferenceId = item.ReferenceId,
                ItemReferenceId = item.VariantReferenceId,
                ItemCode = item.VariantCode,
                ItemName = item.VariantName,
                UnitsetReferenceId = item.UnitsetReferenceId,
                UnitsetCode = item.UnitsetCode,
                UnitsetName = item.UnitsetName,
                SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                SubUnitsetCode = item.SubUnitsetCode,
                SubUnitsetName = item.SubUnitsetName,
                StockQuantity = item.StockQuantity,
                MainItemReferenceId = item.ProductReferenceId,
                MainItemCode = item.ProductCode,
                MainItemName = item.ProductName,
                IsVariant = true,
                LocTracking = item.LocTracking,
                TrackingType = item.TrackingType,
                LocTrackingIcon = item.LocTrackingIcon,
                VariantIcon = item.VariantIcon,
                TrackingTypeIcon = item.TrackingTypeIcon,
                OutputQuantity = item.LocTracking == 0 ? 1 : 0,
                IsSelected = item.IsSelected,
                Image = item.Image
            };
            SelectedProducts.Add(basketItem);
            if (SelectedProduct is not null)
            {
                SelectedProduct.IsSelected = true;
                SelectedItems.Add(SelectedProduct);
            }
            CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;
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

    //private async Task NextViewAsync()
    //{
    //    if (IsBusy)
    //        return;
    //    try
    //    {
    //        IsBusy = true;

    //        if (TransferBasketModel.OutProducts.Count == 0)
    //        {
    //            await _userDialogs.AlertAsync("Lütfen en az bir ürün seçiniz.", "Uyarı", "Tamam");
    //            return;
    //        }

    //        TransferBasketModel.OutWarehouse = WarehouseModel;
    //        SelectedItems.Clear();

    //        await Shell.Current.GoToAsync($"{nameof(TransferOutBasketView)}", new Dictionary<string, object>
    //        {
    //            [nameof(TransferBasketModel)] = TransferBasketModel,
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        if (_userDialogs.IsHudShowing)
    //            _userDialogs.HideHud();

    //        _userDialogs.Alert(ex.Message);
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}

    private async Task ConfirmAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            SearchText.Text = string.Empty;

            var previousViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();

            if(SelectedProducts.Any())
            {
                foreach (var item in SelectedProducts)
                {
                    if(!previousViewModel.TransferBasketModel.OutProducts.Any(x => x.ItemReferenceId == item.ItemReferenceId))
                    {
						previousViewModel.TransferBasketModel.OutProducts.Add(item);
					}
                }
            }

			SearchText.Text = string.Empty;
			await Shell.Current.GoToAsync($"..");
			SelectedProducts.Clear();
			SelectedItems.Clear();
		}
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
		}
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PerformSearchAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                await LoadItemsAsync();
                SearchText.Unfocus();
                return;
            }
            IsBusy = true;
            Items.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(
                httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                warehouseNumber: WarehouseModel.Number, 
                search: SearchText.Text, 
                skip: 0, 
                take: 20,
                externalDb: _httpClientService.ExternalDatabase);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                    var matcedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    item.IsSelected = matcedItem is not null ? matcedItem.IsSelected : false;
                    Items.Add(item);
                }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
           
            if (SelectedProducts.Count > 0)
            {
				var result = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
				if (!result)
				{
					return;
				}
				SelectedProducts.Clear();
			}

            SelectedItems.ForEach(x => x.IsSelected = false); 

            SearchText.Text = string.Empty;
            await Shell.Current.GoToAsync($"..");
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