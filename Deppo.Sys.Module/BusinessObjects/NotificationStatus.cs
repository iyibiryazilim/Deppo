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
    [ImageName("BO_Notifications")]
    public class NotificationStatus : BaseObject
    {
        private Notification _notification;
        private ApplicationUser _applicationUser;
        private bool _isRead;

        public NotificationStatus(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [Association("Notification-Statutes")]
        public Notification Notification { get => _notification; set => SetPropertyValue(nameof(Notification), ref _notification, value); }

        public ApplicationUser ApplicationUser { get => _applicationUser; set => SetPropertyValue(nameof(ApplicationUser), ref _applicationUser, value); }

        public bool IsRead
        { get => _isRead; set => SetPropertyValue(nameof(IsRead), ref _isRead, value); }
    }
}