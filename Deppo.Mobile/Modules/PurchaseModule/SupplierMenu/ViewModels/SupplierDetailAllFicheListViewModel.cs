using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

[QueryProperty(name: nameof(SupplierDetailModel), queryId: nameof(SupplierDetailModel))]
public partial class SupplierDetailAllFicheListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISupplierDetailAllFicheService _supplierDetailAllFicheService;
    private readonly ISupplierTransactionService _supplierTransactionService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    public SupplierDetailModel supplierDetailModel;

    public ObservableCollection<PurchaseFicheModel> Items { get; } = new();

    [ObservableProperty]
    public PurchaseFicheModel selectedItem;

    public SupplierDetailAllFicheListViewModel(
        IHttpClientService httpClientService,
        IUserDialogs userDialogs,
        ISupplierTransactionService supplierTransactionService,
        ISupplierDetailAllFicheService supplierDetailAllFicheService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _supplierTransactionService = supplierTransactionService;
        _supplierDetailAllFicheService = supplierDetailAllFicheService;

        Title = "Tedarikçi Hareketleri";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<PurchaseFicheModel>(async (item) => await ItemTappedAsync(item));
        BackCommand = new Command(async () => await BackAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command BackCommand { get; }

    public async Task LoadItemsAsync()
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
            var result = await _supplierDetailAllFicheService.GetAllFichesBySupplier(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber,
              SupplierDetailModel.Supplier.ReferenceId, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<PurchaseFicheModel>(item));

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

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

            _userDialogs.Loading("Loading More Items...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _supplierDetailAllFicheService.GetAllFichesBySupplier(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SupplierDetailModel.Supplier.ReferenceId, string.Empty, Items.Count, 20);
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                    Items.Add(Mapping.Mapper.Map<PurchaseFicheModel>(item));

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

    public async Task ItemTappedAsync(PurchaseFicheModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            await LoadFicheTransactionsAsync(item);
            await Task.Delay(500);
            CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadFicheTransactionsAsync(PurchaseFicheModel purchaseFicheModel)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            SupplierDetailModel.Transactions.Clear();

            var result = await _supplierDetailAllFicheService.GetTransactionsByFiche(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ficheReferenceId: purchaseFicheModel.ReferenceId
                );
            SelectedItem = purchaseFicheModel;

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SupplierDetailModel.Transactions.Add(Mapping.Mapper.Map<SupplierTransaction>(item));
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
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
}