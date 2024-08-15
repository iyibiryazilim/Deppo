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
using Deppo.Mobile.Modules.ProductModule.ProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.Views;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.Views;
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
				#endregion

				#region ToastConfig
				ToastConfig.DefaultCornerRadius = 15;
				#endregion

				#region Confirm Config
				ConfirmConfig.DefaultBackgroundColor = Colors.White;
				ConfirmConfig.DefaultTitleFontSize = 16;
				ConfirmConfig.DefaultTitleColor = Colors.Black;
				ConfirmConfig.DefaultMessageFontSize = 14;
				ConfirmConfig.DefaultPositiveButtonFontSize = 14;
				ConfirmConfig.DefaultNegativeButtonFontSize = 14;
				ConfirmConfig.DefaultCancelText = "İptal";
				ConfirmConfig.DefaultOkText = "Evet";
				#endregion
			})
#if DEBUG
			.EnableHotReload()
#endif
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
				fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
				fonts.AddFont("Roboto-Thin.ttf", "RobotoThin");
				fonts.AddFont("Roboto-ThinItalic.ttf", "RobotoThinItalic");
				#endregion

				#region FontAwesome
				fonts.AddFont("fa-solid.otf", "FAS");
				fonts.AddFont("fa-regular.otf", "FAR");
				fonts.AddFont("fa-brands.otf", "FAB");
				#endregion
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton(UserDialogs.Instance);
		builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
		builder.Services.AddSingleton<IAuthenticationService, AuthenticateDataStore>();
		builder.Services.AddSingleton<ICustomQueryService, CustomQueryDataStore>();
		builder.Services.AddSingleton<IProductService, ProductDataStore>();
		builder.Services.AddSingleton<IWarehouseService, WarehouseDataStore>();
		builder.Services.AddSingleton<ICustomerService, CustomerDataStore>();
		builder.Services.AddSingleton<IWaitingSalesOrderService, WaitingSalesOrderDataStore>();
		builder.Services.AddSingleton<ISupplierService, SupplierDataStore>();

		builder.Services.AddSingletonWithShellRoute<LoginView, LoginViewModel>(nameof(LoginView));
		builder.Services.AddTransientWithShellRoute<LoginParameterView, LoginParameterViewModel>(nameof(LoginParameterView));

		#region Analysis Modules
		builder.Services.AddSingletonWithShellRoute<OverviewAnalysisView, OverviewAnalysisViewModel>(nameof(OverviewAnalysisView));
		builder.Services.AddSingletonWithShellRoute<ProductAnalysisView, ProductAnalysisViewModel>(nameof(ProductAnalysisView));
		builder.Services.AddSingletonWithShellRoute<PurchaseAnalysisView, PurchaseAnalysisViewModel>(nameof(PurchaseAnalysisView));
		builder.Services.AddSingletonWithShellRoute<SalesAnalysisView, SalesAnalysisViewModel>(nameof(SalesAnalysisView));

		#endregion

		#region Product Modules
		builder.Services.AddSingletonWithShellRoute<ProductPanelView, ProductPanelViewModel>(nameof(ProductPanelView));
		builder.Services.AddSingletonWithShellRoute<ProductListView, ProductListViewModel>(nameof(ProductListView));
		builder.Services.AddSingletonWithShellRoute<WarehouseListView, WarehouseListViewModel>(nameof(WarehouseListView));
		builder.Services.AddSingletonWithShellRoute<ProductProcessView, ProductProcessViewModel>(nameof(ProductProcessView));

		#endregion

		#region Sales Modules
		builder.Services.AddSingletonWithShellRoute<SalesPanelView, SalesPanelViewModel>(nameof(SalesPanelView));
		builder.Services.AddSingletonWithShellRoute<CustomerListView, CustomerListViewModel>(nameof(CustomerListView));
		builder.Services.AddSingletonWithShellRoute<WaitingSalesOrderListView, WaitingSalesOrderListViewModel>(nameof(WaitingSalesOrderListView));
		builder.Services.AddSingletonWithShellRoute<SalesProcessView, SalesProcessViewModel>(nameof(SalesProcessView));
		#endregion

		#region Purchase Modules
		builder.Services.AddSingletonWithShellRoute<PurchasePanelView, PurchasePanelViewModel>(nameof(PurchasePanelView));
		builder.Services.AddSingletonWithShellRoute<SupplierListView, SupplierListViewModel>(nameof(SupplierListView));
		builder.Services.AddSingletonWithShellRoute<WaitingPurchaseOrderListView, WaitingPurchaseOrderListViewModel>(nameof(WaitingPurchaseOrderListView));
		builder.Services.AddSingletonWithShellRoute<PurchaseProcessView, PurchaseProcessViewModel>(nameof(PurchaseProcessView));
		#endregion

		#region Counting Modules
		builder.Services.AddSingletonWithShellRoute<CountingWarehouseListView, CountingWarehouseListViewModel>(nameof(CountingWarehouseListView));

		#endregion

		#region Fast Production Modules
		builder.Services.AddSingletonWithShellRoute<FastProductionProductView, FastProductionProductViewModel>(nameof(FastProductionProductView));
			
		#endregion

		return builder.Build();
	}
}
