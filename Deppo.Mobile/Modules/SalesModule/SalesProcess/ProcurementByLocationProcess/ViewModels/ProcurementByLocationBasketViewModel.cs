using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;
using System.Collections.ObjectModel;
using ProcurementCustomerModel = Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels.ProcurementCustomerModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;

[QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
[QueryProperty(nameof(LocationModel), nameof(LocationModel))]
[QueryProperty(nameof(SelectedItems), nameof(SelectedItems))]
[QueryProperty(nameof(SelectedOrderWarehouseModel), nameof(SelectedOrderWarehouseModel))]
public partial class ProcurementByLocationBasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IProcurementByLocationCustomerService _procurementByLocationCustomerService;

	public ProcurementByLocationBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProcurementByLocationCustomerService procurementByLocationCustomerService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_procurementByLocationCustomerService = procurementByLocationCustomerService;

		Title = "Basket";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		NextPositionCommand = new Command(NextPositionAsync);
		PreviousPositionCommand = new Command(PreviousPositionAsync);
		IncreaseCommand = new Command<ProcurementCustomerModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<ProcurementCustomerModel>(async (item) => await DecreaseAsync(item));
		QuantityTappedCommand = new Command<ProcurementCustomerModel>(async (item) => await QuantityTappedAsync(item));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		GoToReasonsForRejectionListViewCommand = new Command<ProcurementCustomerModel>(async (item) => await GoToReasonsForRejectionListViewAsync(item));
		ReverseRejectStatusCommand = new Command<ProcurementCustomerModel>(async (item) => await ReverseRejectStatusAsync(item));
	}


	[ObservableProperty]
	ObservableCollection<Core.Models.ProcurementModels.ByLocationModels.ProcurementCustomerBasketModel> selectedItems;

	[ObservableProperty]
	ObservableCollection<ProcurementLocationBasketModel> items = new();


	[ObservableProperty]
	ObservableCollection<ProcurementCustomerModel> procurementCustomers = new();


	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private WarehouseModel selectedOrderWarehouseModel = null!;

	[ObservableProperty]
	private LocationModel locationModel = null!;

	partial void OnCurrentPositionChanged(int value)
	{
		OnPropertyChanged(nameof(IsPreviousButtonVisible));
		OnPropertyChanged(nameof(IsNextButtonVisible));
		OnPropertyChanged(nameof(IsCompleteButtonVisible));
		//OnPropertyChanged(nameof(IsPageIndicatorVisible));
	}

	partial void OnTotalPositionChanged(int value)
	{
		OnPropertyChanged(nameof(IsNextButtonVisible));
		OnPropertyChanged(nameof(IsCompleteButtonVisible));
		//OnPropertyChanged(nameof(IsPageIndicatorVisible));
	}
	[ObservableProperty]
	private int currentPosition = 0;

	[ObservableProperty]
	private int totalPosition;

	public Page CurrentPage { get; set; }
	public bool IsPreviousButtonVisible => CurrentPosition == 0 ? false : true;
	public bool IsNextButtonVisible => CurrentPosition == TotalPosition ? false : true;
	public bool IsCompleteButtonVisible => (/*Items.Count > 0 &&*/ CurrentPosition == TotalPosition) ? true : false;
	public Command LoadItemsCommand { get; }
	public Command NextPositionCommand { get; }
	public Command PreviousPositionCommand { get; }
	public Command GoToReasonsForRejectionListViewCommand { get; }
	public Command ReverseRejectStatusCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command QuantityTappedCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }


	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Loading...");
			await Task.Delay(1000);

			Items.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			List<int> productIds = new List<int>();
			string productIdsString = string.Empty;

			foreach (var item in SelectedItems)
			{
				foreach (var product in item.ProcurementByLocationProducts)
				{
					if (!productIds.Contains(product.ItemReferenceId))
					{
						productIds.Add(product.ItemReferenceId);
						productIdsString += product.ItemReferenceId + ",";
					}
				}
			}

			if (productIdsString.EndsWith(","))
			{
				productIdsString = productIdsString.Substring(0, productIdsString.Length - 1); // Son virgülü kaldır
			}

			ProcurementCustomers.Clear();

			var result = await _procurementByLocationCustomerService.GetCustomers(
				httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				SelectedOrderWarehouseModel.Number,
				productIdsString,//product.ItemReferenceId,
				string.Empty,
				0,
				9999999
			);

			if (result.Data.Any())
			{
				foreach (var item in result.Data)
				{
					ProcurementCustomers.Add(Mapping.Mapper.Map<ProcurementCustomerModel>(item));
				}

				var groupedProducts = ProcurementCustomers.GroupBy(x => x.ProductReferenceId);
				foreach (var product in groupedProducts) 
				{

					var newBasketModel = new ProcurementLocationBasketModel();
					newBasketModel.ProcurementByLocationProduct = SelectedItems.FirstOrDefault(x => x.ProcurementByLocationProducts.Any(y => y.ItemReferenceId == product.Key))?.ProcurementByLocationProducts.FirstOrDefault(x => x.ItemReferenceId == product.Key);

					foreach (var customer in product)
					{
						if(SelectedItems.Any(x=>x.ProcurementCustomerModel.CustomerReferenceId == customer.CustomerReferenceId))
							newBasketModel.ProcurementCustomers.Add(customer);
					}

					Items.Add(newBasketModel);
				}
			}


			TotalPosition = Items.Count - 1;
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Error", "OK");
		}
		finally
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			IsBusy = false;
		}
	}


	private void NextPositionAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;


			bool canNext = true;

			foreach (var customer in Items[CurrentPosition].ProcurementCustomers)
			{
				if (customer.OutputQuantity != customer.WaitingQuantity && string.IsNullOrEmpty(customer.RejectionCode))
				{
					canNext = false;
					_userDialogs.ShowSnackbar(message: $"Lütfen {customer.CustomerName} adlı müşteriye miktar giriniz veya hata kodu giriniz");
				}
			}

			if (CurrentPosition != TotalPosition && canNext)
			{
				var position = CurrentPage.FindByName<CarouselView>("carouselView").Position;
				position++;

				CurrentPosition = position;
			}

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void PreviousPositionAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (CurrentPosition == 0)
				return;

			CurrentPosition = CurrentPage.FindByName<CarouselView>("carouselView").Position - 1;
		}
		catch (System.Exception)
		{
			_userDialogs.Alert("Bir hata oluştu.", "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task IncreaseAsync(ProcurementCustomerModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (!string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			IsBusy = true;
			var stockQuantity = Items[CurrentPosition].ProcurementByLocationProduct.StockQuantity;
			if (Items[CurrentPosition].ProcurementCustomers.Sum(x => x.OutputQuantity) >= stockQuantity)
			{
				await _userDialogs.AlertAsync("Stok miktarını aşamazsınız.", "Uyarı", "Tamam");
				return;
			}
			if (item.WaitingQuantity > item.OutputQuantity && stockQuantity > item.OutputQuantity)
			{
				item.OutputQuantity++;
			}
			else if (item.OutputQuantity >= item.WaitingQuantity)
			{
				await _userDialogs.AlertAsync($"Bekleyen miktarı ({item.WaitingQuantity}) kadar arttırabilirsiniz.", "Uyarı", "Tamam");
				return;
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

	private async Task DecreaseAsync(ProcurementCustomerModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (!string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			if (item.OutputQuantity > 0)
			{
				item.OutputQuantity--;
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

	private async Task QuantityTappedAsync(ProcurementCustomerModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (!string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.CustomerName,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.WaitingQuantity)
			{
				await _userDialogs.AlertAsync($"Girilen miktar, bekleyen miktarını ({item.WaitingQuantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
			}


			item.OutputQuantity = quantity;
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

	private async Task GoToReasonsForRejectionListViewAsync(ProcurementCustomerModel item)
	{
		if (IsBusy)
			return;

		if (item is null)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationReasonsForRejectionListView)}", new Dictionary<string, object>
			{
				[nameof(ProcurementCustomerModel)] = item
			});
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

	private async Task ReverseRejectStatusAsync(ProcurementCustomerModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			item.RejectionCode = string.Empty;
			item.RejectionName = string.Empty;
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
		try
		{
			IsBusy = true;



			await Shell.Current.GoToAsync($"{nameof(ProcurementByLocationCustomerFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(LocationModel)] = LocationModel,
				[nameof(SelectedItems)] = SelectedItems,
				[nameof(SelectedOrderWarehouseModel)] = SelectedOrderWarehouseModel,
				["BasketModel"] = Items
			});
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

			var confirm = await _userDialogs.ConfirmAsync("Verileriniz silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;



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
