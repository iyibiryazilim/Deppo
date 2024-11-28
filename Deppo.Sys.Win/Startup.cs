using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Win.ApplicationBuilder;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraEditors;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Design;
using Deppo.Sys.Module.CustomLogonSecurity;
using Deppo.Sys.Module.Helpers.HttpClientHelpers;

namespace Deppo.Sys.Win;

public class ApplicationBuilder : IDesignTimeApplicationFactory {
    public static WinApplication BuildApplication(string connectionString) {
        var builder = WinApplication.CreateBuilder();
        
        builder.UseApplication<SysWindowsFormsApplication>();
        
        builder.Modules
            .AddCloningXpo()
            .AddConditionalAppearance()
            .AddDashboards(options => {
                options.DashboardDataType = typeof(DevExpress.Persistent.BaseImpl.DashboardData);
                options.DesignerFormStyle = DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon;
            })
            .AddFileAttachments()
            .AddPivotChart()
            .AddPivotGrid()
            .AddReports(options => {
                options.EnableInplaceReports = true;
                options.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
                options.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
            })
            .AddValidation(options => {
                options.AllowValidationDetailsAccess = false;
            })
            .AddViewVariants()
            .Add<Deppo.Sys.Module.SysModule>()
        	.Add<SysWinModule>();
        builder.ObjectSpaceProviders
            .AddSecuredXpo((application, options) => {
                options.ConnectionString = connectionString;
            })
            .AddNonPersistent();
        builder.Security
            .UseIntegratedMode(options =>
            {
                options.RoleType = typeof(PermissionPolicyRole);
                options.UserType = typeof(Deppo.Sys.Module.BusinessObjects.ApplicationUser);
                options.UserLoginInfoType = typeof(Deppo.Sys.Module.BusinessObjects.ApplicationUserLoginInfo);
                options.UseXpoPermissionsCaching();
                options.Events.OnSecurityStrategyCreated = securityStrategyBase => {
                    var securityStrategy = (SecurityStrategy)securityStrategyBase;
                    securityStrategy.Authentication = new CustomAuthentication();
                    //securityStrategy.AnonymousAllowedTypes.Add(typeof(Company));
                    //securityStrategy.AnonymousAllowedTypes.Add(typeof(ApplicationUser));
                };
            });
            //.UsePasswordAuthentication();
        builder.AddBuildStep(application => {
            application.ConnectionString = connectionString;
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached && application.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                application.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
#endif
        });
        var winApplication = builder.Build();
        return winApplication;
    }

    XafApplication IDesignTimeApplicationFactory.Create()
        => BuildApplication(XafApplication.DesignTimeConnectionString);
}
