using Deppo.Sys.Module.LogoBusinessObjects;
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
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("ProductProcurementManagement")]
    [ImageName("BO_Contract")]
    [Appearance("ProcurementFiche Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("ProcurementFiche", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("ProcurementFiche", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    public class ProcurementFiche : BaseObject
    {
        private int _referenceId;
        private DateTime _createdOn;
        private string _ficheNumber;
        private Customer _customer;


        public ProcurementFiche(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
        }

        [Browsable(false)]
        public int ReferenceId { get => _referenceId; set => SetPropertyValue(nameof(ReferenceId), ref _referenceId, value); }

        [ModelDefault("AllowEdit","False")]
        public DateTime CreatedOn { get => _createdOn; set => SetPropertyValue(nameof(CreatedOn), ref _createdOn, value); }

        [ModelDefault("AllowEdit", "False")]
        public string FicheNumber { get => _ficheNumber; set => SetPropertyValue(nameof(FicheNumber), ref _ficheNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public Customer Customer { get => _customer; set => SetPropertyValue(nameof(Customer), ref _customer, value); }

        [ModelDefault("AllowEdit", "False")]
        [Association("ProcurementFiche-Transactions"), DevExpress.Xpo.Aggregated]
        public XPCollection<ProcurementFicheTransaction> Transactions => GetCollection<ProcurementFicheTransaction>(nameof(Transactions));




    }
}