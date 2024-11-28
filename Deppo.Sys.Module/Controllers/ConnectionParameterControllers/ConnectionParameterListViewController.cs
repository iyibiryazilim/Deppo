using Deppo.Sys.Module.BusinessObjects;
using Deppo.Sys.Module.LogoBusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.Controllers.ConnectionParameterControllers
{
    
    public partial class ConnectionParameterListViewController : ViewController<ListView>
    {
        public SimpleAction SyncParameter;
        public ConnectionParameterListViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(ConnectionParameter);

            SyncParameter = new SimpleAction(this,nameof(SyncParameter),PredefinedCategory.View);
            SyncParameter.ImageName = "ConvertTo";
            SyncParameter.Caption = "Sync";
            SyncParameter.ConfirmationMessage = "LOGO üzerinden veriler senkronize edilecektir. Bu işlem bağlantı hızınıza göre değişiklik gösterebilir. Yine de devam etmek istiyor musunuz ?";
            SyncParameter.Execute += SyncParameter_Execute;
            
                        
        }

        private void SyncParameter_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ConnectionParameter connectionParameter = View.CurrentObject as ConnectionParameter;
            if (connectionParameter != null)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(ConnectionParameter));

                SyncWarehouse(objectSpace, connectionParameter.FirmNumber);
                SyncUnitset(objectSpace, connectionParameter.FirmNumber);
                SyncSubUnitset(objectSpace, connectionParameter.PeriodNumber);
            }
        }

        private void SyncWarehouse(IObjectSpace objectSpace,int firmNumber)
        {
            var warehouses = objectSpace.GetObjects<LOGO_Warehouse>(CriteriaOperator.Parse("FirmNumber = ?",firmNumber));
            foreach (var warehouse in warehouses)
            {
                var item = objectSpace.FindObject<Warehouse>(CriteriaOperator.Parse("WarehouseNumber = ?", warehouse.WarehouseNumber));

                if (item is null)
                    item = objectSpace.CreateObject<Warehouse>();

                item.WarehouseNumber = warehouse.WarehouseNumber;
                item.WarehouseName = warehouse.WarehouseName;
                item.IntegrationResult = IntegrationResult.Integration;
                item.FirmNumber = firmNumber;
            }

            objectSpace.CommitChanges();
        }


        private void SyncUnitset(IObjectSpace objectSpace,int firmNumber)
        {
            var unitsets = objectSpace.GetObjects<LOGO_Unitset>(CriteriaOperator.Parse("FirmNumber=?", firmNumber));
            foreach (var unitset in unitsets)
            {
                var item =objectSpace.FindObject<Unitset>(CriteriaOperator.Parse("ReferenceId = ?", unitset.ReferenceId));

                if (item is null)
                    item=objectSpace.CreateObject<Unitset>();

                item.ReferenceId = unitset.ReferenceId;
                item.Code= unitset.Code;
                item.Name = unitset.Name;
                item.FirmNumber= firmNumber;
                item.Result = IntegrationResult.Integration;    

            }

            objectSpace.CommitChanges();
        }

        private void SyncSubUnitset(IObjectSpace objectSpace,int firmNumber)
        {
            var subUnitsets = objectSpace.GetObjects<LOGO_SubUnitset>(CriteriaOperator.Parse("FirmNumber=?", firmNumber));
            foreach (var subUnitset in subUnitsets)
            {
                var item = objectSpace.FindObject<SubUnitset>(CriteriaOperator.Parse("ReferenceId = ?", subUnitset.ReferenceId));

                if (item is null)
                    item=objectSpace.CreateObject<SubUnitset>();

                item.ReferenceId= subUnitset.ReferenceId;
                item.UnitsetReferenceId= subUnitset.UnitsetReferenceId;
                item.Unitset = objectSpace.FindObject<Unitset>(CriteriaOperator.Parse("ReferenceId = ?",subUnitset.UnitsetReferenceId));
                item.Code= subUnitset.Code;
                item.Name = subUnitset.Name;
                item.FirmNumber = subUnitset.FirmNumber;
                item.ConversionFactor = subUnitset.ConversionFactor;
                item.OtherConversionFactor= subUnitset.OtherConversionFactor;
                item.ConversionFactor = subUnitset.ConversionFactor;
            }

            objectSpace.CommitChanges();
        }

    }
}
