using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ProcurementSalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels
{
    [QueryProperty(nameof(ProcurementSalesCustomerModel), nameof(ProcurementSalesCustomerModel))]
    public partial class ProcurementSalesProcessProductBasketViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IHttpClientService _httpClientService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private ProcurementPackageBasketModel? procurementPackageBasketModel;

        [ObservableProperty]
        ProcurementSalesCustomerModel procurementSalesCustomerModel = null!;

        [ObservableProperty]
        private ProcurementSalesProductModel selectedSalesProductModel;

        public ObservableCollection<ProcurementSalesProductModel> Items { get; } = new();

        public ProcurementSalesProcessProductBasketViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IServiceProvider serviceProvider)
        {
            _userDialogs = userDialogs;
            _httpClientService = httpClientService;
            _serviceProvider = serviceProvider;

            Title = "Sepet Listesi";

            ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
            DeleteItemCommand = new Command<ProcurementSalesProductModel>(async (item) => await DeleteItemAsync(item));
            NextViewCommand = new Command(async () => await NextViewAsync());
            BackCommand = new Command(async () => await BackAsync());
            IncreaseCommand = new Command<ProcurementSalesProductModel>(async (item) => await IncreaseAsync(item));
            DecreaseCommand = new Command<ProcurementSalesProductModel>(async (item) => await DecreaseAsync(item));

            Items.Clear();
        }

        public Page CurrentPage { get; set; } = null!;

        public Command ShowProductViewCommand { get; }

        public Command<ProcurementSalesProductModel> DeleteItemCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }
        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }

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

                //await Shell.Current.GoToAsync($"{nameof(InputProductProcessFormView)}", new Dictionary<string, object>
                //{
                //    [nameof(WarehouseModel)] = WarehouseModel,
                //    [nameof(Items)] = Items
                //});
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
    }
}
