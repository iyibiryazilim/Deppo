using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ProcurementSalesProcessCustomerListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IProcurementSalesCustomerService _procurementSalesCustomerService;
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    WarehouseModel? warehouseModel;

    [ObservableProperty]
    ProcurementSalesCustomerModel? selectedCustomerModel;

	[ObservableProperty]
	public SearchBar searchText;

	public ObservableCollection<ProcurementSalesCustomerModel> Items { get; } = new();

    [ObservableProperty]
    ProcurementSalesCustomerModel selectedSearhItem;

    public ProcurementSalesProcessCustomerListViewModel(
        IHttpClientService httpClientService,
        IProcurementSalesCustomerService procurementSalesCustomerService,
        IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _procurementSalesCustomerService = procurementSalesCustomerService;
        _userDialogs = userDialogs;

        Title = "Ambar Transferi Olan Müşteriler";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProcurementSalesCustomerModel>(async (item) => await ItemTappedAsync(item));
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsyc());

    }

    public Page CurrentPage { get; set; }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command<ProcurementSalesCustomerModel> ItemTappedCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }


    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        if (WarehouseModel is null)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");
            Items.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementSalesCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: 0,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
					foreach (var item in result.Data)
					{
						var customer = Mapping.Mapper.Map<ProcurementSalesCustomerModel>(item);
						if (SelectedSearhItem is not null && SelectedSearhItem.ReferenceId == customer.ReferenceId)
						{
							customer.IsSelected = SelectedSearhItem.IsSelected;
						}
						else
						{
							customer.IsSelected = false;

						}
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
        if (WarehouseModel is null)
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Yükleniyor...");

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _procurementSalesCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                search: SearchText.Text,
                skip: Items.Count,
                take: 20
            );

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                    foreach (var item in result.Data)
                    {
                        var customer = Mapping.Mapper.Map<ProcurementSalesCustomerModel>(item);
                        if(SelectedSearhItem is not null && SelectedSearhItem.ReferenceId == customer.ReferenceId)
						{
							customer.IsSelected = SelectedSearhItem.IsSelected;
                        }
                        else
                        {
                            customer.IsSelected = false;

                        }
						Items.Add(customer);
                    }
            }

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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

    private async Task ItemTappedAsync(ProcurementSalesCustomerModel item)
    {
        if (item is null)
            return;
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item == SelectedCustomerModel)
            {
                SelectedCustomerModel.IsSelected = false;
                SelectedCustomerModel = null;
                SelectedSearhItem.IsSelected = false;
				SelectedSearhItem = null;
			}
            else
            {
                if (SelectedCustomerModel != null)
                {
                    SelectedCustomerModel.IsSelected = false;
                }

                SelectedCustomerModel = item;
                SelectedCustomerModel.IsSelected = true;
				SelectedSearhItem = item;
				SelectedSearhItem.IsSelected = true;
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

    private async Task PerformSearchAsync()
    {
        if (IsBusy)
            return;
        if (WarehouseModel is null)
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

            var result = await _procurementSalesCustomerService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: WarehouseModel.Number,
                skip: 0,
                take: 20,
                search: SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var customer = Mapping.Mapper.Map<ProcurementSalesCustomerModel>(item);
					if (SelectedSearhItem is not null && SelectedSearhItem.ReferenceId == customer.ReferenceId)
					{
						customer.IsSelected = SelectedSearhItem.IsSelected;
					}
					else
					{
						customer.IsSelected = false;

					}
					Items.Add(customer);

				}
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

    private async Task BackAsyc()
    {
        if (IsBusy)
            return;
        try
        {
			if(SelectedCustomerModel is not null)
            {
                SelectedCustomerModel.IsSelected = false;
				SelectedCustomerModel = null;
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;
        if (WarehouseModel is null || SelectedCustomerModel is null)
            return;

        try
        {
            IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ProcurementSalesProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(ProcurementSalesCustomerModel)] = SelectedCustomerModel
			});
		}
        catch (Exception ex)
        {
            if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
        finally
        {
            IsBusy = false;
		}
    }
}
