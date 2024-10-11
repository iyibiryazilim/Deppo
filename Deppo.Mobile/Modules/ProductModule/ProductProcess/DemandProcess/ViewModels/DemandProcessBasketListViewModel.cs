using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using DevExpress.Maui.Controls;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using System.Collections.ObjectModel;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels
{
    [QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
    public partial class DemandProcessBasketListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly IServiceProvider _serviceProvider;


        [ObservableProperty]
        WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        DemandProcessBasketModel? selectedItem;


        #region Collections
        public ObservableCollection<DemandProcessBasketModel> Items { get; } = new();

        #endregion

        public DemandProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;

            _userDialogs = userDialogs;
            _serviceProvider = serviceProvider;

            Title = "Sepet Listesi";

            ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
            IncreaseCommand = new Command<DemandProcessBasketModel>(async (item) => await IncreaseAsync(item));
            DecreaseCommand = new Command<DemandProcessBasketModel>(async (item) => await DecreaseAsync(item));
            DeleteItemCommand = new Command<DemandProcessBasketModel>(async (item) => await DeleteItemAsync(item));
            NextViewCommand = new Command(async () => await NextViewAsync());
            BackCommand = new Command(async () => await BackAsync());
            CameraTappedCommand = new Command(async () => await CameraTappedAsync());
            SelectProductsCommand = new Command(async () => await SelectProductsAsync());
            SelectVariantsCommand = new Command(async () => await SelectVariantsAsync());

        }

        #region Properties
        public ContentPage CurrentPage { get; set; } = null!;

        #endregion

        #region Commands
        public Command ShowProductViewCommand { get; }
        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }
        public Command DeleteItemCommand { get; }
        public Command NextViewCommand { get; }
        public Command BackCommand { get; }
        public Command CameraTappedCommand { get; }
        public Command SelectProductsCommand { get; }
        public Command SelectVariantsCommand { get; }
        #endregion


        private async Task ShowProductViewAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                CurrentPage.FindByName<BottomSheet>("productTypeBottomSheet").State = BottomSheetState.HalfExpanded;

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

        private async Task SelectProductsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                CurrentPage.FindByName<BottomSheet>("productTypeBottomSheet").State = BottomSheetState.Hidden;
                await Shell.Current.GoToAsync($"{nameof(DemandProcessProductListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel
                });
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

        private async Task SelectVariantsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                CurrentPage.FindByName<BottomSheet>("productTypeBottomSheet").State = BottomSheetState.Hidden;
                await Shell.Current.GoToAsync($"{nameof(DemandProcessVariantListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel
                });
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


        private async Task DeleteItemAsync(DemandProcessBasketModel item)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

                if (!result)
                    return;

                if (SelectedItem == item)
                {
                    SelectedItem.IsSelected = false;
                    SelectedItem = null;
                }

                Items.Remove(item);
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

        private async Task IncreaseAsync(DemandProcessBasketModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    SelectedItem = item;

                    item.Quantity++;

                }
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

        private async Task DecreaseAsync(DemandProcessBasketModel item)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    SelectedItem = item;

                    if (item.Quantity > 0)
                        item.Quantity--;


                }
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

        private async Task NextViewAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                if (Items.Count == 0)
                {
                    await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
                    return;
                }

                bool isQuantityValid = Items.All(x => x.Quantity > 0);
                if (!isQuantityValid)
                {
                    await _userDialogs.AlertAsync("Sepetinizde miktarı 0 olan ürünler bulunmaktadır.", "Uyarı", "Tamam");
                    return;
                }

                await Shell.Current.GoToAsync($"{nameof(DemandProcessFormView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(Items)] = Items
                });
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

        private async Task BackAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                if (Items.Count > 0)
                {
                    var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                    if (!result)
                        return;

                    Items.Clear();

                    var productListViewModel = _serviceProvider.GetRequiredService<DemandProcessProductListViewModel>();
                    var variantListViewModel = _serviceProvider.GetRequiredService<DemandProcessVariantListViewModel>();

                    foreach (var item in productListViewModel.Items)
                        item.IsSelected = false;

                    foreach (var item in variantListViewModel.Items)
                        item.IsSelected = false;

                    variantListViewModel.Items.Clear();
                    variantListViewModel.SelectedVariants.Clear();

                    productListViewModel.Items.Clear();
                    productListViewModel.SelectedProducts.Clear();

                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.GoToAsync("..");
                }
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

        private async Task CameraTappedAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;


                await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
                {
                    ["ComingPage"] = "DemandProcessBasket"
				});
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
}
