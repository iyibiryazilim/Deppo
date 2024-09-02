using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISerilotTransactionService _serilotTransactionService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly IUserDialogs _userDialogs;
	public OutputProductProcessBasketListViewModel(IHttpClientService httpClientService, ISerilotTransactionService serilotTransactionService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_serilotTransactionService = serilotTransactionService;
		_locationTransactionService = locationTransactionService;
		_userDialogs = userDialogs;

		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		IncreaseCommand = new Command<OutputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<OutputProductBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<OutputProductBasketModel>(async (item) => await DeleteItemAsync(item));
		LoadSerilotTransactionItemsCommand = new Command(async () => await LoadSerilotTransactionItemsAsync());
		LoadMoreSerilotTransactionItemsCommand = new Command(async () => await LoadMoreSerilotTransactionItemsAsync());
		LoadLocationTransactionItemsCommand = new Command(async () => await LoadLocationTransactionItemsAsync());
		LoadMoreLocationTransactionItemsCommand = new Command(async () => await LoadMoreLocationTransactionItemsAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	#region Commands
	public Command ShowProductViewCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command DeleteItemCommand { get; }
	#region Locations Command
	public Command LoadLocationTransactionItemsCommand { get; }
	public Command LoadMoreLocationTransactionItemsCommand { get; }
	public Command LocationTransactionTappedCommand { get; }
	public Command ConfirmLocationTransactionCommand { get; }
	#endregion

	#region Serilot Command
	public Command LoadSerilotTransactionItemsCommand { get; }
	public Command LoadMoreSerilotTransactionItemsCommand { get; }
	public Command ConfirmSerilotTransactionCommand { get; }
	#endregion
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	#endregion

	#region Properties
	public ContentPage CurrentPage { get; set; } = null!;

	[ObservableProperty]
	OutputProductProcessType outputProductProcessType;
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	OutputProductBasketModel selectedItem;
	#endregion

	#region Collections
	public ObservableCollection<OutputProductBasketModel> Items { get; } = new();

	public ObservableCollection<SerilotTransaction> SerilotTransactionItems { get; } = new();
	public ObservableCollection<LocationTransaction> LocationTransactionItems { get; } = new();
	#endregion

	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OutputProductProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel
			});
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

	private async Task IncreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				SelectedItem = item;
				if (item.LocTracking == 1)
				{
					OutputProductProcessBasketListView currentPage = CurrentPage as OutputProductProcessBasketListView;
					currentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.HalfExpanded;
					currentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").Loaded += async (s, e) =>
					{
						LocationTransactionItems.Clear();
						await LoadLocationTransactionItemsAsync();
					};
				}
				else
				{
					item.Quantity++;
				}
			}
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

	private async Task DecreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item.Quantity > 1)
			{
				item.Quantity--;
			}

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

	private async Task DeleteItemAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

			if (!result)
				return;

			Items.Remove(item);
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

	private async Task LoadLocationTransactionItemsAsync()
	{
		//if (IsBusy)
		//	return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Load Location Items...");
			await Task.Delay(1000);
			LocationTransactionItems.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedItem.ItemReferenceId,
				warehouseNumber: WarehouseModel.Number
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;
				LocationTransactionItems.Clear();
				foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<LocationTransaction>(item);
					LocationTransactionItems.Add(obj);
                }
            }

			_userDialogs.Loading().Hide();
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

	private async Task LoadMoreLocationTransactionItemsAsync()
	{
		if (IsBusy)
			return;
		if (LocationTransactionItems.Count < (18))  // 18 = Take (20) - Remaining ItemsThreshold (2)
			return; 
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetInputObjectsAsync(httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedItem.ItemReferenceId,
				warehouseNumber: WarehouseModel.Number,
				skip: LocationTransactionItems.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<LocationTransaction>(item);
					LocationTransactionItems.Add(obj);
				}
			}
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


	private async Task LoadSerilotTransactionItemsAsync()
	{
		//if (IsBusy)
		//	return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Load Serilot Items...");
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<OutputProductBasketDetailModel>(item);
					SerilotTransactionItems.Add(obj);
				}
			}

			_userDialogs.Loading().Hide();
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

	private async Task LoadMoreSerilotTransactionItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, skip: SerilotTransactionItems.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<OutputProductBasketDetailModel>(item);
					SerilotTransactionItems.Add(obj);
				}
			}
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (Items.Count == 0)
			{
				await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
				return;
			}
			await Shell.Current.GoToAsync($"{nameof(OutputProductProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(OutputProductProcessType)] = OutputProductProcessType
			});
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (Items.Count > 0)
			{
				var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
				if (!result)
					return;

				Items.Clear();
				SerilotTransactionItems.Clear();
				LocationTransactionItems.Clear();
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				await Shell.Current.GoToAsync("..");
			}
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
}
