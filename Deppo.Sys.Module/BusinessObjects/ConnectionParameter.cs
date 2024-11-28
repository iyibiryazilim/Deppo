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
using System.Runtime.InteropServices.JavaScript;
using System.Text;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("SystemSettings")]
    [ImageName("Actions_Settings")]
    public class ConnectionParameter : BaseObject
    {
        private string _gatewayUri;
        private string _gatewayPort;
        private int _firmNumber;
        private int _periodNumber;
        private bool _isActive;

        public ConnectionParameter(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [RuleRequiredField]
        [RuleUniqueValue]
        public string GatewayUri { get => _gatewayUri; set => SetPropertyValue(nameof(GatewayUri), ref _gatewayUri, value); }

        public string GatewayPort { get => _gatewayPort; set => SetPropertyValue(nameof(GatewayPort), ref _gatewayPort, value); }

        public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

        public int PeriodNumber { get => _periodNumber; set => SetPropertyValue(nameof(PeriodNumber), ref _periodNumber, value); }


        public bool IsActive { get => _isActive; set => SetPropertyValue(nameof(IsActive), ref _isActive, value); }
    }
}