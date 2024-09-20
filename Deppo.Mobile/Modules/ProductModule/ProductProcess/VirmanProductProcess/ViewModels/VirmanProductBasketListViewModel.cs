using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

[QueryProperty(name: nameof(VirmanBasketModel), queryId: nameof(VirmanBasketModel))]
public partial class VirmanProductBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocationService _locationService;



    [ObservableProperty]
    private VirmanBasketModel virmanBasketModel= null!;

    public ObservableCollection<SeriLotTransactionModel> SelectedSeriLotTransactions { get; } = new();
    [ObservableProperty]
    SeriLotTransactionModel? selectedSeriLotTransaction;
     public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();


    public VirmanProductBasketListViewModel(IHttpClientService httpClientService
        , ISeriLotTransactionService serilotTransactionService
        , IUserDialogs userDialogs
        , IProductService productService
        , IServiceProvider serviceProvider,
ILocationService locationService)
    {
        _httpClientService = httpClientService;
        _serilotTransactionService = serilotTransactionService;
        _userDialogs = userDialogs;
        _productService = productService;
        Title = "Virman Sepet Listesi";
        _serviceProvider = serviceProvider;
        _locationService = locationService;
        Title = "Sepet Listesi";

        IncreaseCommand = new Command<VirmanBasketModel>(async (item) => await IncreaseAsync(item));
        DecreaseCommand = new Command<VirmanBasketModel>(async (item) => await DecreaseAsync(item));
       // DeleteItemCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await DeleteItemAsync(item));
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());


        LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
        SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionIncreaseAsync(item));
        SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionDecreaseAsync(item));
        ConfirmSeriLotTransactionCommand = new Command(ConfirmSeriLotTransactionAsync);
        SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());



        //LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
        //LocationCloseCommand = new Command(async () => await LocationCloseAsync());
        //LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
        //LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
        //LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));


    }
    public Page CurrentPage { get; set; } = null!;

    public Command ShowProductViewCommand { get; }
    public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command DeleteItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }



    public Command LoadMoreSeriLotTransactionsCommand { get; }
    public Command SeriLotTransactionIncreaseCommand { get; }
    public Command SeriLotTransactionDecreaseCommand { get; }
    public Command ConfirmSeriLotTransactionCommand { get; }
    public Command SeriLotTransactionCloseCommand { get; }




 	public Command LoadMoreLocationsCommand { get; }
	public Command<LocationModel> LocationDecreaseCommand { get; }
	public Command<LocationModel> LocationIncreaseCommand { get; }
	public Command<LocationModel> LocationConfirmCommand { get; }
	public Command LocationCloseCommand { get; }



   




    private async Task DecreaseAsync(VirmanBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;




            if(item.OutVirmanQuantity >0)
            {
                if (item.OutVirmanProduct.TrackingType == 1 || item.OutVirmanProduct.TrackingType == 2)
                {
                    await LoadSeriLotTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {

                    item.OutVirmanQuantity = item.OutVirmanQuantity + 1;
                    item.InVirmanQuantity = item.OutVirmanQuantity;
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

    private async Task IncreaseAsync(VirmanBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
                 if ( item.OutVirmanProduct.TrackingType == 1 || item.OutVirmanProduct.TrackingType == 2)
                {
                    await LoadSeriLotTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                   
                        item.OutVirmanQuantity = item.OutVirmanQuantity +1;
                        item.InVirmanQuantity = item.OutVirmanQuantity;
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




    private async Task LoadSeriLotTransactionsAsync()
    {

        try
        {
            _userDialogs.ShowLoading("Load Serilot Items...");
            await Task.Delay(1000);
            SelectedSeriLotTransactions.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: VirmanBasketModel.OutVirmanProduct.ReferenceId, warehouseNumber: VirmanBasketModel.OutVirmanWarehouse.Number, search: string.Empty);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SelectedSeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
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

    private async Task LoadMoreSeriLotTransactionsAsync()
    {

        try
        {
            _userDialogs.ShowLoading("Load Serilot Items...");
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: VirmanBasketModel.OutVirmanProduct.ReferenceId, warehouseNumber: VirmanBasketModel.OutVirmanWarehouse.Number,take:20,skip:SelectedSeriLotTransactions.Count ,search: string.Empty);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SelectedSeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
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

    private void SeriLotTransactionIncreaseAsync(SeriLotTransactionModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item is not null)
            {
                selectedSeriLotTransaction = item;
                if (item.OutputQuantity < item.Quantity)
                {
                    item.OutputQuantity++;

                    if (item.OutputQuantity > 0 && !item.IsSelected)
                        item.IsSelected = true;
                }
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SeriLotTransactionDecreaseAsync(SeriLotTransactionModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item is not null)
            {
                SelectedSeriLotTransaction = item;
                if (item.OutputQuantity == 0)
                    item.IsSelected = false;
                if (item.OutputQuantity > 0)
                {
                    item.OutputQuantity--;
                }
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ConfirmSeriLotTransactionAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (SelectedSeriLotTransactions.Count > 0)
            {
                SelectedSeriLotTransactions.ToList().AddRange(SeriLotTransactions.Where(x => x.OutputQuantity > 0));

                if (virmanBasketModel.OutVirmanProduct.LocTracking == 0)
                {

                    foreach (var item in SelectedSeriLotTransactions)
                    {
                        virmanBasketModel.OutVirmanProduct.SeriLotTransactionModels.Add(new SeriLotTransactionModel
                        {
                             ReferenceId = item.ReferenceId,
                             SerilotCode = item.SerilotCode,
                             SerilotName = item.SerilotName,
                             TransactionFicheReferenceId = item.TransactionFicheReferenceId,
                             TransactionReferenceId = item.TransactionReferenceId,
                             InTransactionReferenceId = item.InTransactionReferenceId,
                             Quantity = item.OutputQuantity,
                             InSerilotTransactionReferenceId = item.InSerilotTransactionReferenceId,
                             RemainingQuantity = item.Quantity - item.OutputQuantity

                        }); 
                           
                        
                    }


                    var totalOutputQuantity = SeriLotTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
                    VirmanBasketModel.OutVirmanQuantity = totalOutputQuantity;
                    VirmanBasketModel.InVirmanQuantity = totalOutputQuantity;

                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
                }
                
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SeriLotTransactionCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

           

            
            if (VirmanBasketModel.OutVirmanQuantity == 0 || VirmanBasketModel.OutVirmanQuantity  == 0)
            {
                await _userDialogs.AlertAsync("0 Miktarda Bir Ürüne Virman İşlemi Uygulanamaz.", "Uyarı", "Tamam");
                return;
            }


            await Shell.Current.GoToAsync($"{nameof(ReturnPurchaseDispatchFormView)}", new Dictionary<string, object>
            {
                [nameof(VirmanBasketModel)] = VirmanBasketModel
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

            var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürün silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!result)
                return;

            
           //     VirmanBasketModel.IsSelected = false;
            
         //   foreach (var item in SelectedPurchaseTransactions)
          //  {
            //    item.IsSelected = false;
          ///  }
          //  SelectedLocationTransactions.Clear();
          virmanBasketModel.OutVirmanQuantity = 0;
            virmanBasketModel.InVirmanQuantity = 0;
            virmanBasketModel.OutVirmanProduct = null!;
            virmanBasketModel.InVirmanProduct = null!;
            virmanBasketModel.OutVirmanWarehouse = null!;
            virmanBasketModel.InVirmanWarehouse = null!;
            virmanBasketModel = null;
            SelectedSeriLotTransactions.Clear();
          //  SelectedPurchaseTransactions.Clear();
         //   Items.Clear();
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
    /*
    private async Task DeleteItemAsync(ReturnPurchaseBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

            if (!result)
                return;

            if (SelectedItem == item)
            {
                SelectedItem.IsSelected = false;
                SelectedItem = null;
            }

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
    }*/

}
