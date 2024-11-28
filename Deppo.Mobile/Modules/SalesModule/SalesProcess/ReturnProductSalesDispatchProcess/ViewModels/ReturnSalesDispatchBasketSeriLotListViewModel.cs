using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MessageHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(ReturnSalesBasketModel), queryId: nameof(ReturnSalesBasketModel))]
public partial class ReturnSalesDispatchBasketSeriLotListViewModel : BaseViewModel
{

    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotService _SeriLotService; //serilotservice
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;

    public ReturnSalesDispatchBasketSeriLotListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ISeriLotService SeriLotService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _SeriLotService = SeriLotService;
        _serviceProvider = serviceProvider;

        LoadSelectedItemsCommand = new Command(async () => await LoadSelectedItemsAsync());
        ShowSeriLotsCommand = new Command(async () => await ShowSeriLotsAsync());
        PerformSearchCommand = new Command<Entry>(async (searchBar) => await PerformSearchAsync(searchBar));
        IncreaseCommand = new Command<SeriLotModel>(async (SeriLotModel) => await IncreaseAsync(SeriLotModel));
        DecreaseCommand = new Command<SeriLotModel>(async (SeriLotModel) => await DecreaseAsync(SeriLotModel));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        CancelCommand = new Command(async () => await CancelAsync());
        Title = "Sepet Listesi";

        CloseSeriLotsCommand = new Command(async () => await CloseSeriLotsAsync());
        ItemTappedCommand = new Command<SeriLotModel>(async (SeriLotModel) => await ItemTappedAsync(SeriLotModel));
        ConfirmSeriLotsCommand = new Command(async () => await ConfirmSeriLotsAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
    }
    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private ReturnSalesBasketModel returnSalesBasketModel = null!;

    public ObservableCollection<SeriLotModel> Items { get; } = new(); // serilotmodel
    public ObservableCollection<SeriLotModel> SelectedItems { get; } = new();

    [ObservableProperty]
    private SeriLotModel selectedItem; //Serilotmodel

    public Page CurrentPage { get; set; }

    public Command LoadSelectedItemsCommand { get; }
    public Command<SeriLotModel> IncreaseCommand { get; }
    public Command<SeriLotModel> DecreaseCommand { get; }
    public Command<Entry> PerformSearchCommand { get; }
    public Command ConfirmCommand { get; }
    public Command CancelCommand { get; }

    #region SeriLot BottomSheet Commands

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ShowSeriLotsCommand { get; }
    public Command CloseSeriLotsCommand { get; }
    public Command<SeriLotModel> ItemTappedCommand { get; }
    public Command<SearchBar> SeriLotsPerformSearchCommand { get; }
    public Command ConfirmSeriLotsCommand { get; }
    #endregion
    public async Task LoadSelectedItemsAsync()
    {
        //if (IsBusy)
        //    return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            await Task.Delay(500);

            SelectedItems.Clear();
            if (returnSalesBasketModel.Details.Count > 0)
            {
                foreach (var item in returnSalesBasketModel.Details)
                    SelectedItems.Add(new SeriLotModel
                    {
                        Code = item.SeriLotCode,
                        Name = item.SeriLotName,
                        StockQuantity = default,
                        InputQuantity = item.Quantity
                    });
            }

            _userDialogs.HideHud();
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

    private async Task ShowSeriLotsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            await LoadItemsAsync();
            CurrentPage.FindByName<BottomSheet>("SeriLotBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Items.Clear();

            _userDialogs.ShowLoading("Loading...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _SeriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, returnSalesBasketModel.ItemReferenceId, string.Empty, 0, 20);

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<SeriLotModel>(item));
                }
            }

