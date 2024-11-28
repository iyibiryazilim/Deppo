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
    [ImageName("GaugeStyleLinearHorizontal")]
    public class SubUnitset : BaseObject
    {
        private int _referenceId;
        private int _unitsetReferenceId;
        private Unitset _unitset;
        private string _code = string.Empty;
        private string _name = string.Empty;
        private double _conversionFactor;
        private double _otherConversionFactor;
        private int _firmNumber;
        IntegrationResult _result;
        public SubUnitset(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
        }
        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int ReferenceId { get => _referenceId; set => SetPropertyValue(nameof(ReferenceId), ref _referenceId, value); }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int UnitsetReferenceId { get => _unitsetReferenceId; set => SetPropertyValue(nameof(UnitsetReferenceId), ref _unitsetReferenceId, value); }


        [ModelDefault("AllowEdit", "False")]

        [Association("UnitSet-SubUnitsets")]
        public Unitset Unitset { get => _unitset; set => SetPropertyValue(nameof(Unitset), ref _unitset, value); }


        [ModelDefault("AllowEdit", "False")]
        public string Code { get => _code; set => SetPropertyValue(nameof(Code), ref _code, value); }


        [ModelDefault("AllowEdit", "False")]

        public string Name { get => _name; set => SetPropertyValue(nameof(Name), ref _name, value); }


        [ModelDefault("AllowEdit", "False")]
        public double ConversionFactor { get => _conversionFactor; set => SetPropertyValue(nameof(ConversionFactor), ref _conversionFactor, value); }


        [ModelDefault("AllowEdit", "False")]
        public double OtherConversionFactor { get => _otherConversionFactor; set => SetPropertyValue(nameof(OtherConversionFactor), ref _otherConversionFactor, value); }
        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public IntegrationResult Result { get => _result; set => SetPropertyValue(nameof(Result), ref _result, value); }






    }
}