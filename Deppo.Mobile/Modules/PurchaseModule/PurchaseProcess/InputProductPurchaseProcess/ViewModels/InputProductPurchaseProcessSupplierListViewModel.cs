using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(InputProductProcessType), queryId: nameof(InputProductProcessType))]
public partial class InputProductPurchaseProcessSupplierListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISupplierService _supplierService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SupplierModel supplierModel = null!;


    [ObservableProperty]
    private Supplier selectedSupplier = null!;

    [ObservableProperty]
    private InputProductProcessType inputProductProcessType;

    public InputProductPurchaseProcessSupplierListViewModel(IHttpClientService httpClientService,
        ISupplierService supplierService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _supplierService = supplierService;
        _userDialogs = userDialogs;

        Title = "Tedarikçiler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        PerformSearchCommand = new Command<SearchBar>(async (searchBar) => await PerformSearchAsync(searchBar));

        ItemTappedCommand = new Command<SupplierModel>(async (supplier) => await ItemTappedAsync(supplier));
        NextViewCommand = new Command(async () => await NextViewAsync());
    }

    public ObservableCollection<SupplierModel> Items { get; } = new();

    // public ObservableCollection<SupplierModel> SelectedItems { get; } = new();
    public Command LoadItemsCommand { get; }

    public Command LoadMoreItemsCommand { get; }
    public Command<SearchBar> PerformSearchCommand { get; }

    public Command<SupplierModel> ItemTappedCommand { get; }

    public Command NextViewCommand { get; }

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
            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SupplierModel>(item));

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

    public async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.Loading("Refreshing Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<SupplierModel>(item));

                if (_userDialogs.IsHudShowing)
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

    private async Task PerformSearchAsync(SearchBar searchBar)
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                await LoadItemsAsync();
                searchBar.Unfocus();
                return;
            }
            else
            {
                if (searchBar.Text.Length >= 3)
                {
                    IsBusy = true;
                    using (_userDialogs.Loading("Searching.."))
                    {
                        var httpClient = _httpClientService.GetOrCreateHttpClient();

                        var result = await _supplierService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, Items.Count, 20);
                        if (!result.IsSuccess)
                        {
                            _userDialogs.Alert(result.Message, "Hata");
                            return;
                        }

                        Items.Clear();
                        foreach (var item in result.Data)
                            Items.Add(item);
                    }
                }
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

    private async Task ItemTappedAsync(SupplierModel supplier)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (supplier.IsSelected)
            {
                supplier.IsSelected = false;
                SelectedSupplier = null;
            }
            else
            {
                Items.ToList().ForEach(x => x.IsSelected = false);

                var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == supplier.ReferenceId);
                if (selectedItem != null)
                    selectedItem.IsSelected = true;

                SelectedSupplier = supplier;
            }
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (WarehouseModel is not null)
            {
                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessBasketListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                   
                    [nameof(SelectedSupplier)] = SelectedSupplier,
                    [nameof(InputProductProcessType)] = InputProductProcessType
                });
            }
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