            _userDialogs.HideHud();
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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _SeriLotService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                //    productReferenceId: returnSalesBasketModel.ItemReferenceId,
                skip: Items.Count,
                take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<SeriLotModel>(item));
                }
            }

            _userDialogs.HideHud();
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

    private async Task SeriLotsPerformSearchAsync(SearchBar searchText)
    {
        try
        {
            if (!string.IsNullOrEmpty(searchText.Text))
                return;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _SeriLotService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                // productReferenceId: returnSalesBasketModel.ItemReferenceId,
                search: searchText.Text,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CloseSeriLotsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            await Task.Delay(500);
            CurrentPage.FindByName<BottomSheet>("SeriLotBottomSheet").State = BottomSheetState.Hidden;
            _userDialogs.HideHud();
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

    private async Task ItemTappedAsync(SeriLotModel SeriLotModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SeriLotModel.IsSelected)
            {
                Items.FirstOrDefault(x => x.ReferenceId == SeriLotModel.ReferenceId).IsSelected = false;
            }
            else
            {
                Items.FirstOrDefault(x => x.ReferenceId == SeriLotModel.ReferenceId).IsSelected = true;
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

    private async Task ConfirmSeriLotsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            await Task.Delay(500);
            foreach (var item in Items.Where(x => x.IsSelected))
            {
                if (!SelectedItems.Any(x => x.Code == item.Code))
                    SelectedItems.Add(item);
            }

            CurrentPage.FindByName<BottomSheet>("SeriLotBottomSheet").State = BottomSheetState.Hidden;
            _userDialogs.HideHud();
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

    private async Task IncreaseAsync(SeriLotModel SeriLotModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedItem = SeriLotModel;

            if (returnSalesBasketModel.TrackingType != 0)
            {//InputProductProcessBasketSeriLotListView
                await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketSeriLotListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(returnSalesBasketModel)] = returnSalesBasketModel
                });
            }
            else
            {
                SeriLotModel.InputQuantity++;
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

    private async Task DecreaseAsync(SeriLotModel SeriLotModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (SeriLotModel.InputQuantity > 0)
            {
                SelectedItem = SeriLotModel;

                if (returnSalesBasketModel.TrackingType != 0)
                {//InputProductProcessBasketSeriLotListView
                    await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchBasketSeriLotListView)}", new Dictionary<string, object>
                    {
                        [nameof(WarehouseModel)] = WarehouseModel,
                        [nameof(returnSalesBasketModel)] = returnSalesBasketModel
                    });
                }
                else
                {
                    SeriLotModel.InputQuantity--;
                }
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

    private async Task PerformSearchAsync(Entry barcodeEntry)
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrEmpty(barcodeEntry.Text))
                return;

            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            await Task.Delay(500);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _SeriLotService.GetObjects(
                        httpClient: httpClient,
                        firmNumber: _httpClientService.FirmNumber,
                        periodNumber: _httpClientService.PeriodNumber,
                        warehouseNumber: WarehouseModel.Number,
                        //productReferenceId: returnSalesBasketModel.ItemReferenceId,
                        search: barcodeEntry.Text,
                        skip: 0,
                        take: 1);

            if (result.IsSuccess)
            {
                if (!(result.Data.Count() > 0))
                {
                    new MessageHelper().GetToastMessage(message: $"{barcodeEntry.Text} kodlu raf bulunamad�.");
                    return;
                }

                foreach (var item in result.Data)
                    SelectedItems.Add(Mapping.Mapper.Map<SeriLotModel>(item));

                barcodeEntry.Text = string.Empty;
                barcodeEntry.Focus();
            }

            _userDialogs.HideHud();
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


    private async Task ConfirmAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");

            var previousViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();

            if (previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == returnSalesBasketModel.ItemReferenceId) is not null)
            {
                foreach (var item in SelectedItems.Where(x => x.InputQuantity > 0)) //SeriLots
                {
                    var SeriLot = previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == returnSalesBasketModel.ItemReferenceId).Details.FirstOrDefault(x => x.SeriLotCode == item.Code);
                    if (SeriLot is not null)
                    {
                        SeriLot.Quantity = item.InputQuantity;
                    }
                    else
                    {
                        previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == returnSalesBasketModel.ItemReferenceId).Details.Add(new ReturnSalesBasketDetailModel
                        {
                            SeriLotReferenceId = item.ReferenceId,
                            SeriLotCode = item.Code,
                            SeriLotName = item.Name,
                            Quantity = item.InputQuantity
                        });


                    }

                    var totalInputQuantity = SelectedItems.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);
                    previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == returnSalesBasketModel.ItemReferenceId).Quantity = totalInputQuantity;
                }


            }

            await Shell.Current.GoToAsync("..");
            _userDialogs.HideHud();
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

    private async Task CancelAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            await Task.Delay(500);
            await Shell.Current.GoToAsync("..");
            _userDialogs.HideHud();
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