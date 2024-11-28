using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
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

namespace Deppo.Mobile.Modules.ProductModule.Variant;

[QueryProperty(name: nameof(ProductModel), queryId: nameof(ProductModel))]
public partial class ProductVariantListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IVariantService _variantService;
    private readonly IUserDialogs _userDialogs;


    public ObservableCollection<VariantModel> Items { get; } = new();

    public ObservableCollection<ProductVariantModel> ProductVariants { get; } = new();



    [ObservableProperty]
    ProductModel productModel= null!;


    [ObservableProperty]
    private VariantModel? selectedVariant;

    public ProductVariantListViewModel(IHttpClientService httpClientService, ICountingPanelService countingPanelService, IUserDialogs userDialogs, IVariantService variantService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _variantService = variantService;

        Title = "Varyant Listesi";

        BackCommand = new Command(async () => await BackAsync());
        ItemTappedCommand = new Command<VariantModel>(async (variant) => await ItemTappedAsync(variant));
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command BackCommand { get; }
    public Command ItemTappedCommand { get; }
    public Page CurrentPage { get; set; }



    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            Items.Clear();
            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _variantService.GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber,ProductModel.ReferenceId,"" ,0, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<VariantModel>(item));
                    }
                }
            }
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
        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _variantService.GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductModel.ReferenceId, "", Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<VariantModel>(item));
                    }
                }
            }
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


    private async Task ItemTappedAsync(VariantModel variant)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedVariant = variant;

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            await GetVariantCharAsgnAsync(variant);

            CurrentPage.FindByName<BottomSheet>("variantCharAsgnBottomSheet").State = BottomSheetState.HalfExpanded;

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.", "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetVariantCharAsgnAsync(VariantModel variantModel)
    {
        try
        {
            ProductVariants.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _variantService.GetProductVariants(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                variantReferenceId: variantModel.ReferenceId,
                search: "",
                skip: 0,
                take: 9999
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<ProductVariantModel>(item);
                        ProductVariants.Add(obj);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
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
                Items.Clear();
            }
            if (ProductVariants.Count > 0)
            {
                ProductVariants.Clear();
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
