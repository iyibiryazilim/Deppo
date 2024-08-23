using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.QueryHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels
{
    [QueryProperty(name: nameof(Supplier), queryId: nameof(Supplier))]
    public partial class SupplierOutputTransactionViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ICustomQueryService _customQueryService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private SupplierDetailModel supplierDetailModel = null!;

        [ObservableProperty]
        private Supplier supplier = null!;

        public SupplierOutputTransactionViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _customQueryService = customQueryService;
            _userDialogs = userDialogs;
            Title = "Tedarikçi Çıkış Hareketleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            GoToBackCommand = new Command(async () => await GoToBackAsync());
        }

        public ObservableCollection<SupplierTransaction> Items { get; } = new();

        public Command LoadItemsCommand { get; }

        public Command LoadMoreItemsCommand { get; }
        public Command GoToBackCommand { get; }

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var query = SupplierQuery.OutputTransactionListQuery(FirmNumber: _httpClientService.FirmNumber, PeriodNumber: _httpClientService.FirmNumber, SupplierReferenceId: Supplier.ReferenceId);

                Items.Clear();

                _userDialogs.Loading("Loading Items...");
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _customQueryService.GetObjectsAsync(httpClient, query);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;
                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<SupplierTransaction>(item));
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

        private async Task LoadMoreItemsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var query = SupplierQuery.OutputTransactionListQuery(
                    FirmNumber: _httpClientService.FirmNumber,
                    PeriodNumber: _httpClientService.PeriodNumber,
                    SupplierReferenceId: Supplier.ReferenceId,
                    Sorting: "DESC",
                    Skip: Items.Count,
                    Take: 20

                    );

                _userDialogs.Loading("Loading More Items...");
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _customQueryService.GetObjectsAsync(httpClient, query);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;
                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<SupplierTransaction>(item));
                    }
                    _userDialogs.Loading().Hide();
                }
                else
                {
                    if (_userDialogs.IsHudShowing)
                        _userDialogs.Loading().Hide();

                    _userDialogs.Alert(message: result.Message, title: "Load More Items");
                }
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Load More Items Error");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GoToBackAsync()
        {
            try
            {
                IsBusy = true;

                await Task.Delay(300);
                await Shell.Current.GoToAsync("..");
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