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
    public class Currency : BaseObject
    {
        private int _referenceId;
        private string _symbol;
        private string _name;
        private string _code;
        private short _curType;

        public Currency(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
        }

        [ModelDefault("AllowEdit", "False")]
        public string Code
        {
            get { return _code; }
            set { SetPropertyValue("Code", ref _code, value); }
        }


        [ModelDefault("AllowEdit", "False")]
        public string Name
        {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }


        [ModelDefault("AllowEdit", "False")]
        public string Symbol
        {
            get { return _symbol; }
            set { SetPropertyValue("Symbol", ref _symbol, value); }
        }

        [Browsable(false)]
        [NonCloneable]
        public int ReferenceId
        {
            get { return _referenceId; }
            set { SetPropertyValue("ReferenceId", ref _referenceId, value); }
        }

        [Browsable(false)]
        [NonCloneable]
        public short CurType
        {
            get { return _curType; }
            set { SetPropertyValue("CurType", ref _curType, value); }
        }

    }
}