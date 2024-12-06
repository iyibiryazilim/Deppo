using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(OutsourceModel), queryId: nameof(OutsourceModel))]
public partial class InputOutsourceTransferV2ProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IInputOutsourceTransferV2ProductService _inputOutsourceTransferV2ProductService;

	[ObservableProperty]
	WarehouseModel? warehouseModel;

	[ObservableProperty]
	OutsourceModel? outsourceModel;

	[ObservableProperty]
	InputOutsourceTransferProductModel? selectedOutsourceProductModel;

	public ObservableCollection<InputOutsourceTransferProductModel> Items { get; } = new();

	[ObservableProperty]
	SearchBar searchText;

	public InputOutsourceTransferV2ProductListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IInputOutsourceTransferV2ProductService inputOutsourceTransferV2ProductService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_inputOutsourceTransferV2ProductService = inputOutsourceTransferV2ProductService;
		
		Title = "Fason Ürünler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		ItemTappedCommand = new Command<InputOutsourceTransferProductModel>(async (productModel) => await ItemTappedAsync(productModel));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command PerformEmptySearchCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	public async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _inputOutsourceTransferV2ProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				currentReferenceId: OutsourceModel.ReferenceId,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<InputOutsourceTransferProductModel>(item);
					obj.IsSelected = SelectedOutsourceProductModel != null && SelectedOutsourceProductModel.ReferenceId == obj.ReferenceId ? SelectedOutsourceProductModel.IsSelected : false;
					Items.Add(obj);
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;

		if (Items.Count < 18)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading More Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _inputOutsourceTransferV2ProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				currentReferenceId: OutsourceModel.ReferenceId,
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
					var obj = Mapping.Mapper.Map<InputOutsourceTransferProductModel>(item);
					obj.IsSelected = SelectedOutsourceProductModel != null && SelectedOutsourceProductModel.ReferenceId == obj.ReferenceId ? SelectedOutsourceProductModel.IsSelected : false;
					Items.Add(obj);
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

	private async Task PerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(SearchText.Text))
		{
			await PerformSearchAsync();
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
			_userDialogs.Loading("Searching...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _inputOutsourceTransferV2ProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				currentReferenceId: OutsourceModel.ReferenceId,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);


			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<InputOutsourceTransferProductModel>(item);
					obj.IsSelected = SelectedOutsourceProductModel != null && SelectedOutsourceProductModel.ReferenceId == obj.ReferenceId ? SelectedOutsourceProductModel.IsSelected : false;
					Items.Add(obj);
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
	private async Task ItemTappedAsync(InputOutsourceTransferProductModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedOutsourceProductModel == item)
			{
				SelectedOutsourceProductModel.IsSelected = false;
				SelectedOutsourceProductModel = null;
			}
			else
			{
				if (SelectedOutsourceProductModel != null)
				{
					SelectedOutsourceProductModel.IsSelected = false;
				}
				SelectedOutsourceProductModel = item;
				SelectedOutsourceProductModel.IsSelected = true;

				Items.Where(x => x.ReferenceId != item.ReferenceId).ToList().ForEach(x => x.IsSelected = false);
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		if (SelectedOutsourceProductModel is null && WarehouseModel is null && SelectedOutsourceProductModel is null)
			return;
		try
		{
			IsBusy = true;

			InputOutsourceTransferV2BasketModel inputOutsourceTransferV2BasketModel = new();

			inputOutsourceTransferV2BasketModel.OutsourceWarehouseModel = WarehouseModel;
			inputOutsourceTransferV2BasketModel.OutsourceModel = OutsourceModel;

			SelectedOutsourceProductModel.InputQuantity = (SelectedOutsourceProductModel.LocTracking == 0) ? 1 : 0;
			inputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel = SelectedOutsourceProductModel;

			if(inputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.Details is null)
			{
				inputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.Details = new();
			}

			await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2BasketView)}", new Dictionary<string, object>
			{
				[nameof(InputOutsourceTransferV2BasketModel)] = inputOutsourceTransferV2BasketModel
			});

			SearchText.Text = string.Empty;
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;


			if (SelectedOutsourceProductModel is not null)
			{
				SelectedOutsourceProductModel.IsSelected = false;
				SelectedOutsourceProductModel = null;
			}

			SearchText.Text = string.Empty;

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
