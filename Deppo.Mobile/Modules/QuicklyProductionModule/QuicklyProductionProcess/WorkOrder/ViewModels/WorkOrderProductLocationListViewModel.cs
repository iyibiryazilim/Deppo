using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Android.Provider.CallLog;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels
{
	[QueryProperty(name: nameof(QuicklyBomProductBasketModel), queryId: nameof(QuicklyBomProductBasketModel))]
	public partial class WorkOrderProductLocationListViewModel : BaseViewModel
	{
		private readonly IHttpClientService _httpClientService;
		private readonly ILocationService _locationService;
		private readonly IUserDialogs _userDialogs;
		private readonly IServiceProvider _serviceProvider;

		[ObservableProperty]
		QuicklyBomProductBasketModel quicklyBomProductBasketModel = null!;

		public ObservableCollection<LocationModel> Items { get; } = new(); // LocationTransactionBottomSheet'deki verileri tutan liste
		public ObservableCollection<LocationModel> SelectedItems { get; } = new();

		[ObservableProperty]
		LocationModel? selectedItem;

		public WorkOrderProductLocationListViewModel(IHttpClientService httpClientService, ILocationService locationService, IUserDialogs userDialogs, IServiceProvider serviceProvider)
		{
			_httpClientService = httpClientService;
			_locationService = locationService;
			_userDialogs = userDialogs;
			_serviceProvider = serviceProvider;

			ShowLocationsCommand = new Command(async () => await ShowLocationsAsync());
			PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
			IncreaseCommand = new Command<LocationModel>(async (x) => await IncreaseAsync(x));
			DecreaseCommand = new Command<LocationModel>(async (x) => await DecreaseAsync(x));
			ConfirmCommand = new Command(async () => await ConfirmAsync());
			LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
			CloseLocationsCommand = new Command(async () => await CloseLocationsAsync());
			ItemTappedCommand = new Command<LocationModel>(async (x) => await ItemTappedAsync(x));
			ConfirmLocationsCommand = new Command(async () => await ConfirmLocationsAsync());
			CancelCommand = new Command(async () => await CancelAsync());
		}
		public Page CurrentPage { get; set; } = null!;
		public Command ShowLocationsCommand { get; }
		public Command CancelCommand { get; }
		public Command PerformSearchCommand { get; }
		public Command IncreaseCommand { get; }
		public Command DecreaseCommand { get; }
		public Command QuantityTappedCommand { get; }
		public Command CloseLocationsCommand { get; }
		public Command ConfirmLocationsCommand { get; }
		public Command ConfirmCommand { get; }
		public Command ItemTappedCommand { get; }
		public Command LoadMoreItemsCommand { get; }

		public async Task LoadSelectedItemsAsync()
		{
			try
			{
				IsBusy = true;

				_userDialogs.ShowLoading("Loading...");
				SelectedItems.Clear();
				await Task.Delay(1000);

				if (QuicklyBomProductBasketModel.MainLocations.Count > 0)
				{
					foreach (var item in QuicklyBomProductBasketModel.MainLocations)
						SelectedItems.Add(new LocationModel
						{
							Code = item.Code,
							Name = item.Name,
							StockQuantity = item.StockQuantity,
							InputQuantity = item.InputQuantity,
						});
				}

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();
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

		private async Task ShowLocationsAsync()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				await LoadItemsAsync();
				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.HalfExpanded;
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

		private async Task PerformSearchAsync(Entry barcodeEntry)
		{
			if (IsBusy)
				return;

			try
			{
				if (string.IsNullOrEmpty(barcodeEntry.Text))
					return;

				IsBusy = true;

				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var result = await _locationService.GetObjects(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: QuicklyBomProductBasketModel.QuicklyBomProduct.WarehouseNumber,
							productReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant == true ? QuicklyBomProductBasketModel.QuicklyBomProduct.MainItemReferenceId : QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId,
							variantReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant == true ? QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId : 0,
							search: barcodeEntry.Text,
							skip: 0,
							take: 1);

				if (result.IsSuccess)
				{
					if (!(result.Data.Count() > 0))
					{
						if (_userDialogs.IsHudShowing)
							_userDialogs.HideHud();

						_userDialogs.ShowToast(message: $"{barcodeEntry.Text} kodlu raf bulunamadı.");
						return;
					}

					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<LocationModel>(item);
						if (SelectedItems.Where(x => x.Code == obj.Code).Any())
						{
							SelectedItems.Where(x => x.Code == obj.Code).FirstOrDefault().InputQuantity += 1;
						}
						else
						{
							SelectedItems.Add(obj);
						}
					}
				}

				if (_userDialogs.IsHudShowing)
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
				barcodeEntry.Text = string.Empty;
				barcodeEntry.Focus();
				IsBusy = false;
			}
		}
		private async Task IncreaseAsync(LocationModel locationModel)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				SelectedItem = locationModel;

				var selectedItemsTotalInputQuantity = SelectedItems.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);

				if (QuicklyBomProductBasketModel.QuicklyBomProduct.TrackingType != 0)
				{
				}
				else
				{

					locationModel.InputQuantity += 1;
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

		private async Task DecreaseAsync(LocationModel locationModel)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				SelectedItem = locationModel;

				if (locationModel.InputQuantity > 0)
				{
					if (QuicklyBomProductBasketModel.QuicklyBomProduct.TrackingType != 0)
					{
					}
					else
					{
						locationModel.InputQuantity -= 1;
					}
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
		private async Task ConfirmAsync()
		{
			if (IsBusy)
				return;
			try
			{
				IsBusy = true;

				var basketViewModel = _serviceProvider.GetRequiredService<WorkOrderCalcViewModel>();


				double locationsTotalInputQuantity = (double)Items.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);

				if (locationsTotalInputQuantity != 0)
				{
					double rate = locationsTotalInputQuantity / QuicklyBomProductBasketModel.QuicklyBomProduct.Amount;

					QuicklyBomProductBasketModel.QuicklyBomProduct.Amount = locationsTotalInputQuantity;

					foreach (var i in QuicklyBomProductBasketModel.SubProducts)
					{
						i.ProductModel.Amount = (double)(i.ProductModel.Amount * rate);
					}
					foreach (var item in Items.Where(x => x.InputQuantity > 0))
					{
						if (QuicklyBomProductBasketModel.MainLocations.Any(x => x.ReferenceId == item.ReferenceId))
						{
							QuicklyBomProductBasketModel.MainLocations.FirstOrDefault(x => x.ReferenceId == item.ReferenceId).InputQuantity = item.InputQuantity;
						}
						else
						{
							QuicklyBomProductBasketModel.MainLocations.Add(item);
						}
					}
					foreach (var item in SelectedItems.Where(x => x.InputQuantity > 0))
					{
						var location = QuicklyBomProductBasketModel.MainLocations.FirstOrDefault(x => x.Code == item.Code);
						if (location is not null)
						{
							location.InputQuantity = item.InputQuantity;

						}
						else
						{
							QuicklyBomProductBasketModel.MainLocations.Add(new LocationModel
							{
								ReferenceId = item.ReferenceId,
								Code = item.Code,
								Name = item.Name,
								StockQuantity = item.StockQuantity,
								InputQuantity = item.InputQuantity,
							});
						}
					}
				}
				else
				{
					QuicklyBomProductBasketModel.QuicklyBomProduct.Amount = QuicklyBomProductBasketModel.MainAmount;
					foreach (var i in QuicklyBomProductBasketModel.SubProducts)
					{
						i.ProductModel.Amount = i.SubAmount;
					}
					QuicklyBomProductBasketModel.MainLocations.Clear();
				}

				QuicklyBomProductBasketModel.BOMQuantity = (double)QuicklyBomProductBasketModel.MainLocations.Sum(x => x.InputQuantity);

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

		private async Task LoadItemsAsync()
		{
			try
			{
				IsBusy = true;
				_userDialogs.ShowLoading("Loading...");
				Items.Clear();
				await Task.Delay(1000);
				var httpClient = _httpClientService.GetOrCreateHttpClient();

				var result = await _locationService.GetObjects(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					warehouseNumber: QuicklyBomProductBasketModel.QuicklyBomProduct.WarehouseNumber,
					productReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant == true ?  QuicklyBomProductBasketModel.QuicklyBomProduct.MainItemReferenceId : QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId,
					variantReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant == true ? QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId : 0,
					skip: 0,
					take: 20);

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
				_userDialogs.Alert(ex.Message);
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

				var result = await _locationService.GetObjects(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					warehouseNumber: QuicklyBomProductBasketModel.QuicklyBomProduct.WarehouseNumber,
					productReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant == true ? QuicklyBomProductBasketModel.QuicklyBomProduct.MainItemReferenceId : QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId,
					variantReferenceId: QuicklyBomProductBasketModel.QuicklyBomProduct.IsVariant == true ? QuicklyBomProductBasketModel.QuicklyBomProduct.ReferenceId : 0,
					skip: Items.Count,
					take: 20);

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
				_userDialogs.Alert(ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
		private async Task ItemTappedAsync(LocationModel locationModel)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				if (locationModel.IsSelected)
				{
					Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = false;
				}
				else
				{
					Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = true;
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
		private async Task ConfirmLocationsAsync()
		{
			if (IsBusy)
				return;
			try
			{
				IsBusy = true;

				_userDialogs.ShowLoading("Onaylanıyor...");
				await Task.Delay(500);
				foreach (var item in Items.Where(x => x.IsSelected))
				{
					if (!SelectedItems.Any(X => X.Code == item.Code))
						SelectedItems.Add(item);
				}

				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
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

		private async Task CloseLocationsAsync()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				_userDialogs.ShowLoading("Loading...");
				await Task.Delay(500);
				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;

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

		private async Task CancelAsync()
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;

				_userDialogs.ShowLoading("Loading...");
				await Task.Delay(500);
				await Shell.Current.GoToAsync("..");
				_userDialogs.HideHud();
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
