using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels
{
    public partial class SupplierInputTransactionViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ICustomQueryService _customQueryService;
        private readonly IUserDialogs _userDialogs;

        [ObservableProperty]
        private SupplierDetailModel supplierDetailModel = null!;

        public SupplierInputTransactionViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _customQueryService = customQueryService;
            _userDialogs = userDialogs;
            Title = "Tedarikçi Giriş İşlemleri";

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
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.Loading().Hide();

                _userDialogs.Alert(message: ex.Message, title: "Hata...");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GetLastTransactionsAsync(HttpClient httpclient)
        {
            try
            {
                var query = @$"SELECT
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[TransactionReferenceId] = STFICHE.LOGICALREF,
        [TransactionNumber] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
        [WarehouseName] = CAPIWHOUSE.NAME,
		[SupplierReferenceId] = CLCARD.LOGICALREF,
		[SupplierCode] = CLCARD.CODE,
		[SupplierName] = CLCARD.DEFINITION_
        FROM LG_001_02_STLINE AS STLINE
        LEFT JOIN LG_001_02_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_001_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_001_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_001_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF AND MAINUNIT = 1
        LEFT JOIN LG_001_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 1
		WHERE STLINE.IOCODE IN (1,2,3,4) AND STFICHE.TRCODE IN (1,2,3,7,6,8) AND  CLCARD.LOGICALREF = {SupplierDetailModel.Supplier.ReferenceId}  ORDER BY STLINE.DATE_ DESC";

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