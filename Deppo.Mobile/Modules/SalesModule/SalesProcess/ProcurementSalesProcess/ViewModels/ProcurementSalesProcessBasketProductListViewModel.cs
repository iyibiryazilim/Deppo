using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PackageProductModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

[QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
public partial class ProcurementSalesProcessBasketProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementSalesProductService _procurementSalesProductService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    ProcurementSalesCustomerModel procurementSalesCustomerModel;

    public ProcurementSalesProcessBasketProductListViewModel(
        IHttpClientService httpClientService,
        IProcurementSalesProductService procurementSalesProductService,
        IUserDialogs userDialogs,
        IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _procurementSalesProductService = procurementSalesProductService;
        _userDialogs = userDialogs;
        _serviceProvider = serviceProvider;

        Title = "Koliye Eklenecek Ürünleri Seçiniz";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        BackCommand = new Command(async () => await BackAsync());
        ItemTappedCommand = new Command<ProcurementSalesProductModel>(async (item) => await ItemTappedAsync(item));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
    }

    [ObservableProperty]
    ProcurementSalesProductModel selectedProduct;
    public ObservableCollection<ProcurementSalesProductModel> Items { get; } = new();
    public ObservableCollection<ProcurementSalesProductModel> SelectedProducts { get; } = new();
    public ObservableCollection<ProcurementSalesProductModel> SelectedItems { get; } = new();

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command ConfirmCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command BackCommand { get; }

    public SearchBar SearchText { get; set; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementSalesProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: ProcurementSalesCustomerModel.ReferenceId,
                search: SearchText.Text,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customer = Mapping.Mapper.Map<ProcurementSalesProductModel>(item);
                        Items.Add(customer);
                    }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMoreItemsAsync()
    {
        if (Items.Count < 18)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Yükleniyor...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementSalesProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: ProcurementSalesCustomerModel.ReferenceId,
                search: SearchText.Text,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customer = Mapping.Mapper.Map<ProcurementSalesProductModel>(item);
                        Items.Add(customer);
                    }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (System.Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

        }
        finally
        {
            IsBusy = false;
        }
    }


    private async Task ItemTappedAsync(ProcurementSalesProductModel procurementSalesProductModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            ProcurementSalesProductModel item = Items.FirstOrDefault(x => x.ReferenceId == procurementSalesProductModel.ReferenceId);

            if (item is not null)
            {
                if (!item.IsSelected)
                {

                    var tappedItem = Items.ToList().FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                    if (tappedItem != null)
                        tappedItem.IsSelected = true;

                    SelectedProduct = item;

                    SelectedProducts.Add(item);
                    SelectedItems.Add(item);

                }
                else
                {
                    SelectedProduct = null;
                    var selectedItem = SelectedProducts.FirstOrDefault(x => x.ReferenceId== item.ReferenceId);
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

            var previouseViewModel = _serviceProvider.GetRequiredService<ProcurementSalesProcessProductBasketViewModel>();
            if (previouseViewModel is not null)
            {
                foreach (var item in SelectedProducts)
                    if (!previouseViewModel.Items.Any(x => x.ReferenceId== item.ReferenceId))
                        previouseViewModel.Items.Add(item);
            }
            SelectedItems.Clear();
            SelectedProducts.Clear();

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

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            Items.Clear();
            var result = await _procurementSalesProductService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: ProcurementSalesCustomerModel.ReferenceId,
                skip: 0,
                take: 20,
                search: SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<ProcurementSalesProductModel>(item));
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(SelectedProducts.Count > 0)
            {
                var confirm = await _userDialogs.ConfirmAsync("Seçili ürünler silinecek. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
					if (!confirm)
						return;
				}

				SelectedProducts.Clear();
                SelectedItems.Clear();

				await Shell.Current.GoToAsync("..");

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
