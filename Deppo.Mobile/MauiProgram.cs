using CommunityToolkit.Maui;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.DataStores;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.OverviewAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.ProductAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.PurchaseAnalysis.Views;
using Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.ViewModels;
using Deppo.Mobile.Modules.AnalysisModule.SalesAnalysis.Views;
using Deppo.Mobile.Modules.CountingModule.ViewModels;
using Deppo.Mobile.Modules.CountingModule.Views;
using Deppo.Mobile.Modules.FastProductionModule.ViewModels;
using Deppo.Mobile.Modules.FastProductionModule.Views;
using Deppo.Mobile.Modules.LoginModule.ViewModels;
using Deppo.Mobile.Modules.LoginModule.Views;
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
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.WaitingOrderMenu.Views;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.ViewModels;
using Deppo.Mobile.Modules.SalesModule.CustomerMenu.Views;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesPanel.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.Views;
using Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.ViewModels;
using Deppo.Mobile.Modules.SalesModule.WaitingOrderMenu.Views;
using DevExpress.Maui;
using DotNet.Meteor.HotReload.Plugin;
using Microsoft.Extensions.Logging;

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
            .UseDevExpressCollectionView()
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
        builder.Services.AddSingleton<IWarehouseTransactionService, WarehouseTransactionDataStore>();
        builder.Services.AddSingleton<ISupplierTransactionLineService, SupplierTransactionLineDataStore>();
        builder.Services.AddSingleton<ICustomerTransactionLineService, CustomerTransactionLineDataStore>();
        builder.Services.AddSingleton<IProductTransactionLineService, ProductTransactionLineDataStore>();
        builder.Services.AddSingleton<IVariantService, VariantDataStore>();
        builder.Services.AddSingleton<IWarehouseTotalService, WarehouseTotalDataStore>();
        builder.Services.AddSingleton<ISerilotTransactionService, SerilotTransactionDataStore>();
        builder.Services.AddSingleton<ILocationTransactionService, LocationTransactionDataStore>();

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
        builder.Services.AddTransientWithShellRoute<InputProductProcessProductListView, InputProductProcessProductListViewModel>(nameof(InputProductProcessProductListView));

        #endregion ProductionInput Modules

        #region OutputProductProcess Modules

        builder.Services.AddTransientWithShellRoute<OutputProductProcessWarehouseListView, OutputProductProcessWarehouseListViewModel>(nameof(OutputProductProcessWarehouseListView));
        builder.Services.AddScopedWithShellRoute<OutputProductProcessBasketListView, OutputProductProcessBasketListViewModel>(nameof(OutputProductProcessBasketListView));
        builder.Services.AddTransientWithShellRoute<OutputProductProcessProductListView, OutputProductProcessProductListViewModel>(nameof(OutputProductProcessProductListView));
        builder.Services.AddTransientWithShellRoute<OutputProductProcessFormView, OutputProductProcessFormViewModel>(nameof(OutputProductProcessFormView));
        #endregion OutputProductProcess Modules

        #endregion Product Modules

        #region Sales Modules

        builder.Services.AddSingletonWithShellRoute<SalesPanelView, SalesPanelViewModel>(nameof(SalesPanelView));
        builder.Services.AddSingletonWithShellRoute<CustomerListView, CustomerListViewModel>(nameof(CustomerListView));
        builder.Services.AddSingletonWithShellRoute<CustomerDetailView, CustomerDetailViewModel>(nameof(CustomerDetailView));
        builder.Services.AddSingletonWithShellRoute<CustomerInputTransactionView, CustomerInputTransactionViewModel>(nameof(CustomerInputTransactionView));
        builder.Services.AddSingletonWithShellRoute<CustomerOutputTransactionView, CustomerOutputTransactionViewModel>(nameof(CustomerOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<WaitingSalesOrderListView, WaitingSalesOrderListViewModel>(nameof(WaitingSalesOrderListView));
        builder.Services.AddSingletonWithShellRoute<SalesProcessView, SalesProcessViewModel>(nameof(SalesProcessView));

        #endregion Sales Modules

        #region Purchase Modules

        builder.Services.AddSingletonWithShellRoute<PurchasePanelView, PurchasePanelViewModel>(nameof(PurchasePanelView));
        builder.Services.AddSingletonWithShellRoute<SupplierListView, SupplierListViewModel>(nameof(SupplierListView));
        builder.Services.AddSingletonWithShellRoute<SupplierDetailView, SupplierDetailViewModel>(nameof(SupplierDetailView));
        builder.Services.AddSingletonWithShellRoute<SupplierInputTransactionView, SupplierInputTransactionViewModel>(nameof(SupplierInputTransactionView));
        builder.Services.AddSingletonWithShellRoute<SupplierOutputTransactionView, SupplierOutputTransactionViewModel>(nameof(SupplierOutputTransactionView));
        builder.Services.AddSingletonWithShellRoute<WaitingPurchaseOrderListView, WaitingPurchaseOrderListViewModel>(nameof(WaitingPurchaseOrderListView));
        builder.Services.AddSingletonWithShellRoute<PurchaseProcessView, PurchaseProcessViewModel>(nameof(PurchaseProcessView));
        builder.Services.AddSingletonWithShellRoute<InputProductProcessPurchaseWarehouseListView, InputProductProcessPurchaseWarehouseListViewModel>(nameof(InputProductProcessPurchaseWarehouseListView));
        builder.Services.AddSingletonWithShellRoute<InputProductProcessPurchaseSupplierListView, InputProductProcessPurchaseSupplierListViewModel>(nameof(InputProductProcessPurchaseSupplierListView));
        builder.Services.AddSingletonWithShellRoute<InputProductProcessPurchaseBasketListView, InputProductProcessPurchaseBasketListViewModel>(nameof(InputProductProcessPurchaseBasketListView));
        builder.Services.AddSingletonWithShellRoute<InputProductProcessPurchaseProductListView, InputProductProcessPurchaseProductListViewModel>
            (nameof(InputProductProcessPurchaseProductListView));

        #endregion Purchase Modules

        #region Counting Modules

        builder.Services.AddSingletonWithShellRoute<CountingWarehouseListView, CountingWarehouseListViewModel>(nameof(CountingWarehouseListView));

        #endregion Counting Modules

        #region Fast Production Modules

        builder.Services.AddSingletonWithShellRoute<FastProductionProductView, FastProductionProductViewModel>(nameof(FastProductionProductView));

        #endregion Fast Production Modules

        return builder.Build();
    }
}