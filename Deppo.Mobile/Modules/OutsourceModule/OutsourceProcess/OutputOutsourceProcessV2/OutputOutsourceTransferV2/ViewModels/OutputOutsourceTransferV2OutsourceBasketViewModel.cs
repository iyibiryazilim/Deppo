using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels
{


    [QueryProperty(name: nameof(OutputOutsourceTransferV2BasketModel), queryId: nameof(OutputOutsourceTransferV2BasketModel))]

    public partial class OutputOutsourceTransferV2OutsourceBasketViewModel : BaseViewModel
    {

        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly ILocationTransactionService _locationTransactionService;
        private readonly IOutputOutsourceTransferV2Service _outputOutsourceTransferV2Service;
        private readonly IProductService _productService;


        [ObservableProperty]
        OutputOutsourceTransferV2BasketModel? outputOutsourceTransferV2BasketModel;

        [ObservableProperty]
        private OutputOutsourceTransferV2ProductModel selectedProduct;

        [ObservableProperty]
        GroupLocationTransactionModel? selectedLocationTransaction;

        [ObservableProperty]
        OutputOutsourceTransferV2SubProductModel outputOutsourceTransferV2SubProductModel;

        [ObservableProperty]
        public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();
        public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();
        public ObservableCollection<LocationModel> Items { get; } = new();
        public ObservableCollection<LocationModel> SelectedSearchItems { get; } = new();
        public ObservableCollection<LocationModel> SelectedItems { get; } = new();
        public ObservableCollection<OutputOutsourceTransferV2SubProductModel> SubProducts { get; } = new();

        [ObservableProperty]
        private LocationModel selectedItem;

        public Page CurrentPage { get; set; }

        [ObservableProperty]
        public SearchBar searchText;




        public OutputOutsourceTransferV2OutsourceBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, IOutputOutsourceTransferV2Service outputOutsourceTransferV2Service)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _locationTransactionService = locationTransactionService;
            _outputOutsourceTransferV2Service = outputOutsourceTransferV2Service;

            Title = "Ürün - Reçete";


            IncreaseCommand = new Command(async () => await IncreaseAsync());
            DecreaseCommand = new Command(async () => await DecreaseAsync());
            NextViewCommand = new Command(async () => await NextViewAsync());
            BackCommand = new Command(async () => await BackAsync());

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());

            //SelectedProduct = new OutputOutsourceTransferV2ProductModel();
            //SelectedProduct.SubProducts.Add(new OutputOutsourceTransferV2SubProductModel
            //{
            //    ProductCode = "Code1",
            //    ProductName = "Name1",
            //    Quantity = 3

            //});
            //SelectedProduct.SubProducts.Add(new OutputOutsourceTransferV2SubProductModel
            //{
            //    ProductCode = "Code2",
            //    ProductName = "Name2",
            //    Quantity = 2

            //});
            //SelectedProduct.SubProducts.Add(new OutputOutsourceTransferV2SubProductModel
            //{
            //    ProductCode = "Code3",
            //    ProductName = "Name3",
            //    Quantity = 4

            //});

            //Console.WriteLine($"SubProducts Count: {SelectedProduct.SubProducts.Count}");



        }

        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }

        public Command LoadItemsCommand { get; }


        private async Task LoadItemsAsync()
        {
            if (OutputOutsourceTransferV2BasketModel is null)
                return;

            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                _userDialogs.Loading("Yükleniyor...");
                OutputOutsourceTransferV2BasketModel?.OutputOutsourceTransferSubProducts?.Clear();
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _outputOutsourceTransferV2Service.GetObjectSubProducts(httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    productReferenceId: OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.ProductReferenceId,
                    search: "",
                    skip: 0,
                    take: 20

                    );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var product in result.Data)
                    {
                        var item = Mapping.Mapper.Map<OutputOutsourceTransferV2SubProductModel>(product);
                        var matchedItem = SubProducts.FirstOrDefault(x => x.ProductReferenceId == item.ProductReferenceId);
                        if (matchedItem is not null)
                            item.IsSelected = matchedItem.IsSelected;
                        else
                            item.IsSelected = false;

                        SubProducts.Add(item);
                    }
                }


                _userDialogs.Loading().Hide();




            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }



        private async Task IncreaseAsync()
        {
            if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel is null)
                return;
            if (IsBusy)
                return;

            try
            {
                if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel is not null)
                {
                    if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity >= 0)
                    {
                        OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity++;
                        foreach (var subProduct in SubProducts)
                        {
                            subProduct.Quantity = subProduct.BomQuantity * OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity;
                        }
                    }
                    else
                    {

                    }
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

        private async Task DecreaseAsync()
        {
            if (IsBusy)
                return;

            try
            {
                if (OutputOutsourceTransferV2BasketModel is not null)
                {
                    if (OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity == 0)
                    {
                        await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
                    }
                    else
                    {
                        OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.Quantity--;
                    }
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

        private async Task NextViewAsync()
        {
            if (IsBusy)
                return;



            if (OutputOutsourceTransferV2BasketModel is null && OutputOutsourceTransferV2BasketModel.OutsourceWarehouseModel is null && OutputOutsourceTransferV2BasketModel.OutsourceModel is null)
                return;
            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2OutsourceFormView)}", new Dictionary<string, object>
                {
                    [nameof(OutputOutsourceTransferV2BasketModel)] = OutputOutsourceTransferV2BasketModel,

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


                if (OutputOutsourceTransferV2BasketModel is not null)
                {
                    OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferMainProductModel.IsSelected = false;
                    OutputOutsourceTransferV2BasketModel = null;
                }




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
}
