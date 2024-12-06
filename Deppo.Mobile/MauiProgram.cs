using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Controls.UserDialogs.Maui;
using Deppo.Core.DataStores;
using Deppo.Core.Services;
using Deppo.Mobile.Core.DataStores;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.Views;
using Deppo.Mobile.Modules.CameraModule.ViewModels;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.CountingModule.CountingPanel.ViewModels;
using Deppo.Mobile.Modules.CountingModule.CountingPanel.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ViewModels;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.ViewModels;
using Deppo.Mobile.Modules.CountingModule.NegativeProductMenu.Views;
using Deppo.Mobile.Modules.LoginModule.ViewModels;
using Deppo.Mobile.Modules.LoginModule.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.Views;
using Deppo.Mobile.Modules.OutsourceModule.WaitingOutsourceMenu.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.WaitingOutsourceMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels.ActionViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views.ActionViews;
using Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels.ActionViewModels;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views.ActionViews;
using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels.ActionViewModels;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views.ActionViews;
using Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrderReworkProcess.Views;
using Deppo.Mobile.Modules.ResultModule.ViewModels;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels.ActionViewModels;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views.ActionViews;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByLocationProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.ViewModels;
using Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.Views;
using Deppo.Mobile.Modules.TaskModule.ViewModels;
using Deppo.Mobile.Modules.TaskModule.Views;
using Deppo.Mobile.Modules.TransactionSchedulerModule.ViewModels;
using Deppo.Mobile.Modules.TransactionSchedulerModule.Views;
using Deppo.Sys.Service.DataStores;
using Deppo.Sys.Service.Services;
using DevExpress.Maui;
using DotNet.Meteor.HotReload.Plugin;
using Java.Lang;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using SkiaSharp.Views.Maui.Controls.Hosting;
using ZXing.Net.Maui.Controls;
using Syncfusion.Maui.Core.Hosting;
using Deppo.Mobile.Modules.SalesModule.WaitingProductMenu.Views;
using Deppo.Mobile.Modules.SalesModule.WaitingProductMenu.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.WaitingProductMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.WaitingProductMenu.ViewModels;
using IProductService = Deppo.Core.Services.IProductService;
using ICustomerService = Deppo.Core.Services.ICustomerService;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

namespace Deppo.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
#if DEBUG
            .EnableHotReload()
#endif
            .UseMauiCommunityToolkit(options =>
            {
                options.SetShouldSuppressExceptionsInConverters(true);
                options.SetShouldSuppressExceptionsInBehaviors(true);
                options.SetShouldSuppressExceptionsInAnimations(true);
            })
            .UseSkiaSharp()
            .UseDevExpressCollectionView()
            .UseDevExpressCharts()
            .UseDevExpressControls()
            .UseDevExpressEditors()
            .UseDevExpress()
            .ConfigureSyncfusionCore()
            .UseBarcodeReader()
            .UseMauiCommunityToolkitMarkup()
            .UseMauiCommunityToolkitCamera()
            .UseMauiCommunityToolkitMediaElement()
            .UseUserDialogs(() =>
            {
                #region AlertConfig

                AlertConfig.DefaultBackgroundColor = Colors.White;
                AlertConfig.DefaultOkText = "Tamam";
                AlertConfig.DefaultTitleFontSize = 16;
                AlertConfig.DefaultMessageFontSize = 14;
                AlertConfig.DefaultMessageColor = Colors.DarkGray;
                AlertConfig.DefaultPositiveButtonFontSize = 14;
#if ANDROID
                AlertConfig.DefaultMessageFontFamily = "Roboto-Regular.ttf";
#else
				AlertConfig.DefaultMessageFontFamily = "Roboto-Regular";
#endif

                #endregion AlertConfig

                #region ToastConfig

                ToastConfig.DefaultCornerRadius = 15;

                #endregion ToastConfig

                #region Confirm Config

                ConfirmConfig.DefaultBackgroundColor = Colors.White;
                ConfirmConfig.DefaultTitleFontSize = 16;
                ConfirmConfig.DefaultTitleColor = Colors.Black;
                ConfirmConfig.DefaultMessageFontSize = 14;
                ConfirmConfig.DefaultPositiveButtonFontSize = 14;
                ConfirmConfig.DefaultNegativeButtonFontSize = 14;
                ConfirmConfig.DefaultCancelText = "İptal";
                ConfirmConfig.DefaultOkText = "Evet";

                #endregion Confirm Config
            })

            .ConfigureFonts(fonts =>
            {
                #region Roboto font

                fonts.AddFont("Roboto-Black.ttf", "RobotoBlack");
                fonts.AddFont("Roboto-BlackItalic.ttf", "RobotoBlackItalic");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
                fonts.AddFont("Roboto-BoldItalic.ttf", "RobotoBoldItalic");
                fonts.AddFont("Roboto-Italic.ttf", "RobotoItalic");
                fonts.AddFont("Roboto-Light.ttf", "RobotoLight");
                fonts.AddFont("Roboto-LightItalic.ttf", "RobotoLightItalic");
                fonts.AddFont("Roboto-Medium.ttf", "RobotoMedium");
                fonts.AddFont("Roboto-MediumItalic.ttf", "RobotoMediumItalic");
                fonts.AddFont("Roboto-Regular.ttf", "Roboto");
                fonts.AddFont("Roboto-Thin.ttf", "RobotoThin");
                fonts.AddFont("Roboto-ThinItalic.ttf", "RobotoThinItalic");

                #endregion Roboto font

                #region FontAwesome

                fonts.AddFont("fa-solid.otf", "FAS");
                fonts.AddFont("fa-regular.otf", "FAR");
                fonts.AddFont("fa-brands.otf", "FAB");

                #endregion FontAwesome
            });

        #region Remove Underline from SearchBar

        Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping(nameof(SearchBar), (handler, view) =>
        {
#if ANDROID
            Android.Widget.LinearLayout? linearLayout = handler.PlatformView.GetChildAt(0) as Android.Widget.LinearLayout;
            linearLayout = linearLayout?.GetChildAt(2) as Android.Widget.LinearLayout;
            linearLayout = linearLayout?.GetChildAt(1) as Android.Widget.LinearLayout;
            linearLayout.Background = null;
#endif
        });

        #endregion Remove Underline from SearchBar

