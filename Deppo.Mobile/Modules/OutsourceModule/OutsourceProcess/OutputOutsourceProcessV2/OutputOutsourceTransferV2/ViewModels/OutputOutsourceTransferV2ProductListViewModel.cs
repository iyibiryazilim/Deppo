using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(OutsourceModel), queryId: nameof(OutsourceModel))]


public partial class OutputOutsourceTransferV2ProductListViewModel : BaseViewModel
{

    private readonly IUserDialogs _userDialogs;
    private readonly IHttpClientService _httpClientService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IVariantService _variantService;
    private readonly IOutputOutsourceTransferV2Service _outputOutsourceTransferV2Service;
    private readonly IProductService _productService;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    OutsourceModel outsourceModel = null!;

    [ObservableProperty]
    public SearchBar searchText;

    [ObservableProperty]
    OutputOutsourceTransferV2ProductModel? selectedProduct;

    public ObservableCollection<VariantWarehouseTotalModel> ItemVariants { get; } = new();
    public ObservableCollection<OutputOutsourceTransferV2ProductModel> Items { get; } = new();

    //Arama İşleminde seçilenlerin tutulması için liste
    public ObservableCollection<OutputOutsourceTransferV2ProductModel> SelectedItems { get; } = new();

    [ObservableProperty]
    public ObservableCollection<OutputOutsourceTransferV2ProductModel> selectedProducts = new();

    public OutputOutsourceTransferV2ProductListViewModel(IHttpClientService httpClientService,
        IVariantService variantService,
        IUserDialogs userDialogs,
        IServiceProvider serviceProvider, IOutputOutsourceTransferV2Service outputOutsourceTransferV2Service,
        IProductService productService
        )
    {

        Title = "Ürün Listesi";
        _httpClientService = httpClientService;
        _variantService = variantService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;
        _outputOutsourceTransferV2Service = outputOutsourceTransferV2Service;
        _productService = productService;


        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<OutputOutsourceTransferV2ProductModel>(async (item) => await ItemTappedAsync(item));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

        NextViewCommand = new Command(async () => await NextViewAsync());



    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }

    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    public Command NextViewCommand { get; }




    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();

            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outputOutsourceTransferV2Service.GetObjects(
                httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                currentReferenceId:OutsourceModel.ReferenceId,
                search: SearchText.Text,
                skip: 0,
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<OutputOutsourceTransferV2ProductModel>(product);
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
            var result = await _outputOutsourceTransferV2Service.GetObjects(
                httpClient,
                _httpClientService.FirmNumber,
                _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                currentReferenceId: OutsourceModel.ReferenceId,
                search: SearchText.Text,
                skip: 0,
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<OutputOutsourceTransferV2ProductModel>(product);
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

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
            _userDialogs.Loading().Dispose();
        }
    }

    private async Task ItemTappedAsync(OutputOutsourceTransferV2ProductModel item)
    {
        if (item is null)
            return;
        if (IsBusy) return;

        try
        {
            IsBusy = true;

           
            if (SelectedProduct == item)
            {
                item.IsSelected = false;
                SelectedProduct = null; // Seçimi sıfırla
            }
            else
            {
                // Daha önce seçilen ürünü sıfırla (varsa)
                if (SelectedProduct != null)
                {
                    SelectedProduct.IsSelected = false;
                }

                // Yeni ürünü seç
                item.IsSelected = true;
                SelectedProduct = item;
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
            var result = await _outputOutsourceTransferV2Service.GetObjects(
              httpClient,
              _httpClientService.FirmNumber,
              _httpClientService.PeriodNumber,
              warehouseNumber: WarehouseModel.Number,
              currentReferenceId: OutsourceModel.ReferenceId,
              search: SearchText.Text,
              skip: 0,
              take: 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<OutputOutsourceTransferV2ProductModel>(item));

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


    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2OutsourceBasketView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(OutsourceModel)] = OutsourceModel,
                [nameof(OutputOutsourceTransferV2ProductModel)] = SelectedProduct
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






}
