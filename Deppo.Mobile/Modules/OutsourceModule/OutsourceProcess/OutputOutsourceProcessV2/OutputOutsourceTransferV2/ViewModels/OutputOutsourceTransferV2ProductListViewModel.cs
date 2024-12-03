using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;

public partial class OutputOutsourceTransferV2ProductListViewModel :BaseViewModel
{
  
    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IVariantService _variantService;

    

    public OutputOutsourceTransferV2ProductListViewModel(IHttpClientService httpClientService,
        IVariantService variantService,
        IUserDialogs userDialogs,
        IServiceProvider serviceProvider)
    {

         Title = "Ürün Listesi";
        _httpClientService = httpClientService;
        _variantService = variantService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;


        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductModel>(async (parameter) => await ItemTappedAsync(parameter));
        LoadVariantItemsCommand = new Command<ProductModel>(async (parameter) => await LoadVariantItemsAsync(parameter));
        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantModel>(async (parameter) => await VariantTappedAsync(parameter));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());


    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command<ProductModel> LoadVariantItemsCommand { get; }
    public Command LoadMoreVariantItemsCommand { get; }
    public Command<VariantModel> VariantTappedCommand { get; }
    public Command ConfirmVariantCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    public ObservableCollection<ProductModel> Items { get; } = new();

    //Arama işleminde seçilen ürünlerin listesi
    public ObservableCollection<ProductModel> SelectedItems { get; } = new();

    public ObservableCollection<VariantModel> ItemVariants { get; } = new();

    [ObservableProperty]
    public SearchBar searchText;


    private async Task LoadItemsAsync()
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

    private async Task ItemTappedAsync(ProductModel productModel)
    {
        if (IsBusy)
            return;
       
    }

    private async Task LoadVariantItemsAsync(ProductModel item)
    {
        try
        {
            _userDialogs.Loading("Loading Variant Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await Task.Delay(1000);
            var result = await _variantService.GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ReferenceId, string.Empty, 0, 20);

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
            IsBusy = false;
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

    private async Task VariantTappedAsync(VariantModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            ItemVariants.ToList().ForEach(x => x.IsSelected = false);
            var selectedItem = ItemVariants.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
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

            //var basketItem = new InputOutsourceTransferBasketModel
            //{
            //    ItemReferenceId = item.ReferenceId,
            //    ItemCode = item.Code,
            //    ItemName = item.Name,
            //    UnitsetReferenceId = item.UnitsetReferenceId,
            //    UnitsetCode = item.UnitsetCode,
            //    UnitsetName = item.UnitsetName,
            //    SubUnitsetReferenceId = item.SubUnitsetReferenceId,
            //    SubUnitsetCode = item.SubUnitsetCode,
            //    SubUnitsetName = item.SubUnitsetName,
            //    IsSelected = false,
            //    MainItemCode = item.ProductCode,
            //    MainItemName = item.ProductName,
            //    MainItemReferenceId = item.ProductReferenceId,
            //    StockQuantity = item.StockQuantity,
            //    Quantity = item.LocTracking == 0 ? 1 : 0,
            //    TrackingType = item.TrackingType,
            //    IsVariant = true,
            //    LocTracking = item.LocTracking,
            //};

            //SelectedProducts.Add(basketItem);

            //if (SelectedProduct is not null)
            //{
            //    SelectedProduct.IsSelected = true;
            //    SelectedItems.Add(SelectedProduct);
            //}

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

    private async Task ConfirmAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            //var previouseViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferOutsourceBasketListViewModel>();
            //if (previouseViewModel is not null)
            //{
            //    foreach (var item in SelectedProducts)
            //        if (!previouseViewModel.Items.Any(x => x.ItemCode == item.ItemCode))
            //            previouseViewModel.Items.Add(item);

            //    await Shell.Current.GoToAsync($"..");
            //}
            //SelectedProducts.ForEach(x => x.IsSelected = false);
            //SelectedProducts.Clear();
            //SelectedItems.Clear();
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
            //var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            //if (result.IsSuccess)
            //{
            //    if (result.Data == null)
            //        return;

            //    foreach (var product in result.Data)
            //    {
            //        var item = Mapping.Mapper.Map<Product>(product);
            //        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
            //        Items.Add(new ProductModel
            //        {
            //            ReferenceId = item.ReferenceId,
            //            Code = item.Code,
            //            Name = item.Name,
            //            UnitsetReferenceId = item.UnitsetReferenceId,
            //            UnitsetCode = item.UnitsetCode,
            //            UnitsetName = item.UnitsetName,
            //            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
            //            SubUnitsetCode = item.SubUnitsetCode,
            //            SubUnitsetName = item.SubUnitsetName,
            //            StockQuantity = item.StockQuantity,
            //            TrackingType = item.TrackingType,
            //            LocTracking = item.LocTracking,
            //            IsVariant = item.IsVariant,
            //            Image = item.Image,
            //            IsSelected = matchedItem != null ? matchedItem.IsSelected : false
            //        });
            //    }
            //}

            //if (!result.IsSuccess)
            //{
            //    _userDialogs.Alert(result.Message, "Hata");
            //    return;
            //}
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






}
