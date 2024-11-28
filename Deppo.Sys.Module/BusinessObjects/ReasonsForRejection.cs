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
    [NavigationItem("Audit")]

    [ImageName("Actions_Forbid")]
    public class ReasonsForRejection : BaseObject
    {
        private string _code;
        private string _name;
        private bool _isActive;

        public ReasonsForRejection(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        //[ModelDefault("AllowEdit", "False")]
        public string Code { get => _code; set => SetPropertyValue(nameof(Code), ref _code, value); }

        //[ModelDefault("AllowEdit", "False")]
        public string Name { get => _name; set => SetPropertyValue(nameof(Name), ref _name, value); }

        //[ModelDefault("AllowEdit", "False")]
        public bool IsActive { get => _isActive; set => SetPropertyValue(nameof(IsActive), ref _isActive, value); }
    }
}