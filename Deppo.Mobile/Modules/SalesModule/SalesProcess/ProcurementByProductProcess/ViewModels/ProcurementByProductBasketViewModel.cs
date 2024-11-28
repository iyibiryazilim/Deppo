using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;


[QueryProperty(name: nameof(ProcurementProductBasketModel), queryId: nameof(ProcurementProductBasketModel))]
public partial class ProcurementByProductBasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IProcurementByProductBasketService _procurementByProductBasketService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ProcurementProductBasketModel procurementProductBasketModel;

	public ObservableCollection<ProcurementProductBasketModel> Items { get; } = new();

	[ObservableProperty]
	ProcurementProductBasketProductModel procurementItem = new();

	[ObservableProperty]
	int currentPosition;

	partial void OnCurrentPositionChanged(int value)
	{
		OnPropertyChanged(nameof(IsPreviousButtonVisible));
		OnPropertyChanged(nameof(IsNextButtonVisible));
		OnPropertyChanged(nameof(IsCompleteButtonVisible));
	}

	[ObservableProperty]
	int totalPosition;
	partial void OnTotalPositionChanged(int value)
	{
		OnPropertyChanged(nameof(IsNextButtonVisible));
		OnPropertyChanged(nameof(IsCompleteButtonVisible));
	}

	public bool IsPreviousButtonVisible => CurrentPosition == 0 ? false : true;
	public bool IsNextButtonVisible => CurrentPosition == TotalPosition ? false : true;
	public bool IsCompleteButtonVisible => CurrentPosition == TotalPosition ? true : false;

	public ProcurementByProductBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IProcurementByProductBasketService procurementByProductBasketService)
	{
		_httpClientService = httpClientService;
		_procurementByProductBasketService = procurementByProductBasketService;
		_userDialogs = userDialogs;

		Title = "Rota";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		IncreaseCommand = new Command<ProcurementProductBasketProductModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<ProcurementProductBasketProductModel>(async (item) => await DecreaseAsync(item));
		QuantityTappedCommand = new Command<ProcurementProductBasketProductModel>(async (item) => await QuantityTappedAsync(item));
		NextPositionCommand = new Command(NextPositionAsync);
		PreviousPositionCommand = new Command(PreviousPositionAsync);
		GoToReasonsForRejectionListViewCommand = new Command<ProcurementProductBasketProductModel>(async (item) => await GoToReasonsForRejectionListViewAsync(item));
		ReverseRejectStatusCommand = new Command<ProcurementProductBasketProductModel>(async (item) => await ReverseRejectStatusAsync(item));
		BackCommand = new Command(async () => await BackAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());

	}
	public Page CurrentPage { get; set; } = null!;
	public Command LoadItemsCommand { get; }

	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command QuantityTappedCommand { get; }

	public Command NextPositionCommand { get; }
	public Command PreviousPositionCommand { get; }
	public Command ProcurementInfoCommand { get; }

	public Command GoToReasonsForRejectionListViewCommand { get; }
	public Command ReverseRejectStatusCommand { get; }
	
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var referenceIds = ProcurementProductBasketModel.ProcurementProductList.Select(x => x.ItemReferenceId).ToArray();
			var result = await _procurementByProductBasketService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: ProcurementProductBasketModel.ProcurementWarehouse.Number,
				itemsReferenceId: referenceIds,
				search: string.Empty,
				skip: 0,
				take: 99999
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

				var groupByLocation = result.Data
					.OrderBy(x => x.LocationName)
					.GroupBy(x => new
					{
						LocationReferenceId = x.LocationReferenceId,
						LocationCode = x.LocationCode,
						LocationName = x.LocationName,
					});

				foreach (var group in groupByLocation)
				{
					var procurementProductBasketModel = new ProcurementProductBasketModel
					{
						LocationReferenceId = group.Key.LocationReferenceId,
						LocationCode = group.Key.LocationCode,
						LocationName = group.Key.LocationName,
					};

					foreach (var item in group)
					{
						var procurementProductBasketProductModel = new ProcurementProductBasketProductModel
						{
							ItemReferenceId = item.ItemReferenceId,
							ItemCode = item.ItemCode,
							ItemName = item.ItemName,
							MainItemReferenceId = item.ItemReferenceId,
							MainItemCode = item.ItemCode,
							MainItemName = item.ItemName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId  = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							TrackingType = item.TrackingType,
							Image = item.Image,
							Quantity = 0,
							ProcurementQuantity = item.ProcurementQuantity,
							StockQuantity = ProcurementProductBasketModel.ProcurementProductList.Sum(x => x.StockQuantity),
						};

						procurementProductBasketModel.BasketProducts.Add(procurementProductBasketProductModel);
					}

					Items.Add(procurementProductBasketModel);
				}

				TotalPosition = Items.Count - 1;
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
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

	private async Task IncreaseAsync(ProcurementProductBasketProductModel item)
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

			if (item.Quantity >= item.StockQuantity)
			{
				await _userDialogs.AlertAsync($"Stok miktarı ({item.StockQuantity}) kadar arttırabilirsiniz.", "Uyarı", "Tamam");
				return;
			}

			if (item.ProcurementQuantity > item.Quantity && item.StockQuantity > item.Quantity)
			{
				item.Quantity++;
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

	private async Task DecreaseAsync(ProcurementProductBasketProductModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (!string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			if (item.Quantity > 0)
			{
				item.Quantity--;
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

	private async Task QuantityTappedAsync(ProcurementProductBasketProductModel item)
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
				title: item.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.StockQuantity)
			{
				await _userDialogs.AlertAsync($"Girilen miktar, stok miktarını ({item.StockQuantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.ProcurementQuantity)
			{
				await _userDialogs.AlertAsync($"Girilen miktar, ürünün toplanabilir miktarını ({item.ProcurementQuantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			item.Quantity = quantity;
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

	private async Task GoToReasonsForRejectionListViewAsync(ProcurementProductBasketProductModel item)
	{
		if (IsBusy)
			return;

		if (item is null)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ProcurementByProductReasonsForRejectionListView)}", new Dictionary<string, object>
			{
				[nameof(ProcurementProductBasketProductModel)] = item
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

	private async Task ReverseRejectStatusAsync(ProcurementProductBasketProductModel item)
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

	private async void NextPositionAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;


			bool canNext = true;

			foreach (var product in Items[CurrentPosition].BasketProducts)
			{
				if (product.ProcurementQuantity > product.Quantity && product.RejectionCode == string.Empty)
				{
					canNext = false;
					_userDialogs.ShowSnackbar(message: $"Lütfen {product.ItemCode} kodlu ürünün tamamını toplayınız ya da hata kodu ekleyiniz!");
				}
			}

			if (CurrentPosition != TotalPosition && canNext)
				CurrentPosition = CurrentPage.FindByName<CarouselView>("carouselView").Position + 1;
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			
		    ProcurementItem = Items.Where(x => x.BasketProducts.Any())
								   .Select(x => x.BasketProducts.FirstOrDefault())
								   .FirstOrDefault();

			if (ProcurementItem is null)
			{
				await _userDialogs.AlertAsync("Herhangi bir toplanan ürününüz yok.", "Hata", "Tamam");
				return;
			}

			bool canNext = true;

			foreach (var product in Items[CurrentPosition].BasketProducts)
			{
				if (product.ProcurementQuantity > product.Quantity && product.RejectionCode == string.Empty)
				{
					canNext = false;
					_userDialogs.ShowSnackbar(message: $"Lütfen {product.ItemCode} kodlu ürünün tamamını toplayınız ya da hata kodu ekleyiniz!");
				}
			}

			if(canNext)
			{
				var confirm = await _userDialogs.ConfirmAsync("Devam istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
				if (!confirm)
					return;

				ProcurementItem.Quantity = Items.Where(x => x.BasketProducts.Any()).SelectMany(x => x.BasketProducts).Sum(x => x.Quantity);
				await Shell.Current.GoToAsync($"{nameof(ProcurementByProductQuantityDistributionListView)}", new Dictionary<string, object>
				{
					[nameof(ProcurementProductBasketModel)] = ProcurementProductBasketModel,
					["ProcurementItem"] = ProcurementItem,
					["Items"] = Items
				});
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
