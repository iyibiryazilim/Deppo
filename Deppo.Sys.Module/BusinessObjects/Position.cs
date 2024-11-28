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
    [ImageName("BO_Position")]
    public class Position : BaseObject
    {
        private string _code;
        private string _name;
        private bool _isActive;

        public Position(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [RuleRequiredField]
        [RuleUniqueValue]
        public string Code { get => _code; set => SetPropertyValue(nameof(Code), ref _code, value); }

        [RuleRequiredField]
        public string Name { get => _name; set => SetPropertyValue(nameof(Name), ref _name, value); }

        public bool isActive { get => _isActive; set => SetPropertyValue(nameof(isActive), ref _isActive, value); }
    }
}