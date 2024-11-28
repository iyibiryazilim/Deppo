using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.ViewModels;

public partial class OutsourcePanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IOutsourcePanelService _outsourcePanelService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private OutsourcePanelModel _outsourcePanelModel = new();

    [ObservableProperty]
    private OutsourceFiche selectedOutsourceFiche;

    public OutsourcePanelViewModel(IHttpClientService httpClientService, IOutsourcePanelService outsourcePanelService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _outsourcePanelService = outsourcePanelService;
        _userDialogs = userDialogs;

        Title = "Fason Paneli";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        ItemTappedCommand = new Command<OutsourceFiche>(async (outsourceFiche) => await ItemTappedAsync(outsourceFiche));
        GoToSupplierDetailCommand = new Command<OutsourceModel>(async (supplier) => await GoToSupplierDetailAsync(supplier));
        AddAllFicheCommand = new Command(async () => await AddAllFicheAsync());
    }

    public Page CurrentPage { get; set; }
    public Command LoadItemsCommand { get; }
    public Command GoToSupplierDetailCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command AddAllFicheCommand { get; }

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            OutsourcePanelModel.Outsources.Clear();
            OutsourcePanelModel.LastOutsourceFiche.Clear();

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);

            await Task.WhenAll(
                GetOutsourceOutProductCountAsync(),
                GetOutsourceInProductCountAsync(),
                GetOutsourceTotalProductCountAsync()
            ).ContinueWith(async _ =>
            {
                await GetLastOutsourcesAsync();
                await GetLastOutsourceFichesAsync();
                OutsourcePanelModel.InProductCountTotalRate = (double)((double)OutsourcePanelModel.InProductCount / (double)OutsourcePanelModel.TotalProductCount);
                OutsourcePanelModel.OutProductCountTotalRate = (double)((double)OutsourcePanelModel.OutProductCount / (double)OutsourcePanelModel.TotalProductCount);
            });

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

    private async Task ItemTappedAsync(OutsourceFiche outsourceFiche)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            await Task.Delay(1000);
            await GetLastOutsourceTransactionsAsync(outsourceFiche);

            CurrentPage.FindByName<BottomSheet>("ficheTransactionBottomSheet").State = BottomSheetState.HalfExpanded;

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

    private async Task GetLastOutsourceTransactionsAsync(OutsourceFiche outsourceFiche)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetLastOutsourceTransactions(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ficheReferenceId: outsourceFiche.ReferenceId
            );
            SelectedOutsourceFiche = outsourceFiche;

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourceTransaction>(item);
                        OutsourcePanelModel.LastOutsourceTransaction.Add(obj);
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

    private async Task GetOutsourceOutProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetOutsourceOutProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourcePanelModel>(item);
                        OutsourcePanelModel.OutProductCount = obj.OutProductCount;
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

    private async Task GetOutsourceInProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetOutsourceInProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourcePanelModel>(item);
                        OutsourcePanelModel.InProductCount = Convert.ToInt32(obj.InProductCount);
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

    private async Task GetOutsourceTotalProductCountAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetOutsourceTotalProductCount(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourcePanelModel>(item);
                        OutsourcePanelModel.TotalProductCount = obj.TotalProductCount;
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

    private async Task GetLastOutsourcesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetLastOutsources(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourceModel>(item);
                        OutsourcePanelModel.Outsources.Add(obj);
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

    private async Task GetLastOutsourceFichesAsync()
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _outsourcePanelService.GetLastOutsourceFiches(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<OutsourceFiche>(item);
                        OutsourcePanelModel.LastOutsourceFiche.Add(obj);
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

    private async Task GoToSupplierDetailAsync(OutsourceModel outsourceModel)
    {
        if (outsourceModel is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            SupplierDetailModel supplierDetailModel = new();

            Supplier newSupplier = new();
            newSupplier.ReferenceId = outsourceModel.ReferenceId;
            newSupplier.Name = outsourceModel.Name;
            newSupplier.FirstName = outsourceModel.FirstName;
            newSupplier.LastName = outsourceModel.LastName;
            newSupplier.Address = outsourceModel.Address;
            newSupplier.City = outsourceModel.City;
            newSupplier.Email = outsourceModel.Email;
            newSupplier.Code = outsourceModel.Code;
            newSupplier.Country = outsourceModel.Country;
            newSupplier.IsActive = outsourceModel.IsActive;
            newSupplier.IsPersonal = outsourceModel.IsPersonal;
            newSupplier.PostalCode = outsourceModel.PostalCode;
            newSupplier.TaxNumber = outsourceModel.TaxNumber;
            newSupplier.TaxOffice = outsourceModel.TaxOffice;
            newSupplier.Tckn = outsourceModel.Tckn;
            newSupplier.Telephone = outsourceModel.Telephone;

            supplierDetailModel.Supplier = newSupplier;

            await Shell.Current.GoToAsync($"{nameof(SupplierDetailView)}", new Dictionary<string, object>
            {
                [nameof(SupplierDetailModel)] = supplierDetailModel
            });
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddAllFicheAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(OutsourcePanelAllFicheListView)}");
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