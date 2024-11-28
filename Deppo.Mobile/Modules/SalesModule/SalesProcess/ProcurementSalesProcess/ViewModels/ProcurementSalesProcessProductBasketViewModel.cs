using Android.Content;
using Android.Views.InputMethods;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels
{
    [QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
    [QueryProperty(nameof(ProcurementPackageBasketModel), nameof(ProcurementPackageBasketModel))]
    public partial class ProcurementSalesProcessProductBasketViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcurementSalesProductService _procurementSalesProductService;

		[ObservableProperty]
		ProcurementSalesCustomerModel procurementSalesCustomerModel = null!;

		[ObservableProperty]
        private ProcurementPackageBasketModel? procurementPackageBasketModel;


        [ObservableProperty]
        private ProcurementSalesProductModel selectedSalesProductModel;

        public ObservableCollection<ProcurementSalesProductModel> Items { get; } = new();

		[ObservableProperty]
		bool isFind = false;

		
		public ProcurementSalesProcessProductBasketViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IServiceProvider serviceProvider, IProcurementSalesProductService procurementSalesProductService)
        {
            _userDialogs = userDialogs;
            _httpClientService = httpClientService;
            _serviceProvider = serviceProvider;
            _procurementSalesProductService = procurementSalesProductService;

            Title = "Koliye Eklenecek Ürünler";

            ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
            DeleteItemCommand = new Command<ProcurementSalesProductModel>(async (item) => await DeleteItemAsync(item));
            ConfirmCommand = new Command(async () => await ConfirmAsync());
            BackCommand = new Command(async () => await BackAsync());
            IncreaseCommand = new Command<ProcurementSalesProductModel>(async (item) => await IncreaseAsync(item));
            DecreaseCommand = new Command<ProcurementSalesProductModel>(async (item) => await DecreaseAsync(item));
            PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
			QuantityTappedCommand = new Command<ProcurementSalesProductModel>(async (item) => await QuantityTappedAsync(item));
            LoadPageCommand = new Command(async () => await LoadPageAsync());
        }

        public Page CurrentPage { get; set; } = null!;

		[ObservableProperty]
		public Entry barcodeEntry;

		public Command ShowProductViewCommand { get; }

        public Command<ProcurementSalesProductModel> DeleteItemCommand { get; }

        public Command ConfirmCommand { get; }
        public Command BackCommand { get; }
        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }
        public Command PerformSearchCommand { get; }
        public Command QuantityTappedCommand { get; }
        public Command LoadPageCommand { get; }

        

        private async Task LoadPageAsync()
        {
            
            try
            {
                IsBusy = true;

				_userDialogs.ShowLoading("Loading...");
                Items.Clear();
				await Task.Delay(1000);

                var barcode = CurrentPage.FindByName<Entry>("barcodeEntry");
                

				if (ProcurementPackageBasketModel is not null && ProcurementPackageBasketModel.PackageProducts.Any())
                {
                    foreach (var item in ProcurementPackageBasketModel.PackageProducts)
                    {
                        Items.Add(item);
                    }
                }

                if(_userDialogs.IsHudShowing)
				    _userDialogs.HideHud();

				if (barcode is not null)
				{
					barcode.Text = string.Empty;
					barcode.Focus();
				}

			}
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
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


                await Shell.Current.GoToAsync($"{nameof(ProcurementSalesProcessBasketProductListView)}", new Dictionary<string, object>
                {
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


        private async Task DeleteItemAsync(ProcurementSalesProductModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili koli ürünleri ile sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
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

		private async Task QuantityTappedAsync(ProcurementSalesProductModel item)
		{
			if (IsBusy)
				return;
			if (item is null)
				return;
			try
			{
				IsBusy = true;

                SelectedSalesProductModel = item;

				var result = await CurrentPage.DisplayPromptAsync(
					title: item.ItemCode,
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

				if (quantity > item.Quantity)
				{
					await _userDialogs.AlertAsync("Girilen miktar, toplanan miktarı aşmamalıdır.", "Hata", "Tamam");
					return;
				}

				var packageBasketViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
				if (packageBasketViewModel is not null)
				{
					double totalOutputQuantity = 0;

					foreach (var package in packageBasketViewModel.Items)
					{
						foreach (var product in package.PackageProducts)
						{
							if (product.ReferenceId == SelectedSalesProductModel.ReferenceId)
							{
								totalOutputQuantity += product.OutputQuantity;
							}
						}
					}

					if (totalOutputQuantity + quantity > SelectedSalesProductModel.Quantity)
					{
						await _userDialogs.AlertAsync("Ürünün toplam toplanabilir miktarını aştınız!");
						return;
					}
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

		private async Task IncreaseAsync(ProcurementSalesProductModel procurementSalesProductModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedSalesProductModel = procurementSalesProductModel;

				var packageBasketViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
                if (packageBasketViewModel is not null)
                {
                    double totalOutputQuantity = 0;

                    foreach (var package in packageBasketViewModel.Items)
                    {
                        foreach (var product in package.PackageProducts)
                        {
                            if (product.ReferenceId == SelectedSalesProductModel.ReferenceId)
                            {
                                totalOutputQuantity += product.OutputQuantity;
                            }
                        }
                    }

                    if (totalOutputQuantity + SelectedSalesProductModel.OutputQuantity  >= SelectedSalesProductModel.Quantity)
                    {
                        await _userDialogs.AlertAsync("Ürünün toplam toplanabilir miktarını aştınız!");
                        return;
                    }
                    
                    if (SelectedSalesProductModel.Quantity > SelectedSalesProductModel.OutputQuantity)
                    {
                        SelectedSalesProductModel.OutputQuantity++;
                    }
                }

			}
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DecreaseAsync(ProcurementSalesProductModel procurementSalesProductModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedSalesProductModel = procurementSalesProductModel;

                if (SelectedSalesProductModel.OutputQuantity > 0)
                    SelectedSalesProductModel.OutputQuantity--;

            }
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message);
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

                var previousViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
                if (previousViewModel is not null && previousViewModel.SelectedPackageBasketModel is not null)
                {
                    foreach (var item in Items.Where(x=>x.OutputQuantity > 0))
                    {
                        if(!previousViewModel.SelectedPackageBasketModel.PackageProducts.Any(x => x.ReferenceId == item.ReferenceId))
                        {
						    previousViewModel.SelectedPackageBasketModel?.PackageProducts.Add(item);
                        }
					}   
                }

				await Shell.Current.GoToAsync($"..");
				Items.Clear();
                
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
                    Items.Clear();
                }
                
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

        private async Task PerformSearchAsync(Entry barcodeEntry)
        {
            if (IsBusy)
                return;

			try
			{
                if (string.IsNullOrEmpty(barcodeEntry.Text))
                    return;

                IsBusy = true;

                bool isFind = false;

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var results = await Task.WhenAll(SearchByCode(barcodeEntry.Text, httpClient, isFind), SearchByBarcode(barcodeEntry.Text, httpClient, isFind));
				ProcurementSalesProductModel? foundResult = results.FirstOrDefault(i => i != null);

				if(foundResult != null) {
					if (!Items.Any(item => item.ItemReferenceId == foundResult.ItemReferenceId))
					{
                        foundResult.OutputQuantity = 0;
						Items.Add(foundResult);
					}
                    else
                    {
						var existingItem = Items.FirstOrDefault(item => item.ItemReferenceId == foundResult.ItemReferenceId);

						if (existingItem != null && existingItem.Quantity > existingItem.OutputQuantity)
						{
							existingItem.OutputQuantity += 1;
						}
                        else if(existingItem != null && existingItem.Quantity <= existingItem.OutputQuantity)
                        {
                            _userDialogs.ShowToast($"Miktar, toplanan miktardan fazla olamaz");
                        }
					}
				}
                else
                {
					_userDialogs.ShowToast($"{barcodeEntry.Text} barkodunda herhangi bir ürün bulunamadı");
					barcodeEntry.Text = string.Empty;
					BarcodeEntry.Text = string.Empty;
					barcodeEntry.Focus();
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
                barcodeEntry.Text = string.Empty;
				BarcodeEntry.Text = string.Empty;
                barcodeEntry.Focus();
				IsBusy = false;
            }
        }

        private async Task<ProcurementSalesProductModel> SearchByCode(string barcode, HttpClient httpClient, bool isFind)
        {
            try
            {
				if (!isFind)
				{
					var result = await _procurementSalesProductService.SearchByItemCode(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProcurementSalesCustomerModel.ReferenceId, barcode);

					if (result.IsSuccess)
					{
						if (result.Data is not null && result.Data.Count != 0)
						{
							string jsonString = result.Data.ToString();
							var procurementSalesProductModelList = JsonConvert.DeserializeObject<List<ProcurementSalesProductModel>>(jsonString);
							var procurementSalesProductModel = procurementSalesProductModelList?.First();
							isFind = true;
							return procurementSalesProductModel;
						}
					}
				}
				return null;
			}
            catch (Exception ex)
            {
                throw;
			}
           
           
        }
        private async Task<ProcurementSalesProductModel> SearchByBarcode(string barcode, HttpClient httpClient, bool isFind)
        {
            try
            {
                if(!isFind)
                {
					var result = await _procurementSalesProductService.SearchByBarcode(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProcurementSalesCustomerModel.ReferenceId, barcode);

					if (result.IsSuccess)
					{
						if (result.Data is not null && result.Data.Count != 0)
						{
							string jsonString = result.Data.ToString();
							var procurementSalesProductModelList = JsonConvert.DeserializeObject<List<ProcurementSalesProductModel>>(jsonString);
							var procurementSalesProductModel = procurementSalesProductModelList?.First();
							isFind = true;
							return procurementSalesProductModel;
						}
					}
				}
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

	}
}
