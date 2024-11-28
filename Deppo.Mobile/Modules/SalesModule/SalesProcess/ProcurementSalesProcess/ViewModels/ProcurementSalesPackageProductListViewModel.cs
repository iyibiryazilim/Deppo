using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PackageProductModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

public partial class ProcurementSalesPackageProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPackageProductService _packageProductService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private PackageProductModel? selectedProduct;

    [ObservableProperty]
    private ObservableCollection<ProcurementPackageBasketModel> selectedProducts = new();

    public ObservableCollection<PackageProductModel> Items { get; } = new();
    public ObservableCollection<PackageProductModel> SelectedItems { get; } = new();

    public ProcurementSalesPackageProductListViewModel(
        IHttpClientService httpClientService,
        IPackageProductService packageProductService,
        IUserDialogs userDialogs,
        IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _packageProductService = packageProductService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;

        Title = "Koli Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PackageProductModel>(async (parameter) => await ItemTappedAsync(parameter));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<PackageProductModel> ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
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
           

            _userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _packageProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<PackageProductModel>(product);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                    if (matchingItem != null)
                        item.IsSelected = matchingItem.IsSelected;
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
            var result = await _packageProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<PackageProductModel>(product);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                    if (matchingItem != null)
                        item.IsSelected = matchingItem.IsSelected;
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

    private async Task ItemTappedAsync(PackageProductModel packageProductModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            PackageProductModel item = Items.FirstOrDefault(x => x.ReferenceId == packageProductModel.ReferenceId);

            if (item is not null)
            {
                if (!item.IsSelected)
                {

                    var tappedItem = Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                    if (tappedItem != null)
                        tappedItem.IsSelected = true;

                    SelectedProduct = item;

                    var basketItem = new ProcurementPackageBasketModel
                    {
                        PackageProductModel = item,
                    };
                   

                    SelectedProducts.Add(basketItem);
                    SelectedItems.Add(item);

                }
                else
                {
                    SelectedProduct = null;
                    var selectedItem = SelectedProducts.FirstOrDefault(x => x.PackageProductModel.ReferenceId == item.ReferenceId);
                    if (selectedItem != null)
                    {
                        SelectedProducts.Remove(selectedItem);
                        Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId).IsSelected = false;
                        SelectedItems.Remove(item);
                    }
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

            var previouseViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
            if (previouseViewModel is not null)
            {
                foreach (var item in SelectedProducts)
                {
                    
                    previouseViewModel.Items.Add(item);
                    
                }

                await Shell.Current.GoToAsync($"..");
            }
            SelectedItems.Clear();
            SelectedProducts.Clear();
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

            if(SelectedItems.Count > 0)
            {
                SelectedItems.Clear();
                SelectedProducts.Clear();
            }

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

            _userDialogs.Loading("Searching Items...");
            Items.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _packageProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, 0, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<PackageProductModel>(product);
                    var matchingItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                    if (matchingItem != null)
                        item.IsSelected = matchingItem.IsSelected;
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
}
