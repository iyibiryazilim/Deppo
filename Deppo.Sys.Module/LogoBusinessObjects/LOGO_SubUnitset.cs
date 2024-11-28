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
    
    [Appearance("LOGO_SubUnitset Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_SubUnitset", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_SubUnitset", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    [Persistent("LOGO_SubUnitset")]
    public class LOGO_SubUnitset : XPLiteObject
    {
       


        public LOGO_SubUnitset(Session session)
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
        public int UnitsetReferenceId { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string Unitset { get; set; }


        [ModelDefault("AllowEdit", "False")]
        public string Code { get; set; }


        [ModelDefault("AllowEdit", "False")]
        public string Name { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public double ConversionFactor { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public double OtherConversionFactor { get; set; }


        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get; set; }


        

    }
}