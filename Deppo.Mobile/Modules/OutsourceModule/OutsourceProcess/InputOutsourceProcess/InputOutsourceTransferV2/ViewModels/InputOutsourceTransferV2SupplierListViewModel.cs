using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;
using DevExpress.Maui.Core.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class InputOutsourceTransferV2SupplierListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IOutsourceService _outsourceService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel? warehouseModel;

	[ObservableProperty]
	OutsourceModel? selectedOutsourceModel;

	public ObservableCollection<OutsourceModel> Items { get; } = new();

	[ObservableProperty]
	SearchBar searchText;

	public InputOutsourceTransferV2SupplierListViewModel(IHttpClientService httpClientService, IOutsourceService outsourceService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_outsourceService = outsourceService;
		_userDialogs = userDialogs;

		Title = "Fason Cari Seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		ItemTappedCommand = new Command<OutsourceModel>(async (outsourceModel) => await ItemTappedAsync(outsourceModel));
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


	private async Task LoadItemsAsync()
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
			var result = await _outsourceService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
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
					var obj = Mapping.Mapper.Map<OutsourceModel>(item);
					obj.IsSelected = SelectedOutsourceModel != null && SelectedOutsourceModel.Code == obj.Code ? SelectedOutsourceModel.IsSelected : false;
					Items.Add(obj);
				}

			}

			if(_userDialogs.IsHudShowing)
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
			var result = await _outsourceService.GetObjects(
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
					var obj = Mapping.Mapper.Map<OutsourceModel>(item);
					obj.IsSelected = SelectedOutsourceModel != null && SelectedOutsourceModel.Code == obj.Code ? SelectedOutsourceModel.IsSelected : false;
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
			var result = await _outsourceService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
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
					var obj = Mapping.Mapper.Map<OutsourceModel>(item);
					obj.IsSelected = SelectedOutsourceModel != null && SelectedOutsourceModel.Code == obj.Code ? SelectedOutsourceModel.IsSelected : false;
					Items.Add(obj);
				}
			}

			if (_userDialogs.IsHudShowing)
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
	private async Task ItemTappedAsync(OutsourceModel outsourceModel)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedOutsourceModel == outsourceModel)
			{
				SelectedOutsourceModel.IsSelected = false;
				SelectedOutsourceModel = null;
			}
			else
			{
				if (SelectedOutsourceModel != null)
				{
					SelectedOutsourceModel.IsSelected = false;
				}
				SelectedOutsourceModel = outsourceModel;
				SelectedOutsourceModel.IsSelected = true;

				Items.Where(x => x.Code != outsourceModel.Code).ForEach(x => x.IsSelected = false);
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
		if (SelectedOutsourceModel is null && WarehouseModel is null)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2ProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(OutsourceModel)] = SelectedOutsourceModel
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


			if(SelectedOutsourceModel is not null)
			{
				SelectedOutsourceModel.IsSelected = false;
				SelectedOutsourceModel = null;
			}

			SearchText.Text = string.Empty;

			await Shell.Current.GoToAsync("..");
		}
		catch(Exception ex)
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
