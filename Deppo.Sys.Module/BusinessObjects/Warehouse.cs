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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("SystemSettings")]
    [ImageName("Actions_Home")]
    public class Warehouse : BaseObject
    {
        private IntegrationResult _integrationResult;
        private int _warehouseNumber;
        private string _warehouseName;
        private int _firmNumber;

        public Warehouse(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [ModelDefault("AllowEdit", "False"),ForeignKey("WarehouseNumber")]
        public int WarehouseNumber { get => _warehouseNumber; set => SetPropertyValue(nameof(WarehouseNumber), ref _warehouseNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public string WarehouseName { get => _warehouseName; set => SetPropertyValue(nameof(WarehouseName), ref _warehouseName, value); }

        [Association("Warehouses-Users")]
        public XPCollection<ApplicationUser> Users => GetCollection<ApplicationUser>(nameof(Users));

        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public IntegrationResult IntegrationResult { get => _integrationResult; set => SetPropertyValue(nameof(IntegrationResult), ref _integrationResult, value); }
    }
}