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
    public class City : BaseObject
    {
        private int _referenceId;
        private Country _country;
        private string _name;
        private string _code;
        private double _longitude;
        private double _latitude;
       

        public City(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
        }

        [Browsable(false)]
        [NonCloneable]
        [ModelDefault("AllowEdit", "False")]
        public int ReferenceId { get => _referenceId; set => SetPropertyValue(nameof(ReferenceId), ref _referenceId, value); }

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
        [Association("Cities-Country")]
        public Country Country
        {
            get { return _country; }
            set { SetPropertyValue("Country", ref _country, value); }
        }

        [ModelDefault("AllowEdit", "False")]
        public double Longitude
        {
            get { return _longitude; }
            set { SetPropertyValue("Longitude", ref _longitude, value); }
        }

        [ModelDefault("AllowEdit", "False")]
        public double Latitude
        {
            get { return _latitude; }
            set { SetPropertyValue("Latitude", ref _latitude, value); }
        }

        [ModelDefault("AllowEdit", "False")]
        [Association("Counties-City"), DevExpress.Xpo.Aggregated]
        public XPCollection<County> Counties
        {
            get { return GetCollection<County>("Counties"); }
        }


    }
}