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
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    [QueryProperty(name: nameof(OutsourceModel), queryId: nameof(OutsourceModel))]
    [QueryProperty(name: nameof(ShipAddressModel), queryId: nameof(ShipAddressModel))]
    [QueryProperty(name: nameof(OutputOutsourceTransferV2ProductModel), queryId: nameof(OutputOutsourceTransferV2ProductModel))]

    public partial class OutputOutsourceTransferV2OutsourceBasketViewModel :BaseViewModel    
    {

        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly ILocationTransactionService _locationTransactionService;
       
        private readonly IProductService _productService;
        

        [ObservableProperty]
        WarehouseModel? warehouseModel;

        [ObservableProperty]
        OutsourceModel? outsourceModel;

        [ObservableProperty]
        ShipAddressModel? shipAddressModel;

        [ObservableProperty]
        OutputOutsourceTransferV2ProductModel? outputOutsourceTransferV2ProductModel;

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




        public OutputOutsourceTransferV2OutsourceBasketViewModel(IHttpClientService httpClientService,IUserDialogs userDialogs, ILocationTransactionService locationTransactionService)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _locationTransactionService = locationTransactionService;
           
            Title = "Ürün - Reçete";


            IncreaseCommand = new Command(async () => await IncreaseAsync());
            DecreaseCommand = new Command(async () => await DecreaseAsync());
            NextViewCommand = new Command(async () => await NextViewAsync());
                BackCommand = new Command(async () => await BackAsync());

            SelectedProduct = new OutputOutsourceTransferV2ProductModel();
            SelectedProduct.SubProducts.Add(new OutputOutsourceTransferV2SubProductModel
            {
                ProductCode = "Code1",
                ProductName = "Name1",
                Quantity = 3

            });
            SelectedProduct.SubProducts.Add(new OutputOutsourceTransferV2SubProductModel
            {
                ProductCode = "Code2",
                ProductName = "Name2",
                Quantity = 2

            });
            SelectedProduct.SubProducts.Add(new OutputOutsourceTransferV2SubProductModel
            {
                ProductCode = "Code3",
                ProductName = "Name3",
                Quantity = 4

            });

            Console.WriteLine($"SubProducts Count: {SelectedProduct.SubProducts.Count}");



        }

        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }


        
      


        private async Task IncreaseAsync()
        {
            if (OutputOutsourceTransferV2ProductModel is null)
                return;
            if (IsBusy)
                return;

            try
            {
                if(OutputOutsourceTransferV2ProductModel is not null)
                {
                    if (OutputOutsourceTransferV2ProductModel.Quantity >=0)
                    {
                        OutputOutsourceTransferV2ProductModel.Quantity++;
                        foreach (var subProduct in SelectedProduct.SubProducts)
                        {
                            subProduct.Quantity = subProduct.BomQuantity * SelectedProduct.Quantity;
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
                if (OutputOutsourceTransferV2ProductModel is not null)
                {
                    if (OutputOutsourceTransferV2ProductModel.Quantity == 0)
                    {
                        await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
                    }
                    else
                    {
                        OutputOutsourceTransferV2ProductModel.Quantity--;
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
            
           

            if (OutputOutsourceTransferV2ProductModel is null && WarehouseModel is null && OutsourceModel is null)
                return;
            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2OutsourceFormView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(OutsourceModel)] = OutsourceModel,
                    [nameof(ShipAddressModel)]= ShipAddressModel,
                    [nameof(InputOutsourceTransferProductModel)] = OutputOutsourceTransferV2ProductModel
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


                if (OutputOutsourceTransferV2ProductModel is not null)
                {
                    OutputOutsourceTransferV2ProductModel.IsSelected = false;
                    OutputOutsourceTransferV2ProductModel = null;
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
