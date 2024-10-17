using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels
{
    [QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
    public partial class ProcurementSalesProcessProductBasketViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcurementSalesProductService _procurementSalesProductService;
        [ObservableProperty]
        private ProcurementPackageBasketModel? procurementPackageBasketModel;

        [ObservableProperty]
        ProcurementSalesCustomerModel procurementSalesCustomerModel = null!;

        [ObservableProperty]
        private ProcurementSalesProductModel selectedSalesProductModel;

        public ObservableCollection<ProcurementSalesProductModel> Items { get; } = new();


        public ProcurementSalesProcessProductBasketViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IServiceProvider serviceProvider, IProcurementSalesProductService procurementSalesProductService)
        {
            _userDialogs = userDialogs;
            _httpClientService = httpClientService;
            _serviceProvider = serviceProvider;

            Title = "Koliye Eklenecek Ürünler";

            ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
            DeleteItemCommand = new Command<ProcurementSalesProductModel>(async (item) => await DeleteItemAsync(item));
            ConfirmCommand = new Command(async () => await ConfirmAsync());
            BackCommand = new Command(async () => await BackAsync());
            IncreaseCommand = new Command<ProcurementSalesProductModel>(async (item) => await IncreaseAsync(item));
            DecreaseCommand = new Command<ProcurementSalesProductModel>(async (item) => await DecreaseAsync(item));
            PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));

            Items.Clear();
            _procurementSalesProductService = procurementSalesProductService;
        }

        public Page CurrentPage { get; set; } = null!;

        public Command ShowProductViewCommand { get; }

        public Command<ProcurementSalesProductModel> DeleteItemCommand { get; }

        public Command ConfirmCommand { get; }
        public Command BackCommand { get; }
        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }
        public Command PerformSearchCommand { get; }

        [ObservableProperty]
        bool isFind = false;

        [ObservableProperty]
        public Entry barcodeEntry;

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

        private async Task IncreaseAsync(ProcurementSalesProductModel procurementSalesProductModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedSalesProductModel = procurementSalesProductModel;

                if(SelectedSalesProductModel.Quantity > SelectedSalesProductModel.OutputQuantity)
                    SelectedSalesProductModel.OutputQuantity++;
                
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

                var previouseViewModel = _serviceProvider.GetRequiredService<ProcurementSalesPackageBasketViewModel>();
                if (previouseViewModel is not null)
                {
                    foreach (var item in Items.Where(x=>x.OutputQuantity > 0))
                       previouseViewModel.SelectedPackageBasketModel?.PackageProducts.Add(item);

                    await Shell.Current.GoToAsync($"..");
                }
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
                    var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                    if (!result)
                        return;

                    Items.Clear();
                    await Shell.Current.GoToAsync("..");
                }
                else
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

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await SearchByCode(barcodeEntry, httpClient);

            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }
            finally
            {
                BarcodeEntry.Text = string.Empty;
                IsBusy = false;
            }
        }

        private async Task SearchByCode(Entry barcodeEntry, HttpClient httpClient)
        {
            var result = await _procurementSalesProductService.SearchByItemCode(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProcurementSalesCustomerModel.ReferenceId, barcodeEntry.Text);

            if (result.IsSuccess)
            {
                if (result.Data is not null && result.Data.Count != 0)
                {
                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<ProcurementSalesProductModel>(item);

                        if (Items.Count == 0)
                        {
                            obj.OutputQuantity++;
                            Items.Add(obj);

                        }
                        else
                        {
                            foreach (var product in Items)
                            {
                                if (product.ItemReferenceId == obj.ItemReferenceId && product.Quantity > product.OutputQuantity)
                                    product.OutputQuantity++;
                                else if (product.ItemReferenceId != obj.ItemReferenceId)
                                {
                                    obj.OutputQuantity++;
                                    Items.Add(obj);

                                }
                            }
                        }
                    }

                }
                else
                {
                    await _userDialogs.AlertAsync("Ürün Bulunamadı..", "Uyarı", "Tamam");
                }
            }
        }
        //private async Task SearchByBarcode(Entry barcodeEntry, HttpClient httpClient)
        //{
        //    if (!IsFind)
        //    {
        //        try
        //        {
        //            var result = await _procurementSalesProductService.SearchByBarcode(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, ProcurementSalesCustomerModel.ReferenceId, barcodeEntry.Text);

        //            if (result.IsSuccess)
        //            {
        //                if (result.Data is not null && result.Data.Count != 0)
        //                {
        //                    IsFind = true;
        //                    foreach (var item in result.Data)
        //                    {
        //                        var obj = Mapping.Mapper.Map<ProcurementSalesProductModel>(item);

        //                        if (Items.Count == 0)
        //                        {
        //                            obj.OutputQuantity++;
        //                            Items.Add(obj);

        //                        }
        //                        else
        //                        {
        //                            foreach (var product in Items)
        //                            {
        //                                if (product.ItemReferenceId == obj.ItemReferenceId && product.Quantity > product.OutputQuantity)
        //                                    product.OutputQuantity++;
        //                                else if (product.ItemReferenceId != obj.ItemReferenceId)
        //                                {
        //                                    obj.OutputQuantity++;
        //                                    Items.Add(obj);

        //                                }
        //                            }
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    await _userDialogs.AlertAsync("Ürün Bulunamadı..", "Uyarı", "Tamam");
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //        finally
        //        {

        //        }

        //    }
           
        //}
    }
}
