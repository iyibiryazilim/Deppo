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

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [Appearance("ProcurementFicheTransaction Delete", AppearanceItemType = "Action", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("ProcurementFicheTransaction", AppearanceItemType = "Action", TargetItems = "Edit", Visibility = ViewItemVisibility.Hide)]
    [Appearance("ProcurementFicheTransaction", AppearanceItemType = "Action", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    [NavigationItem(false)]
    public class ProcurementFicheTransaction : BaseObject
    {
        private Product _product;
        private SubUnitset _subUnitset;
        private double _quantity;
        private Warehouse _warehouse;
        private string _orderNumber;
        private int _orderReferenceId;
        private ProcurementFiche _procurementFiche;
       

        public ProcurementFicheTransaction(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
        }

        [ModelDefault("AllowEdit", "False")]
        public Product Product { get => _product; set => SetPropertyValue(nameof(Product), ref _product, value); }

        [ModelDefault("AllowEdit", "False")]
        public SubUnitset SubUnitset { get => _subUnitset; set=> SetPropertyValue(nameof(SubUnitset), ref _subUnitset, value); }

        [ModelDefault("AllowEdit", "False")]
        public double Quantity { get => _quantity; set=> SetPropertyValue(nameof(Quantity), ref _quantity, value); }

        [ModelDefault("AllowEdit", "False")]
        public Warehouse Warehouse { get => _warehouse; set => SetPropertyValue(nameof(Warehouse), ref _warehouse, value); }

        [ModelDefault("AllowEdit", "False")]
        public string OrderNumber { get => _orderNumber; set => SetPropertyValue(nameof(OrderNumber), ref _orderNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public int OrderReferenceId { get => _orderReferenceId; set => SetPropertyValue(nameof(OrderReferenceId), ref _orderReferenceId, value); }

        [ModelDefault("AllowEdit", "False")]
        [Association("ProcurementFiche-Transactions")]
        public ProcurementFiche ProcurementFiche { get => _procurementFiche; set => SetPropertyValue(nameof(ProcurementFiche), ref _procurementFiche, value); }


    }
}