using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using DevExpress.Maui.Controls;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using System.Collections.ObjectModel;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels
{
	[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
	public partial class ProcurementByLocationWarehouseLocationListViewModel : BaseViewModel
	{
		private readonly IHttpClientService _httpClientService;
		private readonly IProcurementByLocationService _procurementByLocationService;
		private readonly IUserDialogs _userDialogs;

		[ObservableProperty]
		private LocationModel selectedLocation = null!;


		[ObservableProperty]
		private WarehouseModel warehouseModel = null!;

		public ObservableCollection<LocationModel> Items { get; } = new();


		public ProcurementByLocationWarehouseLocationListViewModel(IHttpClientService httpClientService,
		IUserDialogs userDialogs,
		IProcurementByLocationService procurementByLocationService)
		{
			_httpClientService = httpClientService;
			_procurementByLocationService = procurementByLocationService;
			_userDialogs = userDialogs;

			Title = "Raf Seçimi";

			LoadItemsCommand = new Command(async () => await LoadItemsAsync());
			LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
			ItemTappedCommand = new Command<LocationModel>(ItemTappedAsync);
			NextViewCommand = new Command(async () => await NextViewAsync());
			PerformSearchCommand = new Command(async () => await PerformSearchAsync());
			PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
		}

		public Command LoadItemsCommand { get; }
		public Command LoadMoreItemsCommand { get; }
		public Command ItemTappedCommand { get; }
		public Command NextViewCommand { get; }
		public Command PerformSearchCommand { get; }
		public Command PerformEmptySearchCommand { get; }

		[ObservableProperty]
		public SearchBar searchText;

		public Page CurrentPage { get; set; } = null!;
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
				var result = await _procurementByLocationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, 0, 20);
				if (result.IsSuccess)
				{
					if (result.Data is not null)
					{
						foreach (var item in result.Data)
							Items.Add(Mapping.Mapper.Map<LocationModel>(item));
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
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				_userDialogs.ShowLoading("Loading...");
				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var result = await _procurementByLocationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, Items.Count, 20);
				if (result.IsSuccess)
				{
					if (result.Data is not null)
					{
						foreach (var item in result.Data)
							Items.Add(Mapping.Mapper.Map<LocationModel>(item));
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

		private void ItemTappedAsync(LocationModel item)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				if (item == SelectedLocation)
				{
					SelectedLocation.IsSelected = false;
					SelectedLocation = null;
				}
				else
				{
					if (SelectedLocation != null)
					{
						SelectedLocation.IsSelected = false;
					}

					SelectedLocation = item;
					SelectedLocation.IsSelected = true;
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
				if (SelectedLocation is not null)
				{
					await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationProductListView)}", new Dictionary<string, object>
					{
						[nameof(WarehouseModel)] = WarehouseModel,
						[nameof(LocationModel)] = SelectedLocation
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
				var result = await _procurementByLocationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SearchText.Text, 0, 20);
				if (result.IsSuccess)
				{
					if (result.Data is not null)
					{
						foreach (var item in result.Data)
							Items.Add(Mapping.Mapper.Map<LocationModel>(item));
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
	}
}