#if ANDROID
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
        {
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton(UserDialogs.Instance);
        builder.Services.AddSingleton<IBarcodeSearchHelper, BarcodeSearchHelper>();
        builder.Services.AddSingleton<IBarcodeSearchOutHelper, BarcodeSearchOutHelper>();
        builder.Services.AddSingleton<IBarcodeSearchDemandHelper, BarcodeSearchDemandHelper>();
        builder.Services.AddSingleton<IBarcodeSearchSalesHelper, BarcodeSearchSalesHelper>();
        builder.Services.AddSingleton<IBarcodeSearchPurchaseHelper, BarcodeSearchPurchaseHelper>();
        builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
        builder.Services.AddSingleton<IHttpClientSysService, HttpClientSysService>();
        builder.Services.AddSingleton<IAuthenticationService, AuthenticateDataStore>();
        builder.Services.AddSingleton<ICustomQueryService, CustomQueryDataStore>();
        builder.Services.AddSingleton<IProductService, ProductDataStoreV2>();
        builder.Services.AddSingleton<ISubUnitsetService, SubUnitsetDataStore>();
        builder.Services.AddSingleton<Deppo.Core.Services.IWarehouseService, Deppo.Core.DataStores.WarehouseDataStore>();
        builder.Services.AddSingleton<ICustomerService, CustomerDataStoreV2>();
        builder.Services.AddSingleton<IWaitingSalesOrderService, WaitingSalesOrderDataStore>();
        builder.Services.AddSingleton<ISupplierService, SupplierDataStoreV2>();
        builder.Services.AddSingleton<ISupplierDetailService, SupplierDetailDataStore>();
        builder.Services.AddSingleton<ISupplierDetailAllFicheService, SupplierDetailAllFicheDataStore>();
        builder.Services.AddSingleton<ISupplierDetailActionService, SupplierDetailActionDataStore>();
        builder.Services.AddSingleton<ISupplierDetailInputProductService, SupplierDetailInputProductDataStore>();
        builder.Services.AddSingleton<ISupplierDetailOutputProductService, SupplierDetailOutputProductDataStore>();
        builder.Services.AddSingleton<IProductTransactionService, ProductTransactionDataStore>();
        builder.Services.AddSingleton<IWarehouseTransactionService, WarehouseTransactionDataStore>();
        builder.Services.AddSingleton<ICustomerTransactionService, CustomerTransactionDataStore>();
        builder.Services.AddSingleton<ISupplierTransactionService, SupplierTransactionDataStore>();
        builder.Services.AddSingleton<ISupplierTransactionLineService, SupplierTransactionLineDataStore>();
        builder.Services.AddSingleton<ICustomerTransactionLineService, CustomerTransactionLineDataStore>();
        builder.Services.AddSingleton<IProductTransactionLineService, ProductTransactionLineDataStore>();
        builder.Services.AddSingleton<IVariantService, VariantDataStore>();
        builder.Services.AddSingleton<IWarehouseTotalService, WarehouseTotalDataStore>();
        builder.Services.AddSingleton<ISeriLotTransactionService, SeriLotTransactionDataStore>();
        builder.Services.AddSingleton<ILocationTransactionService, LocationTransactionDataStore>();
        builder.Services.AddSingleton<ISeriLotService, SeriLotDataStore>();
        builder.Services.AddSingleton<ILocationService, LocationDataStore>();
        builder.Services.AddSingleton<IWaitingPurchaseOrderService, WaitingPurchaseOrderDataStore>();
        builder.Services.AddSingleton<IPurchaseSupplierService, PurchaseSupplierDataStore>();
        builder.Services.AddSingleton<IPurchaseSupplierProductService, PurchaseSupplierProductDataStore>();
        builder.Services.AddSingleton<ISalesCustomerService, SalesCustomerDataStore>();
        builder.Services.AddSingleton<ISalesCustomerProductService, SalesCustomerProductDataStore>();
        builder.Services.AddSingleton<IShipAddressService, ShipAddressDataStore>();
        builder.Services.AddSingleton<ICarrierService, CarrierDataStore>();
        builder.Services.AddSingleton<IDriverService, DriverDataStore>();
        builder.Services.AddSingleton<IDemandProcessProductService, DemandProcessProductDataStore>();
        builder.Services.AddSingleton<IDemandProcessVariantService, DemandProcessVariantDataStore>();
        builder.Services.AddSingleton<IDemandService, DemandDataStore>();
        builder.Services.AddSingleton<IWarehouseParameterService, WarehouseParameterDataStore>();
        builder.Services.AddSingleton<IWaitingSalesProductService, WaitingSalesProductDataStore>();
		builder.Services.AddSingleton<IWaitingPurchaseProductService, WaitingPurchaseProductDataStore>();

		builder.Services.AddSingleton<IProductPanelService, ProductPanelDataStore>();
        builder.Services.AddSingleton<ISalesPanelService, SalesPanelDataStore>();
        builder.Services.AddSingleton<IPurchasePanelService, PurchasePanelDataStore>();
        builder.Services.AddSingleton<IPurchaseAnalysisService, PurchaseAnalysisDataStore>();
        builder.Services.AddSingleton<ISalesAnalysisService, SalesAnalysisDataStore>();
        builder.Services.AddSingleton<IPurchasePanelService, PurchasePanelDataStore>();
        builder.Services.AddSingleton<ISalesPanelService, SalesPanelDataStore>();
        builder.Services.AddSingleton<IOverviewAnalysisService, OverviewAnalysisDataStore>();
        builder.Services.AddSingleton<IProductAnalysisService, ProductAnalysisDataStore>();

        builder.Services.AddSingleton<IProductionTransactionService, ProductionTransactionDataStore>();
        builder.Services.AddSingleton<IConsumableTransactionService, ConsumableTransactionDataStore>();
        builder.Services.AddSingleton<IInCountingTransactionService, InCountingTransactionDataStore>();
        builder.Services.AddSingleton<IOutCountingTransactionService, OutCountingTransactionDataStore>();
        builder.Services.AddSingleton<IWastageTransactionService, WastageTransactionDataStore>();
        builder.Services.AddSingleton<IPurchaseDispatchTransactionService, PurchaseDispatchTransactionDataStore>();
        builder.Services.AddSingleton<IRetailSalesDispatchTransactionService, RetailSalesDispatchTransactionDataStore>();
        builder.Services.AddSingleton<IWholeSalesDispatchTransactionService, WholeSalesDispatchTransactionDataStore>();

        builder.Services.AddSingleton<IPurchaseReturnDispatchTransactionService, PurchaseReturnDispatchTransactionDataStore>();
        builder.Services.AddSingleton<IRetailSalesReturnDispatchTransactionService, RetailSalesReturnDispatchTransactionDataStore>();
        builder.Services.AddSingleton<IWholeSalesReturnDispatchTransactionService, WholeSalesReturnDispatchTransactionDataStore>();
        builder.Services.AddSingleton<ITransferTransactionService, TransferTransactionDataStore>();

        builder.Services.AddSingleton<ISalesDispatchTransactionService, SalesDispatchTransactionDataStore>();
        builder.Services.AddSingleton<IOutsourceService, OutsourceDataStore>();
        builder.Services.AddSingleton<IInputOutsourceTransferV2ProductService, InputOutsourceTransferV2ProductDataStore>();
        builder.Services.AddSingleton<IInputOutsourceTransferV2SubProductService, InputOutsourceTransferV2SubProductDataStore>();
        builder.Services.AddSingleton<IWarehouseCountingService, WarehouseCountingDataStore>();
        builder.Services.AddSingleton<IProductCountingService, ProductCountingDataStore>();

        builder.Services.AddSingleton<IQuicklyBomService, QuicklyBomDataStore>();
        builder.Services.AddSingleton<IOutsourcePanelService, OutsourcePanelDataStore>();
        builder.Services.AddSingleton<ICountingPanelService, CountingPanelDataStore>();
        builder.Services.AddSingleton<IQuicklyProductionPanelService, QuicklyProductionPanelDataStore>();
        builder.Services.AddSingleton<IProductDetailActionService, ProductDetailActionDataStore>();
        builder.Services.AddSingleton<IVariantWarehouseTotalService, VariantWarehouseTotalDataStore>();

        builder.Services.AddSingleton<IBarcodeSearchService, BarcodeSearchDataStore>();
        builder.Services.AddSingleton<IBarcodeSearchOutService, BarcodeSearchOutDataStore>();
        builder.Services.AddSingleton<IBarcodeSearchDemandService, BarcodeSearchDemandDataStore>();
        builder.Services.AddSingleton<IBarcodeSearchSalesService, BarcodeSearchSalesDataStore>();
        builder.Services.AddSingleton<IBarcodeSearchPurchaseService, BarcodeSearchPurchaseDataStore>();

        builder.Services.AddSingleton<ICustomerDetailService, CustomerDetailDataStore>();
        builder.Services.AddSingleton<ICustomerDetailActionService, CustomerDetailActionDataStore>();
        builder.Services.AddSingleton<ICustomerDetailAllFichesService, CustomerDetailAllFichesDataStore>();
        builder.Services.AddSingleton<ICustomerDetailInputProductService, CustomerDetailInputProductDataStore>();
        builder.Services.AddSingleton<ICustomerDetailOutputProductService, CustomerDetailOutputProductDataStore>();

        builder.Services.AddSingleton<IProductDetailAllFichesService, ProductDetailAllFichesDataStore>();
        builder.Services.AddSingleton<IProductDetailService, ProductDetailDataStore>();

        builder.Services.AddSingletonWithShellRoute<LoginView, LoginViewModel>(nameof(LoginView));
        builder.Services.AddTransientWithShellRoute<LoginParameterView, LoginParameterViewModel>(nameof(LoginParameterView));
        builder.Services.AddTransientWithShellRoute<CompanyListView, CompanyListViewModel>(nameof(CompanyListView));

        builder.Services.AddSingleton<IWarehouseDetailService, WarehouseDetailDataStore>();
        builder.Services.AddSingleton<IWarehouseDetailAllFicheListService, WarehouseDetailAllFicheListDataStore>();

        builder.Services.AddSingleton<IWarehouseInputTransactionService, WarehouseInputTransactionDataStore>();
        builder.Services.AddSingleton<IWarehouseOutputTransactionService, WarehouseOutputTransactionDataStore>();
        builder.Services.AddSingleton<IWarehouseDetailActionService, WarehouseDetailActionDataStore>();
        builder.Services.AddSingleton<IProcurementByCustomerService, ProcurementByCustomerDataStore>();
        builder.Services.AddSingleton<IProcurementByCustomerProductService, ProcurementByCustomerProductDataStore>();
        builder.Services.AddSingleton<IProcurementByCustomerBasketService, ProcurementByCustomerBasketDataStore>();
        builder.Services.AddSingleton<IProcurementSalesCustomerService, ProcurementSalesCustomerDataStore>();
        builder.Services.AddSingleton<IPackageProductService, PackageProductDataStore>();
        builder.Services.AddSingleton<IProductPictureService, ProductPictureDataStore>();
        builder.Services.AddSingleton<IProcurementSalesProductService, ProcurementSalesProductDataStore>();

        builder.Services.AddSingleton<IOutsourceDetailInputProductService, OutsourceDetailInputProductDataStore>();
        builder.Services.AddSingleton<IOutsourceDetailOutputProductService, OutsourceDetailOutputProductDataStore>();
        builder.Services.AddSingleton<IOutsourceDetailAllFichesService, OutsourceDetailAllFichesDataStore>();

        builder.Services.AddSingleton<IAuthenticateSysService, AuthenticateSysDataStore>();
        builder.Services.AddSingleton<ITransactionAuditService, TransactionAuditDataStore>();
        builder.Services.AddSingleton<IWarehouseProcessParameterService, WarehouseProcessParameterDataStore>();
        
        builder.Services.AddSingleton<IApplicationUserService, ApplicationUserDataStore>();
        builder.Services.AddSingleton<IProcurementAuditService, ProcurementAuditDataStore>();
        builder.Services.AddSingleton<IProcurementAuditCustomerService, ProcurementAuditCustomerDataStore>();
        builder.Services.AddSingleton<ITransactionAuditHelperService, TransactionAuditHelperDataStore>();
        builder.Services.AddSingleton<IProcurementByProductService, ProcurementByProductDataStore>();
        builder.Services.AddSingleton<IProcurementByProductCustomerService, ProcurementByProductCustomerDataStore>();
        builder.Services.AddSingleton<IProcurementByProductProcurableProductService, ProcurementByProductProcurableProductDataStore>();
        builder.Services.AddSingleton<IProcurementByProductBasketService, ProcurementByProductBasketDataStore>();
        builder.Services.AddSingleton<IProcurementAuditService, ProcurementAuditDataStore>();
        builder.Services.AddSingleton<IReasonsForRejectionService, ReasonsForRejectionDataStore>();
        builder.Services.AddSingleton<IReasonsForRejectionProcurementService, ReasonsForRejectionProcurementDataStore>();
        builder.Services.AddSingleton<INotificationService, NotificationDataStore>();
        builder.Services.AddSingleton<INotificationStatusService, NotificationStatusDataStore>();
        builder.Services.AddSingleton<IProcessRateService, ProcessRateDataStore>();
        builder.Services.AddSingleton<ITaskListService, TaskListDataStore>();
        builder.Services.AddSingleton<IOverviewAnalysisTransactionService, OverviewAnalysisTransactionDataStore>();
        builder.Services.AddSingleton<IProcurementByLocationProductService, ProcurementByLocationProductDataStore>();
        builder.Services.AddSingleton<IProcurementByLocationCustomerService, ProcurementByLocationCustomerDataStore>();
        builder.Services.AddSingleton<IProcurementByLocationService, ProcurementByLocationDataStore>();

        builder.Services.AddSingleton<ITransactionSchedulerService, TransactionSchedulerDataStore>();
        builder.Services.AddSingleton<IConnectionParameterService, ConnectionParameterDataStore>();
		builder.Services.AddSingleton<IProcurementLocationTransactionService, ProcurementLocationTransactionDataStore>();
        builder.Services.AddSingleton<IProcurementFicheService, ProcurementFicheDataStore>();
		builder.Services.AddSingleton<Deppo.Sys.Service.Services.IWarehouseService, Deppo.Sys.Service.DataStores.WarehouseDataStore>();
		builder.Services.AddSingleton<Deppo.Sys.Service.Services.IProductService, Deppo.Sys.Service.DataStores.ProductDataStore>();
        builder.Services.AddSingleton<Deppo.Sys.Service.Services.ISubunitsetService, Deppo.Sys.Service.DataStores.SubunitsetDataStore>();
        builder.Services.AddSingleton<Deppo.Sys.Service.Services.ICustomerService, Deppo.Sys.Service.DataStores.CustomerDataStore>();

        builder.Services.AddSingleton<IOutputOutsourceTransferV2Service, OutputOutsourceTransferV2DataStore>();


		#region Analysis Modules

		builder.Services.AddSingletonWithShellRoute<OverviewAnalysisView, OverviewAnalysisViewModel>(nameof(OverviewAnalysisView));
        builder.Services.AddSingletonWithShellRoute<ProductAnalysisView, ProductAnalysisViewModel>(nameof(ProductAnalysisView));
        builder.Services.AddSingletonWithShellRoute<PurchaseAnalysisView, PurchaseAnalysisViewModel>(nameof(PurchaseAnalysisView));
        builder.Services.AddSingletonWithShellRoute<SalesAnalysisView, SalesAnalysisViewModel>(nameof(SalesAnalysisView));

        builder.Services.AddTransientWithShellRoute<ProfileView, ProfileViewModel>(nameof(ProfileView));
        builder.Services.AddTransientWithShellRoute<NotificationListView, NotificationListViewModel>(nameof(NotificationListView));
        builder.Services.AddTransientWithShellRoute<OverviewInputTransactionListView, OverviewInputTransactionListViewModel>(nameof(OverviewInputTransactionListView));
        builder.Services.AddTransientWithShellRoute<OverviewOutputTransactionListView, OverviewOutputTransactionListViewModel>(nameof(OverviewOutputTransactionListView));

        #endregion Analysis Modules

        #region Product Modules

        builder.Services.AddSingletonWithShellRoute<ProductPanelView, ProductPanelViewModel>(nameof(ProductPanelView));
        builder.Services.AddTransientWithShellRoute<InputProductListView, InputProductListViewModel>(nameof(InputProductListView));
        builder.Services.AddTransientWithShellRoute<OutputProductListView, OutputProductListViewModel>(nameof(OutputProductListView));
        builder.Services.AddTransientWithShellRoute<AllFicheListView, AllFicheListViewModel>(nameof(AllFicheListView));

        builder.Services.AddSingletonWithShellRoute<ProductListView, ProductListViewModel>(nameof(ProductListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailView, ProductDetailViewModel>(nameof(ProductDetailView));
        builder.Services.AddScopedWithShellRoute<ProductDetailLocationTransactionListView, ProductDetailLocationTransactionListViewModel>(nameof(ProductDetailLocationTransactionListView));
        builder.Services.AddTransientWithShellRoute<ProductDetailAllFicheListView, ProductDetailAllFicheListViewModel>(nameof(ProductDetailAllFicheListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailVariantListView, ProductDetailVariantListViewModel>(nameof(ProductDetailVariantListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailVariantTotalListView, ProductDetailVariantTotalListViewModel>(nameof(ProductDetailVariantTotalListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailWaitingPurchaseOrderListView, ProductDetailWaitingPurchaseOrderListViewModel>(nameof(ProductDetailWaitingPurchaseOrderListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailWaitingSalesOrderListView, ProductDetailWaitingSalesOrderListViewModel>(nameof(ProductDetailWaitingSalesOrderListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailWarehouseTotalListView, ProductDetailWarehouseTotalListViewModel>(nameof(ProductDetailWarehouseTotalListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailApprovedSupplierView, ProductDetailApprovedSupplierViewModel>(nameof(ProductDetailApprovedSupplierView));
        builder.Services.AddScopedWithShellRoute<ProductDetailVariantDetailListView, ProductDetailVariantDetailListViewModel>(nameof(ProductDetailVariantDetailListView));
        builder.Services.AddTransientWithShellRoute<ProductInputTransactionView, ProductInputTransactionViewModel>(nameof(ProductInputTransactionView));
        builder.Services.AddTransientWithShellRoute<ProductOutputTransactionView, ProductOutputTransactionViewModel>(nameof(ProductOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<WarehouseListView, WarehouseListViewModel>(nameof(WarehouseListView));
        builder.Services.AddScopedWithShellRoute<WarehouseDetailView, WarehouseDetailViewModel>(nameof(WarehouseDetailView));
        builder.Services.AddTransientWithShellRoute<WarehouseInputTransactionView, WarehouseInputTransactionViewModel>(nameof(WarehouseInputTransactionView));
        builder.Services.AddTransientWithShellRoute<WarehouseOutputTransactionView, WarehouseOutputTransactionViewModel>(nameof(WarehouseOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<ProductProcessView, ProductProcessViewModel>(nameof(ProductProcessView));
        builder.Services.AddTransientWithShellRoute<WarehouseDetailAllFicheListView, WarehouseDetailAllFicheListViewModel>(nameof(WarehouseDetailAllFicheListView));
        builder.Services.AddScopedWithShellRoute<WarehouseDetailWarehouseTotalListView, WarehouseDetailWarehouseTotalListViewModel>(nameof(WarehouseDetailWarehouseTotalListView));
        builder.Services.AddScopedWithShellRoute<WarehouseDetailLocationListView, WarehouseDetailLocationListViewModel>(nameof(WarehouseDetailLocationListView));
        builder.Services.AddScopedWithShellRoute<ProductCaptureImageView, ProductCaptureImageViewModel>(nameof(ProductCaptureImageView));
        builder.Services.AddScopedWithShellRoute<ProductPreviewImageView, ProductPreviewImageViewModel>(nameof(ProductPreviewImageView));
		builder.Services.AddScopedWithShellRoute<ProductDetailAlternativeProductListView, ProductDetailAlternativeProductListViewModel>(nameof(ProductDetailAlternativeProductListView));

		#region ProductionInput Modules

		builder.Services.AddScopedWithShellRoute<InputProductProcessWarehouseListView, InputProductProcessWarehouseListViewModel>(nameof(InputProductProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessBasketListView, InputProductProcessBasketListViewModel>(nameof(InputProductProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessBasketLocationListView, InputProductProcessBasketLocationListViewModel>(nameof(InputProductProcessBasketLocationListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessBasketSeriLotListView, InputProductProcessBasketSeriLotListViewModel>(nameof(InputProductProcessBasketSeriLotListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessProductListView, InputProductProcessProductListViewModel>(nameof(InputProductProcessProductListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessFormView, InputProductProcessFormViewModel>(nameof(InputProductProcessFormView));

        #endregion ProductionInput Modules

        #region OutputProductProcess Modules

        builder.Services.AddScopedWithShellRoute<OutputProductProcessWarehouseListView, OutputProductProcessWarehouseListViewModel>(nameof(OutputProductProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputProductProcessBasketListView, OutputProductProcessBasketListViewModel>(nameof(OutputProductProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<OutputProductProcessProductListView, OutputProductProcessProductListViewModel>(nameof(OutputProductProcessProductListView));
        builder.Services.AddScopedWithShellRoute<OutputProductProcessFormView, OutputProductProcessFormViewModel>(nameof(OutputProductProcessFormView));

        #endregion OutputProductProcess Modules

        #region DemandProcess Modules

        builder.Services.AddScopedWithShellRoute<DemandProcessWarehouseListView, DemandProcessWarehouseListViewModel>(nameof(DemandProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<DemandProcessBasketListView, DemandProcessBasketListViewModel>(nameof(DemandProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<DemandProcessProductListView, DemandProcessProductListViewModel>(nameof(DemandProcessProductListView));
        builder.Services.AddScopedWithShellRoute<DemandProcessVariantListView, DemandProcessVariantListViewModel>(nameof(DemandProcessVariantListView));
        builder.Services.AddScopedWithShellRoute<DemandProcessFormView, DemandProcessFormViewModel>(nameof(DemandProcessFormView));

        #endregion DemandProcess Modules

        #region TransferProductProcess Modules

        builder.Services.AddScopedWithShellRoute<TransferOutWarehouseListView, TransferOutWarehouseListViewModel>(nameof(TransferOutWarehouseListView));
        builder.Services.AddScopedWithShellRoute<TransferOutProductListView, TransferOutProductListViewModel>(nameof(TransferOutProductListView));
        builder.Services.AddScopedWithShellRoute<TransferOutBasketView, TransferOutBasketViewModel>(nameof(TransferOutBasketView));
        builder.Services.AddScopedWithShellRoute<TransferInWarehouseView, TransferInWarehouseViewModel>(nameof(TransferInWarehouseView));
        builder.Services.AddScopedWithShellRoute<TransferInBasketView, TransferInBasketViewModel>(nameof(TransferInBasketView));
        builder.Services.AddScopedWithShellRoute<TransferFormView, TransferFormViewModel>(nameof(TransferFormView));

        #endregion TransferProductProcess Modules

        #region VirmanProduct Modules

        builder.Services.AddTransientWithShellRoute<VirmanProductOutWarehouseListView, VirmanProductOutWarehouseListViewModel>(nameof(VirmanProductOutWarehouseListView));
        builder.Services.AddScopedWithShellRoute<VirmanProductOutListView, VirmanProductOutListViewModel>(nameof(VirmanProductOutListView));
        builder.Services.AddScopedWithShellRoute<VirmanProductInWarehouseListView, VirmanProductInWarehouseListViewModel>(nameof(VirmanProductInWarehouseListView));
        builder.Services.AddScopedWithShellRoute<VirmanProductInListView, VirmanProductInListViewModel>(nameof(VirmanProductInListView));
        builder.Services.AddScopedWithShellRoute<VirmanProductBasketListView, VirmanProductBasketListViewModel>(nameof(VirmanProductBasketListView));
        builder.Services.AddScopedWithShellRoute<VirmanProductFormListView, VirmanProductFormListViewModel>(nameof(VirmanProductFormListView));

        #endregion VirmanProduct Modules

        #endregion Product Modules

        #region Sales Modules

        builder.Services.AddSingletonWithShellRoute<SalesPanelView, SalesPanelViewModel>(nameof(SalesPanelView));
        builder.Services.AddSingletonWithShellRoute<SalesPanelWaitingProductListView, SalesPanelWaitingProductListViewModel>(nameof(SalesPanelWaitingProductListView));
        builder.Services.AddSingletonWithShellRoute<SalesPanelShippedProductListView, SalesPanelShippedProductListViewModel>(nameof(SalesPanelShippedProductListView));
        builder.Services.AddSingletonWithShellRoute<SalesPanelAllFicheListView, SalesPanelAllFicheListViewModel>(nameof(SalesPanelAllFicheListView));

        builder.Services.AddSingletonWithShellRoute<CustomerListView, CustomerListViewModel>(nameof(CustomerListView));
        builder.Services.AddScopedWithShellRoute<CustomerDetailView, CustomerDetailViewModel>(nameof(CustomerDetailView));
        builder.Services.AddTransientWithShellRoute<CustomerInputTransactionView, CustomerInputTransactionViewModel>(nameof(CustomerInputTransactionView));
        builder.Services.AddTransientWithShellRoute<CustomerOutputTransactionView, CustomerOutputTransactionViewModel>(nameof(CustomerOutputTransactionView));
        builder.Services.AddTransientWithShellRoute<CustomerDetailWaitingSalesOrderListView, CustomerDetailWaitingSalesOrderListViewModel>(nameof(CustomerDetailWaitingSalesOrderListView));
        builder.Services.AddTransientWithShellRoute<CustomerDetailShipAddressListView, CustomerDetailShipAddressListViewModel>(nameof(CustomerDetailShipAddressListView));
        builder.Services.AddTransientWithShellRoute<CustomerDetailApprovedProductListView, CustomerDetailApprovedProductListViewModel>(nameof(CustomerDetailApprovedProductListView));

        builder.Services.AddSingletonWithShellRoute<WaitingSalesOrderListView, WaitingSalesOrderListViewModel>(nameof(WaitingSalesOrderListView));
        builder.Services.AddSingletonWithShellRoute<SalesProcessView, SalesProcessViewModel>(nameof(SalesProcessView));
        builder.Services.AddTransientWithShellRoute<CustomerDetailAllFichesView, CustomerDetailAllFichesViewModel>(nameof(CustomerDetailAllFichesView));

        builder.Services.AddSingletonWithShellRoute<WaitingSalesProductListView, WaitingSalesProductListViewModel>(nameof(WaitingSalesProductListView));

        #region Sevk Islemleri

        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessWarehouseListView, OutputProductSalesProcessWarehouseListViewModel>(nameof(OutputProductSalesProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessCustomerListView, OutputProductSalesProcessCustomerListViewModel>(nameof(OutputProductSalesProcessCustomerListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessBasketListView, OutputProductSalesProcessBasketListViewModel>(nameof(OutputProductSalesProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessProductListView, OutputProductSalesProcessProductListViewModel>(nameof(OutputProductSalesProcessProductListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessFormView, OutputProductSalesProcessFormViewModel>(nameof(OutputProductSalesProcessFormView));

        #endregion Sevk Islemleri

        #region Siparise Bagli Sevk Islemleri

        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessWarehouseListView, OutputProductSalesOrderProcessWarehouseListViewModel>(nameof(OutputProductSalesOrderProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessCustomerListView, OutputProductSalesOrderProcessCustomerListViewModel>(nameof(OutputProductSalesOrderProcessCustomerListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessProductListView, OutputProductSalesOrderProcessProductListViewModel>(nameof(OutputProductSalesOrderProcessProductListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessOrderListView, OutputProductSalesOrderProcessOrderListViewModel>(nameof(OutputProductSalesOrderProcessOrderListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessBasketListView, OutputProductSalesOrderProcessBasketListViewModel>(nameof(OutputProductSalesOrderProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessFormView, OutputProductSalesOrderProcessFormViewModel>(nameof(OutputProductSalesOrderProcessFormView));

        #endregion Siparise Bagli Sevk Islemleri

        #region Toplanan Urunlerin Sevk Islemleri

        builder.Services.AddScopedWithShellRoute<ProcurementSalesProcessWarehouseListView, ProcurementSalesProcessWarehouseListViewModel>(nameof(ProcurementSalesProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ProcurementSalesProcessCustomerListView, ProcurementSalesProcessCustomerListViewModel>(nameof(ProcurementSalesProcessCustomerListView));
        builder.Services.AddScopedWithShellRoute<ProcurementSalesProcessProductListView, ProcurementSalesProcessProductListViewModel>(nameof(ProcurementSalesProcessProductListView));
        builder.Services.AddScopedWithShellRoute<ProcurementSalesPackageBasketView, ProcurementSalesPackageBasketViewModel>(nameof(ProcurementSalesPackageBasketView));
        builder.Services.AddScopedWithShellRoute<ProcurementSalesPackageProductListView, ProcurementSalesPackageProductListViewModel>(nameof(ProcurementSalesPackageProductListView));
        builder.Services.AddScopedWithShellRoute<ProcurementSalesProcessProductBasketView, ProcurementSalesProcessProductBasketViewModel>(nameof(ProcurementSalesProcessProductBasketView));
        builder.Services.AddScopedWithShellRoute<ProcurementSalesProcessBasketProductListView, ProcurementSalesProcessBasketProductListViewModel>(nameof(ProcurementSalesProcessBasketProductListView));
        builder.Services.AddScopedWithShellRoute<ProcurementSalesProcessFormView, ProcurementSalesProcessFormViewModel>(nameof(ProcurementSalesProcessFormView));

        #endregion Toplanan Urunlerin Sevk Islemleri

        #region Satış İade İşlemleri

        builder.Services.AddTransientWithShellRoute<ReturnSalesWarehouseListView, ReturnSalesWarehouseListViewModel>(nameof(ReturnSalesWarehouseListView));

        builder.Services.AddScopedWithShellRoute<ReturnSalesBasketView, ReturnSalesBasketViewModel>(nameof(ReturnSalesBasketView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesProductListView, ReturnSalesProductListViewModel>(nameof(ReturnSalesProductListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesBasketLocationListView, ReturnSalesBasketLocationListViewModel>(nameof(ReturnSalesBasketLocationListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesBasketSeriLotListView, ReturnSalesBasketSeriLotListViewModel>(nameof(ReturnSalesBasketSeriLotListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesFormView, ReturnSalesFormViewModel>(nameof(ReturnSalesFormView));

        #endregion Satış İade İşlemleri

        #region İrsaliyeye Bağlı Satış İade İşlemleri

        builder.Services.AddTransientWithShellRoute<ReturnSalesDispatchWarehouseListView, ReturnSalesDispatchWarehouseListViewModel>(nameof(ReturnSalesDispatchWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesDispatchCustomerListView, ReturnSalesDispatchCustomerListViewModel>(nameof(ReturnSalesDispatchCustomerListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesDispatchProductListView, ReturnSalesDispatchProductListViewModel>(nameof(ReturnSalesDispatchProductListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesDispatchListView, ReturnSalesDispatchListViewModel>(nameof(ReturnSalesDispatchListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesDispatchBasketView, ReturnSalesDispatchBasketViewModel>(nameof(ReturnSalesDispatchBasketView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesDispatchBasketLocationListView, ReturnSalesDispatchBasketLocationListViewModel>(nameof(ReturnSalesDispatchBasketLocationListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesDispatchBasketSeriLotListView, ReturnSalesDispatchBasketSeriLotListViewModel>(nameof(ReturnSalesDispatchBasketSeriLotListView));
        builder.Services.AddScopedWithShellRoute<ReturnSalesDispatchFormView, ReturnSalesDispatchFormViewModel>(nameof(ReturnSalesDispatchFormView));

        #endregion İrsaliyeye Bağlı Satış İade İşlemleri

        #region Müşteriye Göre Ürün Toplama

        builder.Services.AddScopedWithShellRoute<ProcurementByCustomerWarehouseListView, ProcurementByCustomerWarehouseListViewModel>(nameof(ProcurementByCustomerWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByCustomerListView, ProcurementByCustomerListViewModel>(nameof(ProcurementByCustomerListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByCustomerProcurementWarehouseListView, ProcurementByCustomerProcurementWarehouseListViewModel>(nameof(ProcurementByCustomerProcurementWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByCustomerProductListView, ProcurementByCustomerProductListViewModel>(nameof(ProcurementByCustomerProductListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByCustomerBasketView, ProcurementByCustomerBasketViewModel>(nameof(ProcurementByCustomerBasketView));
        builder.Services.AddScopedWithShellRoute<ProcurementByCustomerReasonsForRejectionListView, ProcurementByCustomerReasonsForRejectionListViewModel>(nameof(ProcurementByCustomerReasonsForRejectionListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByCustomerFormView, ProcurementByCustomerFormViewModel>(nameof(ProcurementByCustomerFormView));

        #endregion Müşteriye Göre Ürün Toplama

        #region Malzemeye Göre Ürün Toplama

        builder.Services.AddTransientWithShellRoute<ProcurementByProductWarehouseListView, ProcurementByProductWarehouseListViewModel>(nameof(ProcurementByProductWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductListView, ProcurementByProductListViewModel>(nameof(ProcurementByProductListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductCustomerListView, ProcurementByProductCustomerListViewModel>(nameof(ProcurementByProductCustomerListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductProcurementWarehouseListView, ProcurementByProductProcurementWarehouseListViewModel>(nameof(ProcurementByProductProcurementWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductProcurableProductListView, ProcurementByProductProcurableProductListViewModel>(nameof(ProcurementByProductProcurableProductListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductBasketView, ProcurementByProductBasketViewModel>(nameof(ProcurementByProductBasketView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductReasonsForRejectionListView, ProcurementByProductReasonsForRejectionListViewModel>(nameof(ProcurementByProductReasonsForRejectionListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductQuantityDistributionListView, ProcurementByProductQuantityDistributionListViewModel>(nameof(ProcurementByProductQuantityDistributionListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByProductFormView, ProcurementByProductFormViewModel>(nameof(ProcurementByProductFormView));

        #endregion Malzemeye Göre Ürün Toplama

        #region Stok Yerine (Rafa) Göre Ürün Toplama

        builder.Services.AddTransientWithShellRoute<ProcurementByLocationWarehouseListView, ProcurementByLocationWarehouseListViewModel>(nameof(ProcurementByLocationWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationWarehouseLocationListView, ProcurementByLocationWarehouseLocationListViewModel>(nameof(ProcurementByLocationWarehouseLocationListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationProductListView, ProcurementByLocationProductListViewModel>(nameof(ProcurementByLocationProductListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationOrderWarehouseListView, ProcurementByLocationOrderWarehouseListViewModel>(nameof(ProcurementByLocationOrderWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationCustomerListView, ProcurementByLocationCustomerListViewModel>(nameof(ProcurementByLocationCustomerListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationBasketView, ProcurementByLocationBasketViewModel>(nameof(ProcurementByLocationBasketView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationReasonsForRejectionListView, ProcurementByLocationReasonsForRejectionListViewModel>(nameof(ProcurementByLocationReasonsForRejectionListView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationCustomerFormView, ProcurementByLocationCustomerFormViewModel>(nameof(ProcurementByLocationCustomerFormView));
        builder.Services.AddScopedWithShellRoute<ProcurementByLocationFormView, ProcurementByLocationFormViewModel>(nameof(ProcurementByLocationFormView));

        #endregion Stok Yerine (Rafa) Göre Ürün Toplama

        #endregion Sales Modules

        #region Purchase Modules

        builder.Services.AddSingletonWithShellRoute<PurchasePanelView, PurchasePanelViewModel>(nameof(PurchasePanelView));
        builder.Services.AddSingletonWithShellRoute<PurchasePanelAllFicheListView, PurchasePanelAllFicheListViewModel>(nameof(PurchasePanelAllFicheListView));
        builder.Services.AddSingletonWithShellRoute<PurchasePanelWaitingProductListView, PurchasePanelWaitingProductListViewModel>(nameof(PurchasePanelWaitingProductListView));
        builder.Services.AddSingletonWithShellRoute<PurchasePanelReceivedProductListView, PurchasePanelReceivedProductListViewModel>(nameof(PurchasePanelReceivedProductListView));

        builder.Services.AddSingletonWithShellRoute<SupplierListView, SupplierListViewModel>(nameof(SupplierListView));

        builder.Services.AddSingletonWithShellRoute<SupplierDetailView, SupplierDetailViewModel>(nameof(SupplierDetailView));
        builder.Services.AddSingletonWithShellRoute<SupplierInputTransactionView, SupplierInputTransactionViewModel>(nameof(SupplierInputTransactionView));
        builder.Services.AddSingletonWithShellRoute<SupplierOutputTransactionView, SupplierOutputTransactionViewModel>(nameof(SupplierOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<SupplierDetailAllFicheListView, SupplierDetailAllFicheListViewModel>(nameof(SupplierDetailAllFicheListView));

        builder.Services.AddTransientWithShellRoute<SupplierDetailApprovedProductListView, SupplierDetailApprovedProductListViewModel>(nameof(SupplierDetailApprovedProductListView));
        builder.Services.AddTransientWithShellRoute<SupplierDetailShipAddressListView, SupplierDetailShipAddressListViewModel>(nameof(SupplierDetailShipAddressListView));
        builder.Services.AddTransientWithShellRoute<SupplierDetailWaitingPurchaseOrderListView, SupplierDetailWaitingPurchaseOrderListViewModel>(nameof(SupplierDetailWaitingPurchaseOrderListView));
        builder.Services.AddSingletonWithShellRoute<WaitingPurchaseOrderListView, WaitingPurchaseOrderListViewModel>(nameof(WaitingPurchaseOrderListView));
        builder.Services.AddSingletonWithShellRoute<PurchaseProcessView, PurchaseProcessViewModel>(nameof(PurchaseProcessView));

		builder.Services.AddSingletonWithShellRoute<WaitingPurchaseProductListView, WaitingPurchaseProductListViewModel>(nameof(WaitingPurchaseProductListView));

		#region Satınalma İşlemleri

		builder.Services.AddScopedWithShellRoute<InputProductPurchaseProcessWarehouseListView, InputProductPurchaseProcessWarehouseListViewModel>(nameof(InputProductPurchaseProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseProcessSupplierListView, InputProductPurchaseProcessSupplierListViewModel>(nameof(InputProductPurchaseProcessSupplierListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseProcessProductListView, InputProductPurchaseProcessProductListViewModel>(nameof(InputProductPurchaseProcessProductListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseProcessBasketListView, InputProductPurchaseProcessBasketListViewModel>(nameof(InputProductPurchaseProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseProcessBasketLocationListView,
InputProductPurchaseProcessBasketLocationListViewModel>(nameof(InputProductPurchaseProcessBasketLocationListView));

        builder.Services.AddScopedWithShellRoute<InputProductPurchaseProcessBasketSeriLotListView, InputProductPurchaseProcessBasketSeriLotListViewModel>(nameof(InputProductPurchaseProcessBasketSeriLotListView));

        builder.Services.AddScopedWithShellRoute<InputProductPurchaseProcessFormView, InputProductPurchaseProcessFormViewModel>(nameof(InputProductPurchaseProcessFormView));

        #endregion Satınalma İşlemleri

        #region Siparişe Bağlı Satınalma İşlemleri

        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessWarehouseListView, InputProductPurchaseOrderProcessWarehouseListViewModel>(nameof(InputProductPurchaseOrderProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessSupplierListView, InputProductPurchaseOrderProcessSupplierListViewModel>(nameof(InputProductPurchaseOrderProcessSupplierListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessProductListView, InputProductPurchaseOrderProcessProductListViewModel>(nameof(InputProductPurchaseOrderProcessProductListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessBasketListView, InputProductPurchaseOrderProcessBasketListViewModel>(nameof(InputProductPurchaseOrderProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessOrderListView, InputProductPurchaseOrderProcessOrderListViewModel>(nameof(InputProductPurchaseOrderProcessOrderListView));

        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessBasketLocationListView, InputProductPurchaseOrderProcessBasketLocationListViewModel>(nameof(InputProductPurchaseOrderProcessBasketLocationListView));

        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessBasketSeriLotListView, InputProductPurchaseOrderProcessBasketSeriLotListViewModel>(nameof(InputProductPurchaseOrderProcessBasketSeriLotListView));

        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessFormView, InputProductPurchaseOrderProcessFormViewModel>(nameof(InputProductPurchaseOrderProcessFormView));

        builder.Services.AddScopedWithShellRoute<InputProductPurchaseOrderProcessOtherProductListView, InputProductPurchaseOrderProcessOtherProductListViewModel>(nameof(InputProductPurchaseOrderProcessOtherProductListView));

        #endregion Siparişe Bağlı Satınalma İşlemleri

        #region Satınalma İade İşlemleri

        builder.Services.AddTransientWithShellRoute<ReturnPurchaseWarehouseListView, ReturnPurchaseWarehouseListViewModel>(nameof(ReturnPurchaseWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseProductListView, ReturnPurchaseProductListViewModel>(nameof(ReturnPurchaseProductListView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseBasketView, ReturnPurchaseBasketViewModel>(nameof(ReturnPurchaseBasketView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseFormView, ReturnPurchaseFormViewModel>(nameof(ReturnPurchaseFormView));

        #endregion Satınalma İade İşlemleri

        #region İrsaliyeye Bağlı Satınalma İade İşlemleri

        builder.Services.AddTransientWithShellRoute<ReturnPurchaseDispatchWarehouseListView, ReturnPurchaseDispatchWarehouseListViewModel>(nameof(ReturnPurchaseDispatchWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseDispatchSupplierListView, ReturnPurchaseDispatchSupplierListViewModel>(nameof(ReturnPurchaseDispatchSupplierListView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseDispatchListView, ReturnPurchaseDispatchListViewModel>(nameof(ReturnPurchaseDispatchListView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseDispatchProductListView, ReturnPurchaseDispatchProductListViewModel>(nameof(ReturnPurchaseDispatchProductListView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseDispatchBasketView, ReturnPurchaseDispatchBasketViewModel>(nameof(ReturnPurchaseDispatchBasketView));
        builder.Services.AddScopedWithShellRoute<ReturnPurchaseDispatchFormView, ReturnPurchaseDispatchFormViewModel>(nameof(ReturnPurchaseDispatchFormView));

        #endregion İrsaliyeye Bağlı Satınalma İade İşlemleri

        #endregion Purchase Modules

        #region Counting Modules

        builder.Services.AddSingletonWithShellRoute<CountingPanelView, CountingPanelViewModel>(nameof(CountingPanelView));
        builder.Services.AddTransientWithShellRoute<CountingInputReferenceProductListView, CountingInputReferenceProductListViewModel>(nameof(CountingInputReferenceProductListView));
        builder.Services.AddTransientWithShellRoute<CountingOutputReferenceProductListView, CountingOutputReferenceProductListViewModel>(nameof(CountingOutputReferenceProductListView));
        builder.Services.AddTransientWithShellRoute<CountingTransactionsListView, CountingTransactionsListViewModel>(nameof(CountingTransactionsListView));

        builder.Services.AddSingletonWithShellRoute<NegativeProductListView, NegativeProductListViewModel>(nameof(NegativeProductListView));
        builder.Services.AddScopedWithShellRoute<NegativeProductFormView, NegativeProductFormViewModel>(nameof(NegativeProductFormView));
        builder.Services.AddSingletonWithShellRoute<CountingProcessView, CountingProcessViewModel>(nameof(CountingProcessView));

        #region Ambara Gore Sayim

        builder.Services.AddScopedWithShellRoute<WarehouseCountingWarehouseListView, WarehouseCountingWarehouseListViewModel>(nameof(WarehouseCountingWarehouseListView));
        builder.Services.AddScopedWithShellRoute<WarehouseCountingLocationListView, WarehouseCountingLocationListViewModel>(nameof(WarehouseCountingLocationListView));
        builder.Services.AddScopedWithShellRoute<WarehouseCountingBasketView, WarehouseCountingBasketViewModel>(nameof(WarehouseCountingBasketView));
        builder.Services.AddScopedWithShellRoute<WarehouseCountingShowProductListView, WarehouseCountingShowProductListViewModel>(nameof(WarehouseCountingShowProductListView));
        builder.Services.AddScopedWithShellRoute<WarehouseCountingProductListView, WarehouseCountingProductListViewModel>(nameof(WarehouseCountingProductListView));
        builder.Services.AddScopedWithShellRoute<WarehouseCountingFormView, WarehouseCountingFormViewModel>(nameof(WarehouseCountingFormView));

        #endregion Ambara Gore Sayim

        #region Urune Gore Sayim

        builder.Services.AddScopedWithShellRoute<ProductCountingProductListView, ProductCountingProductListViewModel>(nameof(ProductCountingProductListView));
        builder.Services.AddScopedWithShellRoute<ProductCountingWarehouseTotalListView, ProductCountingWarehouseTotalListViewModel>(nameof(ProductCountingWarehouseTotalListView));
        builder.Services.AddScopedWithShellRoute<ProductCountingLocationListView, ProductCountingLocationListViewModel>(nameof(ProductCountingLocationListView));
        builder.Services.AddScopedWithShellRoute<ProductCountingBasketView, ProductCountingBasketViewModel>(nameof(ProductCountingBasketView));
        builder.Services.AddScopedWithShellRoute<ProductCountingFormView, ProductCountingFormViewModel>(nameof(ProductCountingFormView));

        #endregion Urune Gore Sayim

        #endregion Counting Modules

        #region Quickly Production Modules

        builder.Services.AddSingletonWithShellRoute<QuicklyProductionPanelView, QuicklyProductionPanelViewModel>(nameof(QuicklyProductionPanelView));
        builder.Services.AddSingletonWithShellRoute<QuicklyProductionBOMMenuView, QuicklyProductionBOMMenuViewModel>(nameof(QuicklyProductionBOMMenuView));
        builder.Services.AddSingletonWithShellRoute<QuicklyProductionProcessView, QuicklyProductionProcessViewModel>(nameof(QuicklyProductionProcessView));

        builder.Services.AddScopedWithShellRoute<QuicklyProductionBOMMenuDetailView, QuicklyProductionBOMMenuDetailView>(nameof(QuicklyProductionBOMMenuDetailView));

        builder.Services.AddSingletonWithShellRoute<QuicklyProductionPanelAllFicheListView, QuicklyProductionPanelAllFicheListViewModel>(nameof(QuicklyProductionPanelAllFicheListView));

        builder.Services.AddScopedWithShellRoute<QuicklyProductionInputProductListView, QuicklyProductionInputProductListViewModel>(nameof(QuicklyProductionInputProductListView));
        builder.Services.AddScopedWithShellRoute<QuicklyProductionOutputProductListView, QuicklyProductionOutputProductListViewModel>(nameof(QuicklyProductionOutputProductListView));

        #region Quickly Production Manuel

        builder.Services.AddScopedWithShellRoute<ManuelProductListView, ManuelProductListViewModel>(nameof(ManuelProductListView));
        builder.Services.AddScopedWithShellRoute<ManuelCalcView, ManuelCalcViewModel>(nameof(ManuelCalcView));
        builder.Services.AddScopedWithShellRoute<ManuelCalcInWarehouseListView, ManuelCalcInWarehouseListViewModel>(nameof(ManuelCalcInWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ManuelCalcOutWarehouseListView, ManuelCalcOutWarehouseListViewModel>(nameof(ManuelCalcOutWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ManuelCalcSubProductListView, ManuelCalcSubProductListViewModel>(nameof(ManuelCalcSubProductListView));
        builder.Services.AddScopedWithShellRoute<ManuelFormListView, ManuelFormListViewModel>(nameof(ManuelFormListView));

        #endregion Quickly Production Manuel

        #region Quickly Production WorkOrder

        builder.Services.AddScopedWithShellRoute<WorkOrderProductListView, WorkOrderProductListViewModel>(nameof(WorkOrderProductListView));
        builder.Services.AddScopedWithShellRoute<WorkOrderCalcView, WorkOrderCalcViewModel>(nameof(WorkOrderCalcView));
        builder.Services.AddScopedWithShellRoute<WorkOrderProductLocationListView, WorkOrderProductLocationListViewModel>(nameof(WorkOrderProductLocationListView));
        builder.Services.AddScopedWithShellRoute<WorkOrderFormView, WorkOrderFormViewModel>(nameof(WorkOrderFormView));

        #endregion Quickly Production WorkOrder

        #region Manuel Rework

        builder.Services.AddScopedWithShellRoute<ManuelReworkProcessOutWarehouseListView, ManuelReworkProcessOutWarehouseListViewModel>(nameof(ManuelReworkProcessOutWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ManuelReworkProcessWarehouseTotalListView, ManuelReworkProcessWarehouseTotalListViewModel>(nameof(ManuelReworkProcessWarehouseTotalListView));
        builder.Services.AddScopedWithShellRoute<ManuelReworkProcessBasketView, ManuelReworkProcessBasketViewModel>(nameof(ManuelReworkProcessBasketView));
        builder.Services.AddScopedWithShellRoute<ManuelReworkProcessBasketLocationListView, ManuelReworkProcessBasketLocationListViewModel>(nameof(ManuelReworkProcessBasketLocationListView));
        builder.Services.AddScopedWithShellRoute<ManuelReworkProcessInWarehouseListView, ManuelReworkProcessInWarehouseListViewModel>(nameof(ManuelReworkProcessInWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ManuelReworkProcessAllProductListView, ManuelReworkProcessAllProductListViewModel>(nameof(ManuelReworkProcessAllProductListView));
        builder.Services.AddScopedWithShellRoute<ManuelReworkProcessFormView, ManuelReworkProcessFormViewModel>(nameof(ManuelReworkProcessFormView));

        #endregion Manuel Rework

        #region Work Order Rework

        builder.Services.AddScopedWithShellRoute<WorkOrderReworkProcessProductListView, WorkOrderReworkProcessProductListViewModel>(nameof(WorkOrderReworkProcessProductListView));
        builder.Services.AddScopedWithShellRoute<WorkOrderReworkProcessBasketView, WorkOrderReworkProcessBasketViewModel>(nameof(WorkOrderReworkProcessBasketView));
        builder.Services.AddScopedWithShellRoute<WorkOrderReworkProcessSubProductLocationListView, WorkOrderReworkProcessSubProductLocationListViewModel>(nameof(WorkOrderReworkProcessSubProductLocationListView));
        builder.Services.AddScopedWithShellRoute<WorkOrderReworkProcessFormView, WorkOrderReworkProcessFormViewModel>(nameof(WorkOrderReworkProcessFormView));

        #endregion Work Order Rework

        #endregion Quickly Production Modules

        #region Result Modules

        builder.Services.AddTransientWithShellRoute<InsertSuccessPageView, InsertSuccessPageViewModel>(nameof(InsertSuccessPageView));
        builder.Services.AddTransientWithShellRoute<InsertFailurePageView, InsertFailurePageViewModel>(nameof(InsertFailurePageView));

        #endregion Result Modules

        #region Outsource Modules

        builder.Services.AddSingletonWithShellRoute<OutsourcePanelView, OutsourcePanelViewModel>(nameof(OutsourcePanelView));
        builder.Services.AddSingletonWithShellRoute<OutsourceListView, OutsourceListViewModel>(nameof(OutsourceListView));
        builder.Services.AddScopedWithShellRoute<OutsourceDetailView, OutsourceDetailViewModel>(nameof(OutsourceDetailView));
        builder.Services.AddSingletonWithShellRoute<WaitingOutsourceProductListView, WaitingOutsourceProductListViewModel>(nameof(WaitingOutsourceProductListView));
        builder.Services.AddSingletonWithShellRoute<OutsourceProcessView, OutsourceProcessViewModel>(nameof(OutsourceProcessView));

        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferWarehouseListView, OutputOutsourceTransferWarehouseListViewModel>(nameof(OutputOutsourceTransferWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferBasketListView, OutputOutsourceTransferBasketListViewModel>(nameof(OutputOutsourceTransferBasketListView));
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferProductListView, OutputOutsourceTransferProductListViewModel>(nameof(OutputOutsourceTransferProductListView));
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferFormView, OutputOutsourceTransferFormViewModel>(nameof(OutputOutsourceTransferFormView));

        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferWarehouseListView, InputOutsourceTransferWarehouseListViewModel>(nameof(InputOutsourceTransferWarehouseListView));

        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferOutsourceSupplierListView, InputOutsourceTransferOutsourceSupplierListViewModel>(nameof(InputOutsourceTransferOutsourceSupplierListView));

        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferOutsourceProductListView, InputOutsourceTransferOutsourceProductListViewModel>(nameof(InputOutsourceTransferOutsourceProductListView));

        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferOutsourceBasketListView, InputOutsourceTransferOutsourceBasketListViewModel>(nameof(InputOutsourceTransferOutsourceBasketListView));

        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferBasketLocationListView, InputOutsourceTransferBasketLocationListViewModel>(nameof(InputOutsourceTransferBasketLocationListView));

        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferOutsourceFormView, InputOutsourceTransferOutsourceFormViewModel>(nameof(InputOutsourceTransferOutsourceFormView));

        #region OutputTransferv2
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferV2WarehouseListView,OutputOutsourceTransferV2WarehouseListViewModel>(nameof(OutputOutsourceTransferV2WarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferV2OutsourceSupplierListView, OutputOutsourceTransferV2OutsourceSupplierListViewModel>(nameof(OutputOutsourceTransferV2OutsourceSupplierListView));
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferV2ProductListView, OutputOutsourceTransferV2ProductListViewModel>(nameof(OutputOutsourceTransferV2ProductListView));
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferV2OutsourceBasketView, OutputOutsourceTransferV2OutsourceBasketViewModel>(nameof(OutputOutsourceTransferV2OutsourceBasketView));
        builder.Services.AddScopedWithShellRoute<OutputOutsourceTransferV2OutsourceFormView, OutputOutsourceTransferV2OutsourceFormViewModel>(nameof(OutputOutsourceTransferV2OutsourceFormView));

        #endregion

        #region InputOutsourceTransferV2
        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferV2WarehouseListView, InputOutsourceTransferV2WarehouseListViewModel>(nameof(InputOutsourceTransferV2WarehouseListView));
		builder.Services.AddScopedWithShellRoute<InputOutsourceTransferV2SupplierListView, InputOutsourceTransferV2SupplierListViewModel>(nameof(InputOutsourceTransferV2SupplierListView));
        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferV2ProductListView, InputOutsourceTransferV2ProductListViewModel>(nameof(InputOutsourceTransferV2ProductListView));
		builder.Services.AddScopedWithShellRoute<InputOutsourceTransferV2BasketView, InputOutsourceTransferV2BasketViewModel>(nameof(InputOutsourceTransferV2BasketView));
        builder.Services.AddScopedWithShellRoute<InputOutsourceTransferV2MainProductLocationListView, InputOutsourceTransferV2MainProductLocationListViewModel>(nameof(InputOutsourceTransferV2MainProductLocationListView));
		builder.Services.AddScopedWithShellRoute<InputOutsourceTransferV2FormView, InputOutsourceTransferV2FormViewModel>(nameof(InputOutsourceTransferV2FormView));
		#endregion

		#region OutSource Panel

		builder.Services.AddScopedWithShellRoute<OutsourcePanelAllFicheListView, OutsourcePanelAllFicheListViewModel>(nameof(OutsourcePanelAllFicheListView));

        builder.Services.AddScopedWithShellRoute<OutsourceInputTransactionView, OutsourceInputTransactionViewModel>(nameof(OutsourceInputTransactionView));
        builder.Services.AddScopedWithShellRoute<OutsourceOutputTransactionView, OutsourceOutputTransactionViewModel>(nameof(OutsourceOutputTransactionView));
        builder.Services.AddScopedWithShellRoute<OutsourceDetailAllFichesView, OutsourceDetailAllFichesViewModel>(nameof(OutsourceDetailAllFichesView));

        #endregion OutSource Panel

        #endregion Outsource Modules

        #region Camera Module

        builder.Services.AddScopedWithShellRoute<CameraReaderView, CameraReaderViewModel>(nameof(CameraReaderView));

        #endregion Camera Module

        #region TransactionSchedulerModule

        builder.Services.AddScopedWithShellRoute<TransactionSchedulerView, TransactionSchedulerViewModel>(nameof(TransactionSchedulerView));

        #endregion TransactionSchedulerModule

        #region Task Module

        builder.Services.AddSingletonWithShellRoute<TaskListView, TaskListViewModel>(nameof(TaskListView));

        #endregion Task Module

        return builder.Build();
    }
}