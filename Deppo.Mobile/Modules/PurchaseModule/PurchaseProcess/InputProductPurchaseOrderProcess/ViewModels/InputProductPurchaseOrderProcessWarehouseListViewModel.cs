using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels
{
    [QueryProperty(name: nameof(InputProductProcessType), queryId: nameof(InputProductProcessType))]
    public partial class InputProductPurchaseOrderProcessWarehouseListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private WarehouseModel selectedWarehouseModel = null!;

        [ObservableProperty]
        private InputProductProcessType inputProductProcessType;

		public ObservableCollection<WarehouseModel> Items { get; } = new();

		public InputProductPurchaseOrderProcessWarehouseListViewModel(IHttpClientService httpClientService,
        IWarehouseService warehouseService,
        IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _warehouseService = warehouseService;
            _userDialogs = userDialogs;

            Title = "Ambar Seçimi";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
            NextViewCommand = new Command(async () => await NextViewAsync());
        }

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command NextViewCommand { get; }

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                Items.Clear();
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, 0, 20, _httpClientService.FirmNumber);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                            Items.Add(new WarehouseModel
                            {
                                ReferenceId = item.ReferenceId,
                                Name = item.Name,
                                Number = item.Number,
                                City = item.City,
                                Country = item.Country,
                                IsSelected = false
                            });
                    }
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
			if (Items.Count < 18)
				return;
			if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, Items.Count, 20, _httpClientService.FirmNumber);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
						_userDialogs.ShowLoading("Loading...");
						foreach (var item in result.Data)
                            Items.Add(new WarehouseModel
                            {
                                ReferenceId = item.ReferenceId,
                                Name = item.Name,
                                Number = item.Number,
                                City = item.City,
                                Country = item.Country,
                                IsSelected = false
                            });
                    }
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

        private void ItemTappedAsync(WarehouseModel item)
        {
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				if (item == SelectedWarehouseModel)
				{
					SelectedWarehouseModel.IsSelected = false;
					SelectedWarehouseModel = null;
				}
				else
				{
					if (SelectedWarehouseModel != null)
					{
						SelectedWarehouseModel.IsSelected = false;
					}

					SelectedWarehouseModel = item;
					SelectedWarehouseModel.IsSelected = true;
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

                if (SelectedWarehouseModel is not null)
                {
                    await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessSupplierListView)}", new Dictionary<string, object>
                    {
                        [nameof(WarehouseModel)] = SelectedWarehouseModel,
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
}