using CommunityToolkit.Maui;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.DataStores;
using Deppo.Core.Services;
using Deppo.Mobile.Core.DataStores;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.Views;
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
using Deppo.Mobile.Modules.CountingModule.ViewModels;
using Deppo.Mobile.Modules.CountingModule.Views;
using Deppo.Mobile.Modules.FastProductionModule.ViewModels;
using Deppo.Mobile.Modules.FastProductionModule.Views;
using Deppo.Mobile.Modules.LoginModule.ViewModels;
using Deppo.Mobile.Modules.LoginModule.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceMenu.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourcePanel.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.Views;
using Deppo.Mobile.Modules.OutsourceModule.WaitingOutsourceMenu.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.WaitingOutsourceMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductMenu.Views;
using Deppo.Mobile.Modules.ProductModule.ProductPanel.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductPanel.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ProductionInput.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
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
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionPanel.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Views;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;
using Deppo.Mobile.Modules.ResultModule.ViewModels;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesDispatchProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.ViewModels;
using Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.Views;
using DevExpress.Maui;
using DotNet.Meteor.HotReload.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton(UserDialogs.Instance);
        builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
        builder.Services.AddSingleton<IAuthenticationService, AuthenticateDataStore>();
        builder.Services.AddSingleton<ICustomQueryService, CustomQueryDataStore>();
        builder.Services.AddSingleton<IProductService, ProductDataStoreV2>();
        builder.Services.AddSingleton<IWarehouseService, WarehouseDataStore>();
        builder.Services.AddSingleton<ICustomerService, CustomerDataStoreV2>();
        builder.Services.AddSingleton<IWaitingSalesOrderService, WaitingSalesOrderDataStore>();
        builder.Services.AddSingleton<ISupplierService, SupplierDataStoreV2>();
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
        builder.Services.AddSingleton<IWarehouseCountingService, WarehouseCountingDataStore>();
        builder.Services.AddSingleton<IProductCountingService, ProductCountingDataStore>();

        builder.Services.AddSingleton<IQuicklyBomService, QuicklyBomDataStore>();
        builder.Services.AddSingleton<IOutsourcePanelService, OutsourcePanelDataStore>();
        builder.Services.AddSingleton<ICountingPanelService, CountingPanelDataStore>();
        builder.Services.AddSingleton<IQuicklyProductionPanelService, QuicklyProductionPanelDataStore>();

        builder.Services.AddSingletonWithShellRoute<LoginView, LoginViewModel>(nameof(LoginView));
        builder.Services.AddTransientWithShellRoute<LoginParameterView, LoginParameterViewModel>(nameof(LoginParameterView));
        builder.Services.AddTransientWithShellRoute<CompanyListView, CompanyListViewModel>(nameof(CompanyListView));

        #region Analysis Modules

        builder.Services.AddSingletonWithShellRoute<OverviewAnalysisView, OverviewAnalysisViewModel>(nameof(OverviewAnalysisView));
        builder.Services.AddSingletonWithShellRoute<ProductAnalysisView, ProductAnalysisViewModel>(nameof(ProductAnalysisView));
        builder.Services.AddSingletonWithShellRoute<PurchaseAnalysisView, PurchaseAnalysisViewModel>(nameof(PurchaseAnalysisView));
        builder.Services.AddSingletonWithShellRoute<SalesAnalysisView, SalesAnalysisViewModel>(nameof(SalesAnalysisView));

        #endregion Analysis Modules

        #region Product Modules

        builder.Services.AddSingletonWithShellRoute<ProductPanelView, ProductPanelViewModel>(nameof(ProductPanelView));
        builder.Services.AddTransientWithShellRoute<InputProductListView, InputProductListViewModel>(nameof(InputProductListView));
		builder.Services.AddTransientWithShellRoute<OutputProductListView, OutputProductListViewModel>(nameof(OutputProductListView));
		builder.Services.AddTransientWithShellRoute<AllFicheListView, AllFicheListViewModel>(nameof(AllFicheListView));

		builder.Services.AddSingletonWithShellRoute<ProductListView, ProductListViewModel>(nameof(ProductListView));
        builder.Services.AddScopedWithShellRoute<ProductDetailView, ProductDetailViewModel>(nameof(ProductDetailView));
        builder.Services.AddSingletonWithShellRoute<ProductInputTransactionView, ProductInputTransactionViewModel>(nameof(ProductInputTransactionView));
        builder.Services.AddSingletonWithShellRoute<ProductOutputTransactionView, ProductOutputTransactionViewModel>(nameof(ProductOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<WarehouseListView, WarehouseListViewModel>(nameof(WarehouseListView));
        builder.Services.AddScopedWithShellRoute<WarehouseDetailView, WarehouseDetailViewModel>(nameof(WarehouseDetailView));
        builder.Services.AddTransientWithShellRoute<WarehouseInputTransactionView, WarehouseInputTransactionViewModel>(nameof(WarehouseInputTransactionView));
        builder.Services.AddTransientWithShellRoute<WarehouseOutputTransactionView, WarehouseOutputTransactionViewModel>(nameof(WarehouseOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<ProductProcessView, ProductProcessViewModel>(nameof(ProductProcessView));

        #region ProductionInput Modules

        builder.Services.AddTransientWithShellRoute<InputProductProcessWarehouseListView, InputProductProcessWarehouseListViewModel>(nameof(InputProductProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessBasketListView, InputProductProcessBasketListViewModel>(nameof(InputProductProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessBasketLocationListView, InputProductProcessBasketLocationListViewModel>(nameof(InputProductProcessBasketLocationListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessBasketSeriLotListView, InputProductProcessBasketSeriLotListViewModel>(nameof(InputProductProcessBasketSeriLotListView));
        builder.Services.AddTransientWithShellRoute<InputProductProcessProductListView, InputProductProcessProductListViewModel>(nameof(InputProductProcessProductListView));
        builder.Services.AddScopedWithShellRoute<InputProductProcessFormView, InputProductProcessFormViewModel>(nameof(InputProductProcessFormView));

        #endregion ProductionInput Modules

        #region OutputProductProcess Modules

        builder.Services.AddTransientWithShellRoute<OutputProductProcessWarehouseListView, OutputProductProcessWarehouseListViewModel>(nameof(OutputProductProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputProductProcessBasketListView, OutputProductProcessBasketListViewModel>(nameof(OutputProductProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<OutputProductProcessProductListView, OutputProductProcessProductListViewModel>(nameof(OutputProductProcessProductListView));
        builder.Services.AddTransientWithShellRoute<OutputProductProcessFormView, OutputProductProcessFormViewModel>(nameof(OutputProductProcessFormView));

        #endregion OutputProductProcess Modules

        #region TransferProductProcess Modules

        builder.Services.AddTransientWithShellRoute<TransferOutWarehouseListView, TransferOutWarehouseListViewModel>(nameof(TransferOutWarehouseListView));
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
        builder.Services.AddSingletonWithShellRoute<CustomerDetailView, CustomerDetailViewModel>(nameof(CustomerDetailView));
        builder.Services.AddSingletonWithShellRoute<CustomerInputTransactionView, CustomerInputTransactionViewModel>(nameof(CustomerInputTransactionView));
        builder.Services.AddSingletonWithShellRoute<CustomerOutputTransactionView, CustomerOutputTransactionViewModel>(nameof(CustomerOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<WaitingSalesOrderListView, WaitingSalesOrderListViewModel>(nameof(WaitingSalesOrderListView));
        builder.Services.AddSingletonWithShellRoute<SalesProcessView, SalesProcessViewModel>(nameof(SalesProcessView));

        #region Sevk Islemleri

        builder.Services.AddTransientWithShellRoute<OutputProductSalesProcessWarehouseListView, OutputProductSalesProcessWarehouseListViewModel>(nameof(OutputProductSalesProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessCustomerListView, OutputProductSalesProcessCustomerListViewModel>(nameof(OutputProductSalesProcessCustomerListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessBasketListView, OutputProductSalesProcessBasketListViewModel>(nameof(OutputProductSalesProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessProductListView, OutputProductSalesProcessProductListViewModel>(nameof(OutputProductSalesProcessProductListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesProcessFormView, OutputProductSalesProcessFormViewModel>(nameof(OutputProductSalesProcessFormView));

        #endregion Sevk Islemleri

        #region Siparise Bagli Sevk Islemleri

        builder.Services.AddTransientWithShellRoute<OutputProductSalesOrderProcessWarehouseListView, OutputProductSalesOrderProcessWarehouseListViewModel>(nameof(OutputProductSalesOrderProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessCustomerListView, OutputProductSalesOrderProcessCustomerListViewModel>(nameof(OutputProductSalesOrderProcessCustomerListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessProductListView, OutputProductSalesOrderProcessProductListViewModel>(nameof(OutputProductSalesOrderProcessProductListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessBasketListView, OutputProductSalesOrderProcessBasketListViewModel>(nameof(OutputProductSalesOrderProcessBasketListView));
        builder.Services.AddScopedWithShellRoute<OutputProductSalesOrderProcessFormView, OutputProductSalesOrderProcessFormViewModel>(nameof(OutputProductSalesOrderProcessFormView));

        #endregion Siparise Bagli Sevk Islemleri

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

        #endregion Sales Modules

        #region Purchase Modules

        builder.Services.AddSingletonWithShellRoute<PurchasePanelView, PurchasePanelViewModel>(nameof(PurchasePanelView));
        builder.Services.AddSingletonWithShellRoute<SupplierListView, SupplierListViewModel>(nameof(SupplierListView));
        builder.Services.AddSingletonWithShellRoute<SupplierDetailView, SupplierDetailViewModel>(nameof(SupplierDetailView));
        builder.Services.AddSingletonWithShellRoute<SupplierInputTransactionView, SupplierInputTransactionViewModel>(nameof(SupplierInputTransactionView));
        builder.Services.AddSingletonWithShellRoute<SupplierOutputTransactionView, SupplierOutputTransactionViewModel>(nameof(SupplierOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<WaitingPurchaseOrderListView, WaitingPurchaseOrderListViewModel>(nameof(WaitingPurchaseOrderListView));
        builder.Services.AddSingletonWithShellRoute<PurchaseProcessView, PurchaseProcessViewModel>(nameof(PurchaseProcessView));

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

        #region Quickly Production Manuel

        builder.Services.AddSingletonWithShellRoute<ManuelProductListView, ManuelProductListViewModel>(nameof(ManuelProductListView));
        builder.Services.AddScopedWithShellRoute<ManuelCalcView, ManuelCalcViewModel>(nameof(ManuelCalcView));
        builder.Services.AddTransientWithShellRoute<ManuelCalcInWarehouseListView, ManuelCalcInWarehouseListViewModel>(nameof(ManuelCalcInWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ManuelCalcOutWarehouseListView, ManuelCalcOutWarehouseListViewModel>(nameof(ManuelCalcOutWarehouseListView));
        builder.Services.AddScopedWithShellRoute<ManuelCalcSubProductListView, ManuelCalcSubProductListViewModel>(nameof(ManuelCalcSubProductListView));
        builder.Services.AddTransientWithShellRoute<ManuelFormListView, ManuelFormListViewModel>(nameof(ManuelFormListView));

        #endregion Quickly Production Manuel

        #region Quickly Production WorkOrder

        builder.Services.AddSingletonWithShellRoute<WorkOrderProductListView, WorkOrderProductListViewModel>(nameof(WorkOrderProductListView));
        builder.Services.AddScopedWithShellRoute<WorkOrderCalcView, WorkOrderCalcViewModel>(nameof(WorkOrderCalcView));
        builder.Services.AddTransientWithShellRoute<WorkOrderFormView, WorkOrderFormViewModel>(nameof(WorkOrderFormView));

        #endregion Quickly Production WorkOrder

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

        #region OutSource Panel

        builder.Services.AddScopedWithShellRoute<OutsourcePanelAllFicheListView, OutsourcePanelAllFicheListViewModel>(nameof(OutsourcePanelAllFicheListView));

        #endregion

        #endregion Outsource Modules

        return builder.Build();
    }
}