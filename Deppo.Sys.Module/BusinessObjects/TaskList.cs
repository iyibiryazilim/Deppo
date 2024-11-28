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
using static DevExpress.DataProcessing.InMemoryDataProcessor.AddSurrogateOperationAlgorithm;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Workspace")]
    [ImageName("TaskList")]
    public class TaskList : BaseObject
    {
        private string _subject;
        private DateTime _createdOn;
        private ApplicationUser _user;
        private DateTime _startOn;
        private DateTime _endOn;
        private Priority _priority;
        private TaskStatus _status;
        private string _description;

        public TaskList(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [ModelDefault("AllowEdit", "False")]
        public string Subject { get => _subject; set => SetPropertyValue(nameof(Subject), ref _subject, value); }

        [ModelDefault("AllowEdit", "False")]
        public DateTime CreatedOn { get => _createdOn; set => SetPropertyValue(nameof(CreatedOn), ref _createdOn, value); }

        [ModelDefault("AllowEdit", "False")]
        public ApplicationUser User { get => _user; set => SetPropertyValue(nameof(User), ref _user, value); }

        [ModelDefault("AllowEdit", "False")]
        public DateTime StartOn { get => _startOn; set => SetPropertyValue(nameof(StartOn), ref _startOn, value); }

        [ModelDefault("AllowEdit", "False")]
        public DateTime EndOn { get => _endOn; set => SetPropertyValue(nameof(EndOn), ref _endOn, value); }

        [ModelDefault("AllowEdit", "False")]
        public Priority Priority { get => _priority; set => SetPropertyValue(nameof(Priority), ref _priority, value); }

        [ModelDefault("AllowEdit", "False")]
        public TaskStatus Status { get => _status; set => SetPropertyValue(nameof(Status), ref _status, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Description { get => _description; set => SetPropertyValue(nameof(Description), ref _description, value); }
    }
}