using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;

public partial class ProductCountingProductListViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly IUserDialogs _userDialogs;
    private readonly IVariantService _variantService;
    private readonly IHttpClientService _httpClientService;


	public ProductCountingProductListViewModel(IProductService productService, IUserDialogs userDialogs, IHttpClientService httpClientService, IVariantService variantService, ISubUnitsetService subUnitsetService)
	{
		_productService = productService;
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_variantService = variantService;

		Title = "Malzemeler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
		LoadVariantItemsCommand = new Command(async () => await LoadVariantItemsAsync());
		LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
		VariantTappedCommand = new Command<VariantModel>(async (parameter) => await VariantTappedAsync(parameter));
		ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
		ItemTappedCommand = new Command<ProductModel>(ItemTappedAsync);
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	[ObservableProperty]
    public ProductModel selectedProduct;

    [ObservableProperty]
    public ProductCountingBasketModel productCountingBasketModel = new();

    public ObservableCollection<ProductModel> Items { get; } = new();

    public ObservableCollection<VariantModel> ItemVariants { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command<ProductModel> ItemTappedCommand { get; }

    public Command LoadVariantItemsCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command<VariantModel> VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }

    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public Page CurrentPage { get; set; } = null!;


    [ObservableProperty]
    public SearchBar searchText;
    public async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();

            _userDialogs.Loading("Loading Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<ProductModel>(item));

                _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                Debug.WriteLine(result.Message);
                _userDialogs.Alert(message: result.Message, title: "Load Items");

            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadMoreItemsAsync()
    {
        if (Items.Count < 18)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.Loading("Refreshing Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<ProductModel>(item));

                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: result.Message, title: "Load Items");
            }
        }
        catch (Exception ex)
        {

            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Load Items Error");
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
            var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<ProductModel>(item));

                _userDialogs.Loading().Hide();
            }
            else
            {
                _userDialogs.Alert(result.Message, "Hata");
                return;
            }

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

    private async void ItemTappedAsync(ProductModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item == SelectedProduct)
            {
                SelectedProduct.IsSelected = false;
                SelectedProduct = null;
                ProductCountingBasketModel = null;
            }
            else
            {
                if (item.IsVariant)
                {
                    SelectedProduct = item;
                    await LoadVariantItemsAsync();
                    CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;

                }
                else
                {
                    if (SelectedProduct != null)
                    {
                        SelectedProduct.IsSelected = false;
                    }

                    SelectedProduct = item;
                    SelectedProduct.IsSelected = true;

                    ProductCountingBasketModel = new();
                    ProductCountingBasketModel.ItemReferenceId = SelectedProduct.ReferenceId;
                    ProductCountingBasketModel.ItemCode = SelectedProduct.Code;
                    ProductCountingBasketModel.ItemName = SelectedProduct.Name;
                    ProductCountingBasketModel.Image = SelectedProduct.ImageData;
                    ProductCountingBasketModel.MainItemReferenceId = default;
                    ProductCountingBasketModel.MainItemCode = string.Empty;
                    ProductCountingBasketModel.MainItemName = string.Empty;
                    ProductCountingBasketModel.StockQuantity = SelectedProduct.StockQuantity;
                    ProductCountingBasketModel.OutputQuantity = SelectedProduct.StockQuantity;
                    ProductCountingBasketModel.SubUnitsetReferenceId = SelectedProduct.SubUnitsetReferenceId;
                    ProductCountingBasketModel.SubUnitsetName = SelectedProduct.SubUnitsetName;
                    ProductCountingBasketModel.SubUnitsetCode = SelectedProduct.SubUnitsetCode;
                    ProductCountingBasketModel.UnitsetReferenceId = SelectedProduct.UnitsetReferenceId;
                    ProductCountingBasketModel.UnitsetName = SelectedProduct.UnitsetName;
                    ProductCountingBasketModel.UnitsetCode = SelectedProduct.UnitsetCode;
                    ProductCountingBasketModel.LocTracking = SelectedProduct.LocTracking;
                    ProductCountingBasketModel.IsVariant = SelectedProduct.IsVariant;
                    ProductCountingBasketModel.TrackingType = SelectedProduct.TrackingType;
                    ProductCountingBasketModel.DifferenceQuantity = 0;
                }

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

    private async Task VariantTappedAsync(VariantModel item)
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
            else
            {
                var selectedItem = ItemVariants.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                if (selectedItem != null)
                    selectedItem.IsSelected = true;
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
            ProductCountingBasketModel = new();
            ProductCountingBasketModel.ItemReferenceId = item.ReferenceId;
            ProductCountingBasketModel.ItemCode = item.Code;
            ProductCountingBasketModel.ItemName = item.Name;
            //ProductCountingBasketModel.Image = SelectedProduct.Image;
            ProductCountingBasketModel.MainItemReferenceId = SelectedProduct.ReferenceId;
            ProductCountingBasketModel.MainItemCode = SelectedProduct.Code;
            ProductCountingBasketModel.MainItemName =  SelectedProduct.Name;
            ProductCountingBasketModel.StockQuantity = item.StockQuantity;
            ProductCountingBasketModel.OutputQuantity = item.StockQuantity;
            ProductCountingBasketModel.SubUnitsetReferenceId = item.SubUnitsetReferenceId;
            ProductCountingBasketModel.SubUnitsetName = item.SubUnitsetName;
            ProductCountingBasketModel.SubUnitsetCode = item.SubUnitsetCode;
            ProductCountingBasketModel.UnitsetReferenceId = item.UnitsetReferenceId;
            ProductCountingBasketModel.UnitsetName = item.UnitsetName;
            ProductCountingBasketModel.UnitsetCode = item.UnitsetCode;
            ProductCountingBasketModel.LocTracking = item.LocTracking;
            ProductCountingBasketModel.IsVariant = true;
            ProductCountingBasketModel.TrackingType = item.TrackingType;
            ProductCountingBasketModel.DifferenceQuantity = 0;


            if (SelectedProduct is not null)
            {
                SelectedProduct.IsSelected = true;
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

    private async Task LoadVariantItemsAsync()
    {

        try
        {

            _userDialogs.Loading("Loading Variant Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantService
                                .GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedProduct.ReferenceId, string.Empty, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;
                ItemVariants.Clear();
                foreach (var variant in result.Data)
                {
                    var obj = Mapping.Mapper.Map<VariantModel>(variant);
                    ItemVariants.Add(obj);
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            _userDialogs.Loading().Dispose();
        }
    }

    private async Task LoadMoreVariantItemsAsync()
    {
        if (ItemVariants.Count < 18)
            return;
        if (IsBusy)
            return;

        try
        {

            IsBusy = true;

            _userDialogs.Loading("Loading More Variant Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantService
                                .GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedProduct.ReferenceId, string.Empty, ItemVariants.Count, 20);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var variant in result.Data)
                {
                    var obj = Mapping.Mapper.Map<VariantModel>(variant);
                    ItemVariants.Add(obj);
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
            _userDialogs.Loading().Dispose();
        }
    }

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SelectedProduct is not null)
            {
                await Shell.Current.GoToAsync($"{nameof(ProductCountingWarehouseTotalListView)}", new Dictionary<string, object>
                {
                    [nameof(ProductCountingBasketModel)] = ProductCountingBasketModel,
                });
            }
            else
            {
                await _userDialogs.AlertAsync("Lütfen bir ürün seçiniz.");
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

            if(SelectedProduct is not null)
            {
                SelectedProduct.IsSelected = false;
                SelectedProduct = null;
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
