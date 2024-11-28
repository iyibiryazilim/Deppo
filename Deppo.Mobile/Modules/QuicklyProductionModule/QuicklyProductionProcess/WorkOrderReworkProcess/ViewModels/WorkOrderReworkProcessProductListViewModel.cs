using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;
using DevExpress.Data.Async.Helpers;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;

public partial class WorkOrderReworkProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IQuicklyBomService _quicklyBomService;
	private readonly IUserDialogs _userDialogs;

	public ObservableCollection<QuicklyBOMProductModel> Items { get; } = new();

	[ObservableProperty]
	QuicklyBOMProductModel? selectedItem;

	[ObservableProperty]
	public SearchBar searchText;
	public WorkOrderReworkProcessProductListViewModel(IHttpClientService httpClientService, IQuicklyBomService quicklyBomService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_quicklyBomService = quicklyBomService;
		_userDialogs = userDialogs;

		Title = "Reçete Ürün Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<QuicklyBOMProductModel>(async (x) => await ItemTappedAsync(x));
		BackCommand = new Command(async () => await BackAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

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

			var result = await _quicklyBomService.GetObjectsWorkOrder(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
					Items.Add(Mapping.Mapper.Map<QuicklyBOMProductModel>(item));
                }
            }

			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading More Items...");
			
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _quicklyBomService.GetObjectsWorkOrder(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<QuicklyBOMProductModel>(item));
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

	private async Task ItemTappedAsync(QuicklyBOMProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item == SelectedItem)
			{
				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}
			else
			{
				if (SelectedItem != null)
				{
					SelectedItem.IsSelected = false;
				}

				SelectedItem = item;
				SelectedItem.IsSelected = true;
			}

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
	
	private async Task NextViewAsync()
	{
		if (SelectedItem is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			WorkOrderReworkBasketModel workOrderReworkBasketModel = new WorkOrderReworkBasketModel();
			workOrderReworkBasketModel.WorkOrderReworkMainProductModel = SelectedItem;
			workOrderReworkBasketModel.WorkOrderReworkMainProductModel.Amount = SelectedItem.Amount;
			workOrderReworkBasketModel.BOMQuantity = SelectedItem.LocTracking == 1 ? 0 : 1; //
			SearchText.Text = string.Empty;

			await Shell.Current.GoToAsync($"{nameof(WorkOrderReworkProcessBasketView)}", new Dictionary<string, object>
			{
				[nameof(WorkOrderReworkBasketModel)] = workOrderReworkBasketModel
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			var confirm = await _userDialogs.ConfirmAsync("İşlemi iptal etmek istediğinize emin misiniz?", "İptal", "Evet", "Hayır");
			if (!confirm)
				return;
			if (SelectedItem != null)
			{
				SelectedItem.IsSelected = false;
				SelectedItem = null;
				SearchText.Text = string.Empty;
			}

			await Shell.Current.GoToAsync($"..");
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
			var result = await _quicklyBomService.GetObjectsWorkOrder(
				httpClient: httpClient, 
				firmNumber: _httpClientService.FirmNumber, 
				periodNumber: _httpClientService.PeriodNumber, 
				search: SearchText.Text, 
				skip: 0, 
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<QuicklyBOMProductModel>(product);
					Items.Add(item);
				}
			}

			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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
}
