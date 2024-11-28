using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputOutsourceTransferProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWarehouseTotalService _warehouseTotalService;
	private readonly IVariantWarehouseTotalService _variantWarehouseTotalService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	public SearchBar searchText;

	[ObservableProperty]
    WarehouseTotalModel? selectedProduct;

	public ObservableCollection<VariantWarehouseTotalModel> ItemVariants { get; } = new();
	public ObservableCollection<WarehouseTotalModel> Items { get; } = new();

    //Arama İşleminde seçilenlerin tutulması için liste
    public ObservableCollection<WarehouseTotalModel> SelectedItems { get; } = new();

    [ObservableProperty]
    public ObservableCollection<OutputOutsourceTransferBasketModel> selectedProducts = new();

	public OutputOutsourceTransferProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IVariantWarehouseTotalService variantWarehouseTotalService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseTotalService = warehouseTotalService;
        _variantWarehouseTotalService = variantWarehouseTotalService;
        _serviceProvider = serviceProvider;
        _userDialogs = userDialogs;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseTotalModel>(async (item) => await ItemTappedAsync(item));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantWarehouseTotalModel>(async (item) => await VariantTappedAsync(item));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }

    public Command LoadVariantItemsCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

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
                httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: 0,
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                    var matchedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    if (matchedItem is not null)
                        item.IsSelected = matchedItem.IsSelected;
                    else
                        item.IsSelected = false;

                    Items.Add(item);
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

            _userDialogs.Loading("Loading Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(
                httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: Items.Count,
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                    var matchedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    if (matchedItem is not null)
                        item.IsSelected = matchedItem.IsSelected;
                    else
                        item.IsSelected = false;

                    Items.Add(item);
                }
            }

            _userDialogs.HideHud();
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

    private async Task ItemTappedAsync(WarehouseTotalModel item)
    {
        if (item is null)
            return;
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            SelectedProduct = item;

            if(item.IsSelected)
            {
                item.IsSelected = false;
                SelectedItems.Remove(item);

				var selectedItem = SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == item.ProductReferenceId);
				if (selectedItem is not null)
				{
					SelectedProducts.Remove(selectedItem);
				}
			}
            else
            {
                if(item.IsVariant)
                {
                    await LoadVariantItemsAsync(item);
                    CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                else
                {
                    item.IsSelected = true;
                    SelectedItems.Add(item);

					var basketItem = new OutputOutsourceTransferBasketModel
					{
						ItemReferenceId = item.ProductReferenceId,
						ItemCode = item.ProductCode,
						ItemName = item.ProductName,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        Image = item.ImageData,
						SubUnitsetCode = item.SubUnitsetCode,
						SubUnitsetName = item.SubUnitsetName,
						MainItemReferenceId = default,  //
						MainItemCode = string.Empty,    //
						MainItemName = string.Empty,    //
						StockQuantity = item.StockQuantity,
						IsSelected = false,   //
						IsVariant = item.IsVariant,
						LocTracking = item.LocTracking,
						TrackingType = item.TrackingType,
						Quantity = item.LocTracking == 0 ? 1 : 0,
						LocTrackingIcon = item.LocTrackingIcon,
						VariantIcon = item.VariantIcon,
						TrackingTypeIcon = item.TrackingTypeIcon,
					};

					SelectedProducts.Add(basketItem);
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

    private async Task ConfirmAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            var previousViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();

            if (previousViewModel is not null)
            {
                if (SelectedProducts.Any())
                {
                    foreach (var item in SelectedProducts)
                        if (!previousViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
                            previousViewModel.Items.Add(item);

                    SelectedProducts.Clear();
                }

                SelectedItems.Clear();
                await Shell.Current.GoToAsync("..");
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

    private async Task LoadVariantItemsAsync(WarehouseTotalModel item)
    {
		try
		{

			_userDialogs.Loading("Loading Variant Items");
			ItemVariants.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantWarehouseTotalService.GetObjects(
                httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                productReferenceId: item.ProductReferenceId, 
                warehouseNumber: WarehouseModel.Number, 
                skip: 0, 
                take: 20
            );

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

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

			_userDialogs.Loading("Loading More Variant Items");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantWarehouseTotalService.GetObjects(
				httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedProduct.ProductReferenceId,
				warehouseNumber: WarehouseModel.Number,
				skip: ItemVariants.Count,
				take: 20
			);

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

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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

	private async Task VariantTappedAsync(VariantWarehouseTotalModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

            if(item.IsSelected)
            {
                item.IsSelected = false;
            }

			ItemVariants.ToList().ForEach(x => x.IsSelected = false);
			var selectedItem = ItemVariants.FirstOrDefault(x => x.VariantReferenceId == item.VariantReferenceId);
			if (selectedItem != null)
				selectedItem.IsSelected = true;
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


			var basketItem = new OutputOutsourceTransferBasketModel
			{
				ItemReferenceId = item.VariantReferenceId,
				ItemCode = item.VariantCode,
				ItemName = item.VariantName,
				UnitsetReferenceId = item.UnitsetReferenceId,
				UnitsetCode = item.UnitsetCode,
				UnitsetName = item.UnitsetName,
				SubUnitsetReferenceId = item.SubUnitsetReferenceId,
				SubUnitsetCode = item.SubUnitsetCode,
				SubUnitsetName = item.SubUnitsetName,
				MainItemReferenceId = item.ProductReferenceId, 
				MainItemCode = item.ProductCode,    
				MainItemName = item.ProductName,    
				StockQuantity = item.StockQuantity,
				IsSelected = false,   
				IsVariant = true,
				LocTracking = item.LocTracking,
				TrackingType = item.TrackingType,
				Quantity = item.LocTracking == 0 ? 1 : 0,
				LocTrackingIcon = item.LocTrackingIcon,
				VariantIcon = item.VariantIcon,
				TrackingTypeIcon = item.TrackingTypeIcon,
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
                Items.ForEach(x => x.IsSelected = false);
                SelectedItems.ForEach(x => x.IsSelected = false);
                SelectedItems.Clear();
                SelectedProducts.Clear();
            }
            await Shell.Current.GoToAsync("..");
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
            _userDialogs.Loading("Searching Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(
                httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: 0,
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                    var matchedItem = SelectedItems.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                    if (matchedItem is not null)
                        item.IsSelected = matchedItem.IsSelected;
                    else
                        item.IsSelected = false;

                    Items.Add(item);
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


    private async Task PerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }

    public async Task ClearPageAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                if(SelectedProduct is not null)
                {
					SelectedProduct.IsSelected = false;
					SelectedProduct = null;
				}
				SelectedItems.ForEach(x => x.IsSelected = false);
				SelectedItems.Clear();
				SelectedProducts.Clear();
				Items.Clear();
				ItemVariants.Clear();
				SearchText.Text = string.Empty;
               
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


}
