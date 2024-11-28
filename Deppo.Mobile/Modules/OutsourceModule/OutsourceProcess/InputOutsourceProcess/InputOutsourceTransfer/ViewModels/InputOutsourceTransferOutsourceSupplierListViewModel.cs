using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    public partial class InputOutsourceTransferOutsourceSupplierListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IOutsourceService _outsourceService;
        private readonly IUserDialogs _userDialogs;
        private readonly IShipAddressService _shipAddressService;

        [ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        private ShipAddressModel selectedShipAddressModel;

        [ObservableProperty]
        private OutsourceModel selectedOutsource = null!;

        public InputOutsourceTransferOutsourceSupplierListViewModel(IHttpClientService httpClientService, IOutsourceService outsourceService, IUserDialogs userDialogs, IShipAddressService shipAddressService)
        {
            _httpClientService = httpClientService;
            _outsourceService = outsourceService;
            _userDialogs = userDialogs;
            _shipAddressService = shipAddressService;

            Title = "Fason Cariler";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());

            ItemTappedCommand = new Command<OutsourceModel>(async (outsourceModel) => await ItemTappedAsync(outsourceModel));

            PerformSearchCommand = new Command(async () => await PerformSearchAsync());
            PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
            BackCommand = new Command(async () => await BackAsyc());

            ShipAddressTappedCommand = new Command<ShipAddressModel>(async (shipAddress) => await ShipAddressTappedAsync(shipAddress));
            ConfirmShipAddressCommand = new Command(async () => await ConfirmShipAddressAsync());
            ShipAddressCloseCommand = new Command(async () => await ShipAddressCloseAsync());

            NextViewCommand = new Command(async () => await NextViewAsync());
        }

        public Page CurrentPage { get; set; }
        public ObservableCollection<OutsourceModel> Items { get; } = new();
        public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

        public Command<OutsourceModel> ItemTappedCommand { get; }
        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command PerformSearchCommand { get; }
        public Command PerformEmptySearchCommand { get; }

        public Command ConfirmShipAddressCommand { get; }
        public Command ShipAddressTappedCommand { get; }
        public Command ShipAddressCloseCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }

        [ObservableProperty]
        public SearchBar searchText;

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
                var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, string.Empty, 0, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<OutsourceModel>(item));

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
                var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<OutsourceModel>(item));

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

                Items.Clear();
                _userDialogs.Loading("Searching Items...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await Task.Delay(1000);
                var result = await _outsourceService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SearchText.Text, Items.Count, 20);
                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<OutsourceModel>(item));

                    _userDialogs.Loading().Hide();
                }
                else
                {
                    if (_userDialogs.IsHudShowing)
                        _userDialogs.Loading().Hide();

                    _userDialogs.Alert(message: result.Message, title: "Load Items");
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

        private async Task ItemTappedAsync(OutsourceModel outsourceModel)
        {
            if (outsourceModel is null)
                return;
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                // Eğer outsourceModel zaten seçilmediyse, önce eski seçili öğeyi sıfırla
                if (!outsourceModel.IsSelected)
                {
                    // Daha önce seçilmiş olan varsa, seçimini kaldır
                    if (SelectedOutsource != null)
                    {
                        SelectedOutsource.IsSelected = false;
                    }

                    // Yeni seçilen öğeyi true yap
                    if (outsourceModel.ShipAddressCount > 0)
                    {
                        await LoadShipAddressesAsync(outsourceModel);
                        CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.HalfExpanded;
                    }
                    else
                    {
                        outsourceModel.IsSelected = true;
                        SelectedOutsource = outsourceModel;
                    }
                }
                else
                {
                    // Eğer zaten seçiliyse seçimi kaldır
                    outsourceModel.IsSelected = false;
                    SelectedOutsource = null;
                }
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

        private async Task LoadShipAddressesAsync(OutsourceModel outsourceModel)
        {
            try
            {
                ShipAddresses.Clear();
                _userDialogs.Loading("Loading Ship Addresses...");
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _shipAddressService.GetObjectsByOrder(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    currentReferenceId: outsourceModel.ReferenceId,
                    search: "",
                    skip: 0,
                    take: 99999
                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        ShipAddresses.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
                }

                _userDialogs.HideHud();
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

        private async Task ShipAddressTappedAsync(ShipAddressModel shipAddressModel)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                SelectedShipAddressModel = shipAddressModel;
                if (shipAddressModel.IsSelected)
                {
                    SelectedShipAddressModel.IsSelected = false;
                    SelectedShipAddressModel = null;
                }
                else
                {
                    ShipAddresses.ToList().ForEach(x => x.IsSelected = false);
                    SelectedShipAddressModel = shipAddressModel;
                    SelectedShipAddressModel.IsSelected = true;
                }
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

        private async Task ConfirmShipAddressAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var selectedShipAddress = ShipAddresses.FirstOrDefault(x => x.IsSelected);
                if (selectedShipAddress is not null)
                {
                    //PurchaseSupplier.ShipAddressReferenceId = selectedShipAddress.ReferenceId;
                    //PurchaseSupplier.ShipAddressCode = selectedShipAddress.Code;
                    //PurchaseSupplier.ShipAddressName = selectedShipAddress.Name;

                    Items.ToList().ForEach(x => x.IsSelected = false);

                    //PurchaseSupplier.IsSelected = true;

                    CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.Hidden;
                }
                else
                {
                    _userDialogs.Alert("Lütfen bir adres seçiniz.", "Hata", "Tamam");
                }
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

        private async Task ShipAddressCloseAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentPage.FindByName<BottomSheet>("shipAddressBottomSheet").State = BottomSheetState.Hidden;
            });
        }

        private async Task BackAsyc()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var confirm = await _userDialogs.ConfirmAsync("Verileriniz silinecektir! Devam etmek istediğinize emin misiniz?", "İptal", "Evet", "Hayır");
                if (!confirm)
                    return;

                Items.Clear();
                ShipAddresses.Clear();
                //if (PurchaseSupplier is not null)
                //{
                //    PurchaseSupplier.ShipAddressReferenceId = 0;
                //    PurchaseSupplier.ShipAddressCode = string.Empty;
                //    PurchaseSupplier.ShipAddressName = string.Empty;
                //    PurchaseSupplier.IsSelected = false;
                //}
                //PurchaseSupplier = null;

                Items.ForEach(x => x.IsSelected = false);

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

        private async Task NextViewAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferOutsourceBasketListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(OutsourceModel)] = SelectedOutsource
                });
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
}