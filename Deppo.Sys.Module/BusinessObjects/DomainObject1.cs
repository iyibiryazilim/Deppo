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
    public class DomainObject1 : BaseObject
    {
        private ApplicationUser _applicationUser;
        private ApplicationUser _owner;

        public DomainObject1(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        public ApplicationUser ApplicationUser { get => _applicationUser; set => SetPropertyValue(nameof(ApplicationUser), ref _applicationUser, value); }

        public ApplicationUser Owner { get => _owner; set => SetPropertyValue(nameof(Owner), ref _owner, value); }
    }
}