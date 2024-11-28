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
    [NavigationItem("UserManagement")]
    [ImageName("ChartType_Bar")]
    public class ProcessRate : BaseObject
    {
        private TransactionType _processType;
        private double _rate;

        public ProcessRate(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [ModelDefault("AllowEdit", "False")]
        public TransactionType ProcessType { get => _processType; set => SetPropertyValue(nameof(ProcessType), ref _processType, value); }

        [ModelDefault("AllowEdit", "False")]
        public double Rate { get => _rate; set => SetPropertyValue(nameof(Rate), ref _rate, value); }
    }
}