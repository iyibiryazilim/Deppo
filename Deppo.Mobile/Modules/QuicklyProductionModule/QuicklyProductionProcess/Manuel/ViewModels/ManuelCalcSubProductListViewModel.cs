using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
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


namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    public partial class ManuelCalcSubProductListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWarehouseTotalService _warehouseTotalService;

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        public ManuelCalcSubProductListViewModel(
            IHttpClientService httpClientService,
            IUserDialogs userDialogs,
            IServiceProvider serviceProvider,
            IWarehouseTotalService warehouseTotalService)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _serviceProvider = serviceProvider;
            _warehouseTotalService = warehouseTotalService;

            Title = "Ürün Listesi";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<WarehouseTotalModel>(async (item) => await ItemTappedAsync(item));
            ConfirmCommand = new Command(async () => await ConfirmAsync());

        }

        public Page CurrentPage { get; set; }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command NextViewCommand { get; }
        public Command<WarehouseTotalModel> ItemTappedCommand { get; }
        public Command ConfirmCommand { get; }

        public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
        public ObservableCollection<WarehouseTotalModel> SelectedItems { get; } = new();


        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Items.Clear();
                SelectedItems.Clear();

                _userDialogs.Loading("Loading Items...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await Task.Delay(1000);

                var result = await _warehouseTotalService.GetObjects(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    search: string.Empty,
                    skip: 0,
                    take: 20);


                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                        Items.Add(item);
                    }
                }

                if (_userDialogs.IsHudShowing)
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
            }
        }

        private async Task LoadMoreItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Items.Clear();
                SelectedItems.Clear();

                _userDialogs.Loading("Loading Items...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await Task.Delay(500);

                var result = await _warehouseTotalService.GetObjects(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    search: string.Empty,
                    skip: Items.Count,
                    take: 20);


                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);
                        Items.Add(item);
                    }
                }

                if (_userDialogs.IsHudShowing)
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
            }
        }

        private async Task ItemTappedAsync(WarehouseTotalModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (item.IsSelected)
                {
                    Items.FirstOrDefault(x => x.ProductCode == item.ProductCode).IsSelected = false;
                    SelectedItems.Remove(item);
                }
                else
                {
                    if (item.IsVariant)
                    {
                        await _userDialogs.AlertAsync("Bu ürün varyant üründür. Lütfen varyant ürünlerinizi seçiniz.", "Uyarı", "Tamam");
                        return;
                    }
                    else
                    {
                        Items.FirstOrDefault(x => x.ProductCode == item.ProductCode).IsSelected = true;
                        SelectedItems.Add(item);
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

        private async Task ConfirmAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var manuelCalcViewModel = _serviceProvider.GetRequiredService<ManuelCalcViewModel>();
                if (manuelCalcViewModel is not null)
                {
                    foreach (var item in SelectedItems)
                    {
                        if (!manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.Any(x => x.ProductModel.Code == item.ProductCode))
                        {
                            manuelCalcViewModel.QuicklyBomProductBasketModel.SubProducts.Add(new QuicklyBomSubProductModel
                            {
                                WarehouseModel = WarehouseModel,
                                SubBOMQuantity = 0,
                                SubOutputQuantity = 0,
                                ProductModel = new BOMSubProductModel
                                {
                                    ReferenceId = item.ProductReferenceId,
                                    Code = item.ProductCode,
                                    Name = item.ProductName,
                                    UnitsetReferenceId = item.UnitsetReferenceId,
                                    UnitsetCode = item.UnitsetCode,
                                    UnitsetName = item.UnitsetName,
                                    SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                                    SubUnitsetCode = item.SubUnitsetCode,
                                    SubUnitsetName = item.SubUnitsetName,
                                    Amount = default,
                                    BrandCode = item.BrandCode,
                                    BrandName = item.BrandName,
                                    GroupCode = item.GroupCode,
                                    Image = item.Image,
                                    BrandReferenceId = item.BrandReferenceId,
                                    IsSelected = false,
                                    LocTracking = item.LocTracking,
                                    LocTrackingIcon = item.LocTrackingIcon,
                                    MainProductCode = item.ProductCode,
                                    MainProductReferenceId = item.ProductReferenceId,
                                    StockQuantity = item.StockQuantity,
                                    TrackingType = item.TrackingType,
                                    VariantIcon = item.VariantIcon,
                                    TrackingTypeIcon = item.TrackingTypeIcon,
                                    VatRate = default,
                                    WarehouseName = WarehouseModel.Name,
                                    WarehouseNumber = WarehouseModel.Number,
                                    IsVariant = item.IsVariant
                                },
                                LocationTransactions = new()
                            });
                        }
                    }
                }

                await Shell.Current.GoToAsync("../..");


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
}
