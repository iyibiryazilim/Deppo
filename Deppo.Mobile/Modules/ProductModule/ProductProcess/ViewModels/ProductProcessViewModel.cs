using Android.Graphics.Drawables;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ProcessBottomSheetModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using DevExpress.Maui.Controls;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;

public partial class ProductProcessViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;

    public ProductProcessViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService)
    {
        _userDialogs = userDialogs;
        _httpClientService = httpClientService;

        Title = "Malzeme İşlemleri";

        ProductionInputCommand = new Command(async () => await ProductionInputAsync());
        OverCountCommand = new Command(async () => await OverCountAsync());
        ConsumableProcessCommand = new Command(async () => await ConsumableProcessAsync());
        UnderCountProcessCommand = new Command(async () => await UnderCountProcessAsync());
        WasteProcessCommand = new Command(async () => await WasteProcessAsync());
        VirmanProductProcessCommand = new Command(async () => await VirmanProductProcessAsync());
        TransferProductProcessCommand = new Command(async () => await TransferProductProcessAsync());
        ProductionOpenInfoBottemSheetCommand = new Command(async () => await ProductionOpenInfoBottemSheetAsync());
        DemandProcessCommand = new Command(async () => await DemandProcessAsync());
        OverCountOpenInfoBottemSheetCommand = new Command(async () => await OverCountOpenInfoBottemSheetAsync());
        ConsumableOpenInfoBottemSheetCommand = new Command(async () => await ConsumableOpenInfoBottemSheetAsync());
        UnderCountOpenInfoBottemSheetCommand = new Command(async () => await UnderCountOpenInfoBottemSheetAsync());
        WasteOpenInfoBottemSheetCommand = new Command(async () => await WasteOpenInfoBottemSheetAsync());
        VirmanOpenInfoBottemSheetCommand = new Command(async () => await VirmanOpenInfoBottemSheetAsync());
        TransferProductInfoBottemSheetCommand = new Command(async () => await TransferProductInfoBottemSheetAsync());
        DemandProcessInfoBottemSheetCommand = new Command(async () => await DemandProcessInfoBottemSheetAsync());
    }

    [ObservableProperty]
    private string infoTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ProcessBottomSheetModel processBottomSheetModel = new();

    public Page CurrentPage { get; set; }
    public Command ProductionInputCommand { get; }
    public Command OverCountCommand { get; }
    public Command ConsumableProcessCommand { get; }
    public Command UnderCountProcessCommand { get; }
    public Command WasteProcessCommand { get; }

    public Command VirmanProductProcessCommand { get; }
    public Command TransferProductProcessCommand { get; }
    public Command DemandProcessCommand { get; }

    public Command ProductionOpenInfoBottemSheetCommand { get; }

    public Command OverCountOpenInfoBottemSheetCommand { get; }
    public Command ConsumableOpenInfoBottemSheetCommand { get; }
    public Command UnderCountOpenInfoBottemSheetCommand { get; }
    public Command WasteOpenInfoBottemSheetCommand { get; }

    public Command VirmanOpenInfoBottemSheetCommand { get; }

    public Command TransferProductInfoBottemSheetCommand { get; }

    public Command DemandProcessInfoBottemSheetCommand { get; }

    #region Will be removed

    public Command SuccessPageCommand { get; }
    public Command FailurePageCommand { get; }

    #endregion Will be removed

    private async Task ProductionInputAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(InputProductProcessWarehouseListView)}", new Dictionary<string, object>
        {
            {nameof(InputProductProcessType), InputProductProcessType.ProductionInputProcess}
        });
    }

    private async Task OverCountAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(InputProductProcessWarehouseListView)}", new Dictionary<string, object>
        {
            {nameof(InputProductProcessType), InputProductProcessType.OverCountProcess}
        });
    }

    private async Task ConsumableProcessAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(OutputProductProcessWarehouseListView)}", new Dictionary<string, object>
        {
            {nameof(OutputProductProcessType), OutputProductProcessType.ConsumableProcess}
        });
    }

    private async Task UnderCountProcessAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(OutputProductProcessWarehouseListView)}", new Dictionary<string, object>
        {
            { nameof(OutputProductProcessType), OutputProductProcessType.UnderCountProcess }
        });
    }

    private async Task WasteProcessAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(OutputProductProcessWarehouseListView)}", new Dictionary<string, object>
        {
            { nameof(OutputProductProcessType), OutputProductProcessType.WasteProcess }
        });
    }

    private async Task VirmanProductProcessAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(VirmanProductOutWarehouseListView)}", new Dictionary<string, object>
        {
            //{ nameof(OutputProductProcessType), OutputProductProcessType.WasteProcess }
        });
    }

    private async Task TransferProductProcessAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(TransferOutWarehouseListView)}");
    }

    private async Task DemandProcessAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(DemandProcessWarehouseListView)}");
    }

    private async Task ProductionOpenInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Üretimden Giriş İşlemleri";
        ProcessBottomSheetModel.Description = "Üretimden giriş fişi üretilen malların ambarlara giriş işlemlerini kaydetmek için kullanılır.\nÜretimden giriş işlemleri bölümünün kullanılıması durumunda sarf fişleri program tarafından otomatik oluşturulur.";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task OverCountOpenInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Sayım Fazlası İşlemleri";
        ProcessBottomSheetModel.Description = "Sayım fazlası bilgilerini kaydetmek için kullanılır.";
        ProcessBottomSheetModel.IconText = "plus";
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task ConsumableOpenInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Sarf Fişi İşlemleri";
        ProcessBottomSheetModel.Description = "Üretim sırasında ya da başka nedenlerle oluşan sarfları kaydetmek için kullanılır.\nSarf fişi işlemleri bölümünün kullanılması durumunda sarf fişleri formlarda doldurularak oluşturulur.";

        ProcessBottomSheetModel.IconText = "minus"; // Metni ayrı ayarlayabilirsin
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task UnderCountOpenInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Sayım Eksiği İşlemleri";
        ProcessBottomSheetModel.Description = "Sayım eksiği bilgilerini kaydetmek için kullanılır.";

        ProcessBottomSheetModel.IconText = "minus"; // Metni ayrı ayarlayabilirsin
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task WasteOpenInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Fire Fişi İşlemleri";
        ProcessBottomSheetModel.Description = "Üretim sırasında ya da başka nedenlerle oluşan fireleri kaydetmek için kullanılır.\nSarf fişi işlemleri bölümünün kullanılması durumunda fire fişleri formlarda doldurularak oluşturulur.";

        ProcessBottomSheetModel.IconText = "minus"; // Metni ayrı ayarlayabilirsin
        ProcessBottomSheetModel.IconColor = "#44B0C0";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task VirmanOpenInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Ambar Transferi İşlemleri";
        ProcessBottomSheetModel.Description = "Ambarlar arası malzeme hareketlerini kaydetmek için kullanılır.";

        ProcessBottomSheetModel.IconText = "arrow-right-arrow-left"; // Metni ayrı ayarlayabilirsin
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task TransferProductInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Malzeme Virmanı İşlemleri";
        ProcessBottomSheetModel.Description = "Ambarlar arası malzeme hareketlerini kaydetmek için kullanılır.";

        ProcessBottomSheetModel.IconText = "arrow-right-arrow-left"; // Metni ayrı ayarlayabilirsin
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task DemandProcessInfoBottemSheetAsync()
    {
        ProcessBottomSheetModel.InfoTitle = "Talep İşlemleri";
        ProcessBottomSheetModel.Description = "Talep edilen malzeme ve hizmetlere ait bilgileri talep fişlerine kaydetmek için kullanılır.";

        ProcessBottomSheetModel.IconText = "calendar-plus"; // Metni ayrı ayarlayabilirsin
        ProcessBottomSheetModel.IconColor = "#E6BE0C";

        CurrentPage.FindByName<BottomSheet>("infoBottomSheet").State = BottomSheetState.HalfExpanded;
    }
}