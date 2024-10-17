using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
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
    public class Warehouse : BaseObject
    {
        private string _warehouseNumber;
        private string _warehouseName;

        public Warehouse(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [ModelDefault("AllowEdit", "False")]
        public string WarehouseNumber { get => _warehouseNumber; set => SetPropertyValue(nameof(WarehouseNumber), ref _warehouseNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public string WarehouseName { get => _warehouseName; set => SetPropertyValue(nameof(WarehouseName), ref _warehouseName, value); }

        [Association("Warehouses-Users")]
        public XPCollection<ApplicationUser> Users => GetCollection<ApplicationUser>(nameof(Users));
    }
}