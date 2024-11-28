using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    public partial class ReturnSalesDispatchCustomerListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ISalesCustomerService _salesCustomerService;
        private readonly ISalesCustomerProductService _salesCustomerProductService;
        private readonly IUserDialogs _userDialogs;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        SalesCustomer selectedSalesCustomer = null!;


        #region Collections
        public ObservableCollection<SalesCustomer> Items { get; } = new();

        #endregion

        public ReturnSalesDispatchCustomerListViewModel(IHttpClientService httpClientService,
        ISalesCustomerService salesCustomerService,
        IUserDialogs userDialogs,
        ISalesCustomerProductService salesCustomerProductService,
        IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;
            _salesCustomerService = salesCustomerService;
            _userDialogs = userDialogs;
            _salesCustomerProductService = salesCustomerProductService;
            _serviceProvider = serviceProvider;

            Title = "Müşteri Listesi";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<SalesCustomer>(async (customer) => ItemTappedAsync(customer));
            NextViewCommand = new Command(async () => await NextViewAsync());
            ShowOrdersCommand = new Command<SalesCustomer>(async (customer) => await ShowOrdersAsync(customer));
            PerformSearchCommand = new Command(async () => await PerformSearchAsync());
            PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        }

        #region Commands
        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command ShowOrdersCommand { get; }
        public Command NextViewCommand { get; }
        public Command PerformSearchCommand { get; }
        public Command PerformEmptySearchCommand { get; }
        #endregion

        [ObservableProperty]
        public SearchBar searchText;

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading Items...");
                Items.Clear();
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _salesCustomerService.SalesCustomerQueryFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: 0, take: 20, search: SearchText.Text);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

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

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                _userDialogs.Loading("Loading Items...");
                var result = await _salesCustomerService.SalesCustomerQueryFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: Items.Count, take: 20, search: SearchText.Text);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowOrdersAsync(SalesCustomer selectedItem)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var customerOrders = await _salesCustomerProductService.GetObjects(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					customerReferenceId: selectedItem.ReferenceId,
					warehouseNumber: WarehouseModel.Number,
                    search: "",
					skip: 0,
					take: 20
				);

                if (customerOrders.IsSuccess)
                {
                    if (customerOrders.Data is null)
                        return;

                    foreach (var item in customerOrders.Data)
                    {
                        Items.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId)?
                        .Products.Add(Mapping.Mapper.Map<SalesCustomerProduct>(item));

                    }
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ItemTappedAsync(SalesCustomer item)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                if (item == SelectedSalesCustomer)
                {
                    SelectedSalesCustomer.IsSelected = false;
                    SelectedSalesCustomer = null;
                }
                else
                {
                    if (SelectedSalesCustomer != null)
                    {
                        SelectedSalesCustomer.IsSelected = false;
                    }

                    SelectedSalesCustomer = item;
                    SelectedSalesCustomer.IsSelected = true;

                }

            }
            catch (Exception ex)
            {
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

                if (SelectedSalesCustomer is not null)
                {
                    var viewModel = _serviceProvider.GetRequiredService<ReturnSalesDispatchListViewModel>();
                    await viewModel.LoadPageAsync();
                    await Shell.Current.GoToAsync($"{nameof(ReturnSalesDispatchListView)}", new Dictionary<string, object>
                    {
                        [nameof(SalesCustomer)] = SelectedSalesCustomer,
                        [nameof(WarehouseModel)] = WarehouseModel,
                    });
                }
                else
                {
                    _userDialogs.Alert("Lütfen bir müşteri seçiniz.", "Hata", "Tamam");
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
        public async Task LoadPageAsync()
        {
            try
            {


                if (Items?.Count > 0)
                    Items.Clear();
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _salesCustomerService.SalesCustomerQueryFiche(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: 0, take: 20, search: SearchText.Text);
                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                        Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
                }
                if (!result.IsSuccess)
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
    }
}