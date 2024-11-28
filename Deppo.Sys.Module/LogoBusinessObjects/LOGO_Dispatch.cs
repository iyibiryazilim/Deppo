using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.LogoBusinessObjects
{
    [DefaultClassOptions]
    [Appearance("LOGO_Dispatch Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_Dispatch Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_Dispatch New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    [NavigationItem("ProductProcurementManagement")]
    [ImageName("TaskList")]
    [Persistent("LOGO_Dispatch")]

    public class LOGO_Dispatch : XPLiteObject
    { 
        public LOGO_Dispatch(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
        }

        [Key, Browsable(false)]
        public int ReferenceId { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public int FicheNumber { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public DateTime CreatedOn { get; set; }

        [ModelDefault("AllowEdit", "False"),Browsable(false)]
        public int CustomerReferenceId { get; set; }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public string CustomerCode { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string CustomerName { get; set; }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int ShippedAddressReferenceId { get; set; }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public string ShippedAddressCode { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public bool IsEDispatch { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string ShippedAddressName { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string DocumentNumber { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public int WarehouseNumber { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string WarehouseName { get; set; }

        [Association("LOGO_Dispatch-Transactions"),DevExpress.Xpo.Aggregated]
        public XPCollection<LOGO_DispatchTransaction> Transactions => GetCollection<LOGO_DispatchTransaction>(nameof(Transactions));



    }
}