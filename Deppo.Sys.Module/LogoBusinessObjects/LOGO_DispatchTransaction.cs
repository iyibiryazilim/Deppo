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
    [Appearance("LOGO_DispatchTransaction Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_DispatchTransaction Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_DispatchTransaction New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    [Persistent("LOGO_DispatchTransaction")]
    public class LOGO_DispatchTransaction : XPLiteObject
    { 


        public LOGO_DispatchTransaction(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
        }

        [Key, Browsable(false)]
        [ModelDefault("AllowEdit", "False")]
        public int ReferenceId { get; set; }

        [ModelDefault("AllowEdit", "False"),NoForeignKey]
        [Association("LOGO_Dispatch-Transactions")]
        public LOGO_Dispatch Dispatch { get; set; }

        [ModelDefault("AllowEdit", "False"),NoForeignKey]
        public LOGO_Product Product { get; set; }

        [ModelDefault("AllowEdit", "False"), NoForeignKey]
        public LOGO_Unitset Unitset { get; set; }


        [ModelDefault("AllowEdit", "False"), NoForeignKey]
        public LOGO_SubUnitset SubUnitset { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public double Quantity { get; set; }




    }
}