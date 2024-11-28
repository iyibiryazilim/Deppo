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
    [MapInheritance(MapInheritanceType.OwnTable)]
    [NavigationItem("ProductProcurementManagement")]

    [ImageName("AllowUsersToEditRanges")]
    public class ProcurementAuditCustomer : ProcurementAudit
    {
        private int _currentReferenceId;
        private string _currentCode;
        private string _currentName;

        public ProcurementAuditCustomer(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int CurrentReferenceId { get => _currentReferenceId; set => SetPropertyValue(nameof(CurrentReferenceId), ref _currentReferenceId, value); }

        [ModelDefault("AllowEdit", "False")]
        public string CurrentCode { get => _currentCode; set => SetPropertyValue(nameof(CurrentCode), ref _currentCode, value); }

        [ModelDefault("AllowEdit", "False")]
        public string CurrentName { get => _currentName; set => SetPropertyValue(nameof(CurrentName), ref _currentName, value); }
    }
}