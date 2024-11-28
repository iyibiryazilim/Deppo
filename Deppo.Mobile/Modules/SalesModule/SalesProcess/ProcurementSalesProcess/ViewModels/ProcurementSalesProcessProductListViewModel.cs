using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
[QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
public partial class ProcurementSalesProcessProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementSalesProductService _procurementSalesProductService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel? warehouseModel;

    [ObservableProperty]
    ProcurementSalesCustomerModel procurementSalesCustomerModel;


    [ObservableProperty]
    public SearchBar searchText;

	public ObservableCollection<ProcurementSalesProductModel> Items { get; } = new();

	public ProcurementSalesProcessProductListViewModel(
        IHttpClientService httpClientService,
        IProcurementSalesProductService procurementSalesProductService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementSalesProductService = procurementSalesProductService;
        _userDialogs = userDialogs;

        Title = "Toplanmış Ürünler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());

    }
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        if (ProcurementSalesCustomerModel is null)
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
                search: string.Empty,
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
                search: string.Empty,
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
                search: string.Empty);

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

            await Shell.Current.GoToAsync("..");
		}
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

			_userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
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

            if(Items.Count <= 0)
            {
                await _userDialogs.AlertAsync("Herhangi bir toplanmış ürün bulunmamaktadır", "Bilgilendirme", "Tamam");
                return;
			}

			await Shell.Current.GoToAsync($"{nameof(ProcurementSalesPackageBasketView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(ProcurementSalesCustomerModel)] = ProcurementSalesCustomerModel,
				["CollectedProducts"] = Items
			});
		}
        catch (Exception ex)
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
}
