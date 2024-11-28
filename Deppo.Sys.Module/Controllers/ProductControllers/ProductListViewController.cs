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

namespace Deppo.Sys.Module.Controllers.ProductControllers;


public partial class ProductListViewController : ViewController<ListView>
{
    public PopupWindowShowAction SyncProduct;
    public ProductListViewController()
    {
        InitializeComponent();
        TargetObjectType = typeof(Product);

        SyncProduct = new PopupWindowShowAction(this, nameof(SyncProduct), PredefinedCategory.View);
        SyncProduct.Caption = "Sync";
        SyncProduct.ImageName = "ConvertTo";
        SyncProduct.CustomizePopupWindowParams += SyncProduct_CustomizePopupWindowParams;
    }

    private void SyncProduct_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
    {
        IObjectSpace objectSpace = Application.CreateNestedObjectSpace(View.ObjectSpace);

        string listViewId = Application.FindListViewId(typeof(LOGO_Product));

        CollectionSourceBase collectionSource = Application.CreateCollectionSource(objectSpace, typeof(LOGO_Product), listViewId);
        collectionSource.Criteria.Add("ProductListByFirmNumber", CriteriaOperator.Parse("FirmNumber = 1"));

        e.View = Application.CreateListView(listViewId, collectionSource, true);
        e.Size = new System.Drawing.Size(1024, 768);
        e.DialogController.AcceptAction.Caption = "Senkronize Et";
        e.DialogController.AcceptAction.ConfirmationMessage = "Senkronize işlemi başlatılacaktır. Devam etmek istiyor musunuz ?";
        e.DialogController.AcceptAction.Execute += Sync_AcceptAction_Execute;

    }

    private void Sync_AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        var selectedItems = e.SelectedObjects.Cast<LOGO_Product>().ToList(); //seçtiğim LOGO Ürünleri

        foreach (var selectedItem in selectedItems)
        {
            var item = View.ObjectSpace.FindObject<Product>(CriteriaOperator.Parse("ReferenceId = ?", selectedItem.ReferenceId));

            if (item is null)
                item = View.ObjectSpace.CreateObject<Product>();

            item.FirmNumber = selectedItem.FirmNumber;
            item.ReferenceId = selectedItem.ReferenceId;
            item.Code = selectedItem.Code;
            item.Name=selectedItem.Name;
            item.UnitsetReferenceId = selectedItem.UnitsetReferenceId;
            item.Vat = selectedItem.Vat;
            item.Group = selectedItem.ProductGroup;
            item.SpeCode = selectedItem.Specode;
            item.SpeCode2 = selectedItem.Specode2;
            item.SpeCode3 = selectedItem.Specode3;
            item.SpeCode4 = selectedItem.Specode4;  
            item.SpeCode5 = selectedItem.Specode5;  

        }

        View.ObjectSpace.CommitChanges();
        View.Refresh(true);

        Application.ShowViewStrategy.ShowMessage("Senkronize işlemi başarıyla tamamlandı.", InformationType.Success);
    }
}
