using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PackageProductModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using DevExpress.Maui.Controls;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels
{
    [QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
    [QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
	[QueryProperty(nameof(CollectedProducts), nameof(CollectedProducts))]

	public partial class ProcurementSalesPackageBasketViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPackageProductService _packageProductService;


		[ObservableProperty]
        private WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        public ProcurementSalesCustomerModel procurementSalesCustomerModel = null!;

        [ObservableProperty]
        ObservableCollection<ProcurementSalesProductModel> collectedProducts;


		[ObservableProperty]
		public ProcurementPackageBasketModel? selectedPackageBasketModel = new();

		public ObservableCollection<ProcurementPackageBasketModel> Items { get; } = new();
        public ObservableCollection<ProcurementPackageBasketModel> SelectedItems { get; } = new();

		public ProcurementSalesPackageBasketViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IServiceProvider serviceProvider, IPackageProductService packageProductService)
		{
			_userDialogs = userDialogs;
			_httpClientService = httpClientService;
			_serviceProvider = serviceProvider;
			_packageProductService = packageProductService;

			Title = "Koli Sepet Listesi";

			ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
			DeleteItemCommand = new Command<ProcurementPackageBasketModel>(async (item) => await DeleteItemAsync(item));
			NextViewCommand = new Command(async () => await NextViewAsync());
			BackCommand = new Command(async () => await BackAsync());
			ItemTappedCommand = new Command<ProcurementPackageBasketModel>(async (item) => await ItemTappedAsync(item));
			SwipeItemCommand = new Command<ProcurementPackageBasketModel>(async (item) => await SwipeViewAsync(item));
			PerformSearchCommand = new Command<Entry>(async (barcodeEntry) => await PerformSearchAsync(barcodeEntry));
			LoadPageCommand = new Command(async () => await LoadPageAsync());

			Items.Clear();
		}

		public Page CurrentPage { get; set; } = null!;

        public Command ShowProductViewCommand { get; }

        public Command<ProcurementPackageBasketModel> DeleteItemCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command SwipeItemCommand { get; }
        public Command PerformSearchCommand { get; }
        public Command LoadPageCommand { get; }

        private async Task LoadPageAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

				_userDialogs.ShowLoading("Loading...");
				await Task.Delay(500);
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				var barcode = CurrentPage.FindByName<Entry>("barcodeEntry");
                if(barcode is not null)
                {
                    barcode.Text = string.Empty;
                    barcode.Focus();
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


        private async Task ShowProductViewAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(ProcurementSalesPackageProductListView)}");
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
                var result = await _packageProductService.GetObjects(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
					search: barcodeEntry.Text,
                    skip: 0,
                    take: 1
                );

                if(result.IsSuccess)
                {
					if (!(result.Data.Count() > 0))
					{
						if (_userDialogs.IsHudShowing)
							_userDialogs.HideHud();

						_userDialogs.ShowToast(message: $"{barcodeEntry.Text} barkodunda ürün bulunamadı.");
						return;
					}

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<PackageProductModel>(item);
                        var packageBasket = new ProcurementPackageBasketModel
                        {
                            PackageProductModel = obj
                        };

						Items.Add(packageBasket);
                    }
                }
			}
            catch (Exception ex) 
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            finally
            {
                barcodeEntry.Text = string.Empty;
                barcodeEntry.Focus();
				IsBusy = false;
            }
        }

        private async Task ItemTappedAsync(ProcurementPackageBasketModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

				var confirm = await _userDialogs.ConfirmAsync("Koliye malzeme eklenecektir. Devam etmek istiyor musunuz?", "Bilgilendirme", "Evet", "Hayır");

				if (!confirm)
				{
					return;
				}

				SelectedPackageBasketModel = item;
                await Shell.Current.GoToAsync($"{nameof(ProcurementSalesProcessProductBasketView)}", new Dictionary<string, object>
                {
                    {nameof(ProcurementPackageBasketModel), SelectedPackageBasketModel},
                    {nameof(ProcurementSalesCustomerModel), ProcurementSalesCustomerModel}
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

        private async Task DeleteItemAsync(ProcurementPackageBasketModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var result = await _userDialogs.ConfirmAsync($"{item.PackageProductModel.Code}\n{item.PackageProductModel.Name}\nİlgili koli ürünleri ile sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                    return;

                item.PackageProducts.Clear();
				Items.Remove(item);

				//foreach (var basketItem in Items.ToList())
				//{
				//	if (basketItem.PackageProductModel.ReferenceId == item.PackageProductModel.ReferenceId)
				//	{
				//		basketItem.PackageProducts.Clear();
				//		Items.Remove(basketItem);
				//	}
				//}

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
                if (Items.All(item => item.PackageProducts.Count == 0))
                {
                    await _userDialogs.AlertAsync("Kolilerin içinde ürün bulunmamaktadır.", "Hata", "Tamam");
                    return;
                }

                SelectedItems.Clear();

                foreach (var item in Items)
                {
                    if(item.PackageProducts.Count > 0)
                        SelectedItems.Add(item);
                }

				CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

				await Shell.Current.GoToAsync($"{nameof(ProcurementSalesProcessFormView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(ProcurementSalesCustomerModel)] = ProcurementSalesCustomerModel,
                    [nameof(Items)] = SelectedItems,
                    ["CollectedProducts"] = CollectedProducts
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

        private async Task SwipeViewAsync(ProcurementPackageBasketModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                
                await Task.Delay(200);
                SelectedPackageBasketModel = item;

                CurrentPage.FindByName<BottomSheet>("productsBottomSheet").State = BottomSheetState.HalfExpanded;

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
            catch (System.Exception)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert("Bir hata oluştu. Lütfen tekrar deneyiniz.", "Hata", "Tamam");
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
                }

                CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

				await Shell.Current.GoToAsync("..");
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
}
