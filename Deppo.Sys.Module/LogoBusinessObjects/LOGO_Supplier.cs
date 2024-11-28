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
    [Appearance("LOGO_Supplier Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_Supplier  Edit", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("LOGO_Supplier  New", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    [Persistent("LOGO_Supplier")]

    public class LOGO_Supplier : XPLiteObject
    { 
        public LOGO_Supplier(Session session)
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

        [ModelDefault("AllowEdit", "False")]
        public string FullName { get; set; }
        [ModelDefault("AllowEdit", "False")]
        public string Code { get; set; }
        [ModelDefault("AllowEdit", "False")]
        public string TaxOffice { get; set; }
        [ModelDefault("AllowEdit", "False")]
        public string TaxNumber { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string Country { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string City { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string County { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string PostCode { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public int Currency { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public bool EInvoice { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string Telephone { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string OtherTelephone { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string Fax { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string Email { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string WebAddress { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string Address { get; set; }


        [ModelDefault("AllowEdit", "False")]
        public decimal CreditLimit { get; set; }


        [ModelDefault("AllowEdit", "False")]
        public bool CreditHold { get; set; }


        [ModelDefault("AllowEdit", "False")]
        public bool IsActive { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public bool IsPersonal { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string FirstName { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string LastName { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public double CustomerDiscountRate { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string Tckn { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public bool IsForeign { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get; set; }

        [ModelDefault("AllowEdit", "False")]
        public string CurrentType { get; set; }

        

    }
}