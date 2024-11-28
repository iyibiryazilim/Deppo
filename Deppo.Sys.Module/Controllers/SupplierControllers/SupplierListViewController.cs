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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.Controllers.SupplierControllers
{
  
    public partial class SupplierListViewController : ViewController <ListView>
    {
        public PopupWindowShowAction SyncSupplier;
        public SupplierListViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(Supplier);

            SyncSupplier = new PopupWindowShowAction(this, nameof(SyncSupplier), PredefinedCategory.View);
            SyncSupplier.Caption = "Sync";
            SyncSupplier.ImageName = "ConvertTo";
            SyncSupplier.CustomizePopupWindowParams += SyncSupplier_CustomizePopupWindowParams;

        }

        private void SyncSupplier_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {

            IObjectSpace objectSpace = Application.CreateNestedObjectSpace(View.ObjectSpace);

            string listViewId = Application.FindListViewId(typeof(LOGO_Supplier));

            CollectionSourceBase collectionSource = Application.CreateCollectionSource(objectSpace, typeof(LOGO_Supplier), listViewId);
            collectionSource.Criteria.Add("SupplierListByFirmNumber", CriteriaOperator.Parse("FirmNumber = 1"));


            e.View = Application.CreateListView(listViewId, collectionSource, true);
            e.Size = new System.Drawing.Size(1024, 768);
            e.DialogController.AcceptAction.Caption = "Senkronize Et";
            e.DialogController.AcceptAction.ConfirmationMessage = "Senkronize işlemi başlatılacaktır. Devam etmek istiyor musunuz ?";
            e.DialogController.AcceptAction.Execute += Sync_AcceptAction_Execute;

        }

        private void Sync_AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var selectedItems = e.SelectedObjects.Cast<LOGO_Supplier>().ToList(); //seçtiğim LOGO müşteriler

            foreach (var selectedItem in selectedItems)
            {
                var item = View.ObjectSpace.FindObject<Supplier>(CriteriaOperator.Parse("ReferenceId = ?", selectedItem.ReferenceId));


                if (item is null)
                    item = View.ObjectSpace.CreateObject<Supplier>();

                item.FirmNumber = selectedItem.FirmNumber;
                item.Code = selectedItem.Code;
                item.Title = selectedItem.FirstName;
                item.TaxOffice = selectedItem.TaxOffice;
                item.TaxNumber = selectedItem.TaxNumber;
                item.Country = View.ObjectSpace.FindObject<Country>(CriteriaOperator.Parse("Name = ?", selectedItem.Country));
                item.City = View.ObjectSpace.FindObject<City>(CriteriaOperator.Parse("Name = ?", selectedItem.City));
                item.County = View.ObjectSpace.FindObject<County>(CriteriaOperator.Parse("Name = ?", selectedItem.County));
                item.PostCode = selectedItem.PostCode;
                item.Currency = View.ObjectSpace.FindObject<Currency>(CriteriaOperator.Parse("Name = ?", selectedItem.Currency));
                item.EInvoice = selectedItem.EInvoice;
                item.Telephone = selectedItem.Telephone;
                item.OtherTelephone = selectedItem.OtherTelephone;
                item.Fax = selectedItem.Fax;
                item.EMail = selectedItem.Email;
                item.WebAddress = selectedItem.WebAddress;
                item.Address = selectedItem.Address;
                item.CreditLimit = selectedItem.CreditLimit;
                item.CreditHold = selectedItem.CreditHold;
                item.ReferenceId = selectedItem.ReferenceId;
                item.IsActive = selectedItem.IsActive;
                item.IsPersonal = selectedItem.IsPersonal;
                item.CustomerDiscountRate = selectedItem.CustomerDiscountRate;
                item.Tckn = selectedItem.Tckn;
                item.IsForeign = selectedItem.IsForeign;
                item.CustomerType = CustomerType.Supplier;
                item.Result = IntegrationResult.Integration;

            }

            View.ObjectSpace.CommitChanges();
            View.Refresh(true);

            Application.ShowViewStrategy.ShowMessage("Senkronize işlemi başarıyla tamamlandı.", InformationType.Success);

        }
    }
}
