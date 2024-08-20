using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels
{
    [QueryProperty(name: nameof(SupplierDetailModel), queryId: nameof(SupplierDetailModel))]
    public partial class SupplierDetailViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ICustomQueryService _customQueryService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private SupplierDetailModel supplierDetailModel = null!;

        public SupplierDetailViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
        {
            Title = "Tedarikçi Detayı";

            _httpClientService = httpClientService;
            _customQueryService = customQueryService;
            _userDialogs = userDialogs;

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        }

        public Command LoadItemsCommand { get; }

        private async Task LoadItemsAsync()
        {
            try
            {
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();
                await Task.Delay(1000);

                // Querylerde yer alan firma numarasi dinamik olarak alinacak
                await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync(httpClient));
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata...");
            }
            finally
            {
                Console.WriteLine(SupplierDetailModel);
                IsBusy = false;
            }
        }

        private async Task GetInputOutputQuantityAsync(HttpClient httpClient)
        {
            try
            {
                var query = @$"SELECT
                    [InputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_001_02_STLINE WHERE IOCODE IN(1, 2) AND CLIENTREF = {SupplierDetailModel.Supplier.ReferenceId}),
                    [OutputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_001_02_STLINE WHERE IOCODE IN(3, 4) AND CLIENTREF = {SupplierDetailModel.Supplier.ReferenceId})";

                var result = await _customQueryService.GetObjectAsync(httpClient, query);

                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;
                    var obj = Mapping.Mapper.Map<SupplierDetailModel>(result.Data);
                    SupplierDetailModel.InputQuantity = obj.InputQuantity;
                    SupplierDetailModel.OutputQuantity = obj.OutputQuantity;
                }
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata");
            }
        }

        private async Task GetLastTransactionsAsync(HttpClient httpclient)
        {
            try
            {
                var query = @$"SELECT Top 5

        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[FicheReferenceId] = STFICHE.LOGICALREF,
        [FicheCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
        [WarehouseName] = CAPIWHOUSE.NAME,
		[CurrentReferenceId] = CLCARD.LOGICALREF,
		[CurrentCode] = CLCARD.CODE,
		[CurrentName] = CLCARD.DEFINITION_
        FROM LG_001_02_STLINE AS STLINE
        LEFT JOIN LG_001_02_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_001_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_001_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_001_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF AND MAINUNIT = 1
        LEFT JOIN LG_001_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 1
		WHERE STLINE.IOCODE IN (1,2,3,4) AND STFICHE.TRCODE IN (1,2,3,7,6,8)  AND  CLCARD.LOGICALREF = {SupplierDetailModel.Supplier.ReferenceId}  ORDER BY STLINE.DATE_ DESC";

                var result = await _customQueryService.GetObjectsAsync(httpclient, query);

                if (result.IsSuccess)
                {
                    if (result.Data == null)
                        return;

                    foreach (var item in result.Data)
                    {
                        SupplierDetailModel.LastTransactions.Add(Mapping.Mapper.Map<SupplierTransaction>(item));
                    }
                }
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata");
            }
        }
    }
}