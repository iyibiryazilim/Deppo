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
    [NavigationItem(false)]
    [ImageName("Action_OrganizeDashboard")]
    public class WarehouseProcessParameter : BaseObject
    {
        private Warehouse _warehouse;
        private TransactionType _transactionType;
        private OutProcessType _outProcessType;

        public WarehouseProcessParameter(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        public Warehouse Warehouse { get => _warehouse; set => SetPropertyValue(nameof(Warehouse), ref _warehouse, value); }

        public TransactionType TransactionType { get => _transactionType; set => SetPropertyValue(nameof(TransactionType), ref _transactionType, value); }

        public OutProcessType OutProcessType { get => _outProcessType; set => SetPropertyValue(nameof(OutProcessType), ref _outProcessType, value); }
    }
}