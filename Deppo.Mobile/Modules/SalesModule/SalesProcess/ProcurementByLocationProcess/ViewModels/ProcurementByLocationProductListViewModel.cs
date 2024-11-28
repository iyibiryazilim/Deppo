using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels
{
	[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
	[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
	public partial class ProcurementByLocationProductListViewModel : BaseViewModel
	{
		private readonly IHttpClientService _httpClientService;
		private readonly IUserDialogs _userDialogs;
		private readonly IProcurementByLocationProductService _procurementByLocationProductService;

		[ObservableProperty]
		private LocationModel locationModel = null!;

		[ObservableProperty]
		private WarehouseModel warehouseModel = null!;

		[ObservableProperty]
		private ProcurementByLocationProduct selectedProduct;

		public ObservableCollection<ProcurementByLocationProduct> Items { get; } = new();
		public ObservableCollection<ProcurementByLocationProduct> SelectedItems { get; } = new();
		public ObservableCollection<ProcurementByLocationProduct> SelectedSearchItems { get; } = new();




		public ProcurementByLocationProductListViewModel(IHttpClientService httpClientService,
		IUserDialogs userDialogs,
		IProcurementByLocationProductService procurementByLocationProductService)
		{
			_httpClientService = httpClientService;
			_userDialogs = userDialogs;
			_procurementByLocationProductService = procurementByLocationProductService;

			Title = "Ürün Seçimi";

			LoadItemsCommand = new Command(async () => await LoadItemsAsync());
			LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
			ItemTappedCommand = new Command<ProcurementByLocationProduct>(ItemTappedAsync);
			NextViewCommand = new Command(async () => await NextViewAsync());
			PerformSearchCommand = new Command(async () => await PerformSearchAsync());
			PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
			LoadItemsCommand = new Command(async () => await BackAsync());
		}

		public Command LoadItemsCommand { get; }
		public Command LoadMoreItemsCommand { get; }
		public Command ItemTappedCommand { get; }
		public Command NextViewCommand { get; }
		public Command PerformSearchCommand { get; }
		public Command PerformEmptySearchCommand { get; }

		public Command BackCommand { get; }

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
				var result = await _procurementByLocationProductService.GetProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number,LocationModel.ReferenceId, SearchText.Text, 0, 20);
				if (result.IsSuccess)
				{
					if (result.Data is not null)
					{
						foreach (var item in result.Data)
						{
							var product = Mapping.Mapper.Map<ProcurementByLocationProduct>(item);
							var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.ItemReferenceId == product.ItemReferenceId);
							if (matchedItem is not null)
								product.IsSelected = matchedItem.IsSelected;
							else
								product.IsSelected = false;

							Items.Add(product);

						}
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
				var result = await _procurementByLocationProductService.GetProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, LocationModel.ReferenceId, SearchText.Text, Items.Count, 20);
				if (result.IsSuccess)
				{
					if (result.Data is not null)
					{
						foreach (var item in result.Data)
							Items.Add(Mapping.Mapper.Map<ProcurementByLocationProduct>(item));
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

		private void ItemTappedAsync(ProcurementByLocationProduct item)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				SelectedProduct = item;

				if (!item.IsSelected)
				{
					item.IsSelected = true;
					SelectedItems.Add(item);
					SelectedSearchItems.Add(item);
				}
				else
				{
					item.IsSelected = false;
					SelectedItems.Remove(item);
					SelectedSearchItems.Remove(item);
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
				await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationOrderWarehouseListView)}", new Dictionary<string, object>
				{
					[nameof(WarehouseModel)] = WarehouseModel,
					[nameof(LocationModel)] = LocationModel,
					[nameof(SelectedItems)] = SelectedItems
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
				var result = await _procurementByLocationProductService.GetProducts(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, LocationModel.ReferenceId, SearchText.Text, 0, 20);
				if (result.IsSuccess)
				{
					if (result.Data is not null)
					{
						foreach (var item in result.Data)
						{
							var product = Mapping.Mapper.Map<ProcurementByLocationProduct>(item);
							var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.ItemReferenceId == product.ItemReferenceId);
							if (matchedItem is not null)
								product.IsSelected = matchedItem.IsSelected;
							else
								product.IsSelected = false;
							
							Items.Add(product);
						}
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

		private async Task BackAsync()
		{
			if (IsBusy)
				return;
			try
			{
				IsBusy = true;

				SelectedItems.Clear();
				SelectedProduct = null;
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
}
