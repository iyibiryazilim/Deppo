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
    [NavigationItem("Workspace")]
    [ImageName("BO_Notifications")]
    public class Notification : BaseObject
    {
        private string _title;
        private string _description;

        public Notification(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [ModelDefault("AllowEdit", "False")]
        public string Title { get => _title; set => SetPropertyValue(nameof(Title), ref _title, value); }

        public string Description { get => _description; set => SetPropertyValue(nameof(Description), ref _description, value); }

        [Association("Notification-Statutes"), DevExpress.Xpo.Aggregated]
        public XPCollection<NotificationStatus> Statuses => GetCollection<NotificationStatus>(nameof(Statuses));
    }
}