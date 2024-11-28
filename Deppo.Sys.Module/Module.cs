using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using Deppo.Sys.Module.Helpers.HttpClientHelpers;
using Microsoft.Extensions.Configuration;
using System.Security.AccessControl;
using DevExpress.Xpo.Metadata;


namespace Deppo.Sys.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class SysModule : ModuleBase
{
    public SysModule()
    {
        // 
        // SysModule
        // 
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.ModelDifference));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.ModelDifferenceAspect));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.BaseObject));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileData));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileAttachmentBase));
        AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Analysis));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Dashboards.DashboardsModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.PivotChartModuleBase));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotGrid.PivotGridModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));


    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application)
    {
        base.Setup(application);
        application.SetupComplete += Application_SetupComplete;
    }
    private void Application_SetupComplete(object sender, EventArgs e)
    {
        Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
    }
    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
    {
        var nonPersistentObjectSpace = e.ObjectSpace as NonPersistentObjectSpace;
        if (nonPersistentObjectSpace != null)
        {
            //var os = Application.CreateObjectSpace(typeof(ProductModel));
            //nonPersistentObjectSpace.AdditionalObjectSpaces.Add(os);

            //nonPersistentObjectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
        }
    }
    private void ObjectSpace_ObjectsGetting(Object sender, ObjectsGettingEventArgs e)
    {
        //var httpClientService = Application.ServiceProvider.GetService(typeof(IHttpClientService)) as IHttpClientService;
        //var configurationService = Application.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
        //var authService = Application.ServiceProvider.GetService(typeof(IAuthenticationService)) as IAuthenticationService;

        //httpClientService.BaseUri = configurationService["LBSSetting:BaseUri"];

        //var httpClient = httpClientService.GetOrCreateHttpClient();
        //var username = configurationService["LBSSetting:Username"];
        //var password = configurationService["LBSSetting:Password"];

        //var token = System.Threading.Tasks.Task.Run(() => authService.Authenticate(httpClient, username, password)).Result;

        //if (!string.IsNullOrEmpty(token))
        //    httpClientService.Token = token;

        //if (e.ObjectType == typeof(ProductModel))
        //{
        //    var productService = Application.ServiceProvider.GetService(typeof(IProductService)) as IProductService;

        //    var items = System.Threading.Tasks.Task.Run(() => new ProductModelDataStore(httpClientService, productService).GetObjectsAsync()).Result;

        //    e.Objects = items;
        //}
        
    }

    

    public override void CustomizeTypesInfo(ITypesInfo typesInfo)
    {
        base.CustomizeTypesInfo(typesInfo);
        
    }
}
