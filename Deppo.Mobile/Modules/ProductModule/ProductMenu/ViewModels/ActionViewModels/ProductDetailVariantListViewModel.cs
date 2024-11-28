using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.ActionModels.ProductActionModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels
{
    [QueryProperty(nameof(ProductDetailModel), nameof(ProductDetailModel))]
    public partial class ProductDetailVariantListViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;
        private readonly IVariantService _variantService;

        public ProductDetailVariantListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IVariantService variantService)
        {
            _userDialogs = userDialogs;
            _httpClientService = httpClientService;
            _variantService = variantService;

            Title = "Varyantlar";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            BackCommand = new Command(async () => await BackAsync());
            ItemTappedCommand = new Command<VariantModel>(async (item) => await ItemTappedAsync(item));
            ActionModelsTappedCommand = new Command<VariantListActionModel>(async (model) => await ActionModelsTappedAsync(model));

        }


        [ObservableProperty]
        public ProductDetailModel productDetailModel;

        [ObservableProperty]
        public VariantModel selectedVariant;


        public ObservableCollection<VariantModel> Items { get; } = new();

        public ObservableCollection<VariantListActionModel> VariantActionModels { get; } = new();

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command BackCommand { get; }

        public Command ItemTappedCommand { get; }

        public Command ActionModelsTappedCommand { get; }

        public Page CurrentPage { get; set; }

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



                var result = await _variantService.GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<VariantModel>(item));

                    }
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
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                _userDialogs.Loading("Loading Items...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _variantService.GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProductDetailModel.Product.ReferenceId, string.Empty, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {

                        Items.Add(Mapping.Mapper.Map<VariantModel>(item));

                    }
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

        private async Task ItemTappedAsync(VariantModel item)
        {
            if (IsBusy)
                return;

            try
            {
                SelectedVariant = item;
                IsBusy = true;
                _userDialogs.ShowLoading("Yükleniyor...");
                await Task.Delay(500);

                await LoadActionModelsAsync();

                CurrentPage.FindByName<BottomSheet>("variantActionsBottomSheet").State = BottomSheetState.HalfExpanded;

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

        private async Task LoadActionModelsAsync()
        {
            try
            {
                IsBusy = true;
                VariantActionModels.Clear();
                var httpClient = _httpClientService.GetOrCreateHttpClient();

                VariantActionModels.Add(new VariantListActionModel
                {
                    ActionName = "Varyant Toplamları",
                    ActionUrl = $"{nameof(ProductDetailVariantTotalListView)}",
                    LineNumber = 1,
                    Icon = "",
                    IsSelected = false
                });

                VariantActionModels.Add(new VariantListActionModel
                {
                    ActionName = "Varyant Detayları",
                    ActionUrl = $"{nameof(ProductDetailVariantDetailListView)}",
                    LineNumber = 2,
                    Icon = "",
                    IsSelected = false
                });
                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                await _userDialogs.AlertAsync(message: ex.Message, title: "Hata");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ActionModelsTappedAsync(VariantListActionModel model)
        {
            try
            {
                IsBusy = true;
                CurrentPage.FindByName<BottomSheet>("variantActionsBottomSheet").State = BottomSheetState.Hidden;
                await Task.Delay(300);
                await Shell.Current.GoToAsync($"{model.ActionUrl}", new Dictionary<string, object>
                {
                    [nameof(VariantModel)] = SelectedVariant
                });

            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata");
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
