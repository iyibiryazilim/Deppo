using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.VariantModule.ViewModels
{
    public partial class VariantListViewModel :BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IProductService _productService;
        private readonly IVariantService _variantService;
        private readonly IUserDialogs _userDialogs;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private ProductModel? selectedProduct;

        [ObservableProperty]
        private ObservableCollection<InputProductBasketModel> selectedProducts = new();

        public ObservableCollection<VariantModel> Items { get; } = new();
        public ObservableCollection<VariantModel> SelectedItems { get; } = new();
        public ObservableCollection<VariantModel> ItemVariants { get; } = new();


        public VariantListViewModel(
        IHttpClientService httpClientService,
        IProductService productService,
        IVariantService variantService,
        IUserDialogs userDialogs,
        IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;
            _productService = productService;
            _variantService = variantService;
            _userDialogs = userDialogs;
            _serviceProvider = serviceProvider;

            Title = "Varyant Listesi";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            PerformSearchCommand = new Command(async () => await PerformSearchAsync());
            PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

        }

        public Page CurrentPage { get; set; } = null!;

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command<VariantModel> LoadVariantItemsCommand { get; }
        public Command LoadMoreVariantItemsCommand { get; }
       
        public Command BackCommand { get; }
        public Command PerformSearchCommand { get; }
        public Command PerformEmptySearchCommand { get; }

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
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _variantService.GetVariants(httpClient, firmNumber: _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: 12, "", 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<Variant>(product);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);

                        Items.Add(new VariantModel
                        {
                            ReferenceId = item.ReferenceId,
                            Code = item.Code,
                            Name = item.Name,
                            UnitsetReferenceId = item.UnitsetReferenceId,
                            UnitsetCode = item.UnitsetCode,
                            UnitsetName = item.UnitsetName,
                            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                            SubUnitsetCode = item.SubUnitsetCode,
                            SubUnitsetName = item.SubUnitsetName,
                            StockQuantity = item.StockQuantity,
                            TrackingType = item.TrackingType,
                            LocTracking = item.LocTracking,
                            GroupCode = item.GroupCode,
                            BrandReferenceId = item.BrandReferenceId,
                            BrandCode = item.BrandCode,
                            BrandName = item.BrandName,
                            VatRate = item.VatRate,
                            Image = item.Image,
                            IsVariant = item.IsVariant,
                            IsSelected = matchingItem != null ? matchingItem.IsSelected : false
                        });
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
                Items.Clear();

                _userDialogs.Loading("Loading Items...");
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _variantService.GetVariants(httpClient, firmNumber: _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: 12, "", 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<Variant>(product);
                        var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);

                        Items.Add(new VariantModel
                        {
                            ReferenceId = item.ReferenceId,
                            Code = item.Code,
                            Name = item.Name,
                            UnitsetReferenceId = item.UnitsetReferenceId,
                            UnitsetCode = item.UnitsetCode,
                            UnitsetName = item.UnitsetName,
                            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                            SubUnitsetCode = item.SubUnitsetCode,
                            SubUnitsetName = item.SubUnitsetName,
                            StockQuantity = item.StockQuantity,
                            TrackingType = item.TrackingType,
                            LocTracking = item.LocTracking,
                            GroupCode = item.GroupCode,
                            BrandReferenceId = item.BrandReferenceId,
                            BrandCode = item.BrandCode,
                            BrandName = item.BrandName,
                            VatRate = item.VatRate,
                            Image = item.Image,
                            IsVariant = item.IsVariant,
                            IsSelected = matchingItem != null ? matchingItem.IsSelected : false
                        });
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



        private async Task BackAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (SelectedItems.Count > 0)
                {
                    var confirm = await _userDialogs.ConfirmAsync("Ürünlerinizin seçimi kaldırılacaktır. Sayfayı kapatmak istiyor musunuz?", "Uyarı", "Evet", "Hayır");

                    if (!confirm)
                        return;

                    SelectedItems.Clear();
                    SelectedProducts.Clear();
                }

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
                var result = await _productService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<Variant>(product);
                        var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                        Items.Add(new VariantModel
                        {
                            ReferenceId = item.ReferenceId,
                            Code = item.Code,
                            Name = item.Name,
                            UnitsetReferenceId = item.UnitsetReferenceId,
                            UnitsetCode = item.UnitsetCode,
                            UnitsetName = item.UnitsetName,
                            SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                            SubUnitsetCode = item.SubUnitsetCode,
                            SubUnitsetName = item.SubUnitsetName,
                            StockQuantity = item.StockQuantity,
                            TrackingType = item.TrackingType,
                            LocTracking = item.LocTracking,
                            GroupCode = item.GroupCode,
                            BrandReferenceId = item.BrandReferenceId,
                            BrandCode = item.BrandCode,
                            BrandName = item.BrandName,
                            VatRate = item.VatRate,
                            Image = item.Image,
                            IsVariant = item.IsVariant,
                            IsSelected = matchedItem != null ? matchedItem.IsSelected : false
                        });
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








    }
}
