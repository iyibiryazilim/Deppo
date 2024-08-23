using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
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

namespace Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels
{
    [QueryProperty(name: nameof(Customer), queryId: nameof(Customer))]
    public partial class CustomerInputTransactionViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ICustomQueryService _customQueryService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private CustomerDetailModel customerDetailMode = null!;

        [ObservableProperty]
        private Customer customer = null!;

        public CustomerInputTransactionViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _customQueryService = customQueryService;
            _userDialogs = userDialogs;

            Title = "Müşteri Giriş Hareketleri";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            GoToBackCommand = new Command(async () => await GoToBackAsync());
        }

        public ObservableCollection<CustomerTransaction> Items { get; } = new();

        public Command LoadItemsCommand { get; }
        public Command GoToBackCommand { get; }

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var query = CustomerQuery.InputTransactionListQuery(FirmNumber: _httpClientService.FirmNumber,
                    PeriodNumber: _httpClientService.PeriodNumber,
                    CustomerReferenceId: Customer.ReferenceId
                    );

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
                        Items.Add(Mapping.Mapper.Map<CustomerTransaction>(item));
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