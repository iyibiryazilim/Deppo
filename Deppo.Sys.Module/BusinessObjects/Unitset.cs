using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("ProductManagement")]
    [ImageName("GaugeStyleLinearHorizontal")]
    
    public class Unitset : BaseObject
    {
        private int _referenceId;
        private string _code = string.Empty;
        private string _name = string.Empty;
        private int _firmNumber;
        IntegrationResult _result;
        public Unitset(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
        }

        [ModelDefault("AllowEdit", "False"), Browsable(false)]
        public int ReferenceId { get => _referenceId; set => SetPropertyValue(nameof(ReferenceId), ref _referenceId, value); }

        [ModelDefault("AllowEdit", "False")]
        public string Code { get => _code; set => SetPropertyValue(nameof(Code), ref _code, value); }


        [ModelDefault("AllowEdit", "False")]
        public string Name { get => _name; set => SetPropertyValue(nameof(Name), ref _name, value); }

        [ModelDefault("AllowEdit", "False")]

        [Association("UnitSet-SubUnitsets"),DevExpress.Xpo.Aggregated]
        public XPCollection<SubUnitset> SubUnitsets => GetCollection<SubUnitset>(nameof(SubUnitsets));

        [ModelDefault("AllowEdit", "False")]
        public int FirmNumber { get => _firmNumber; set => SetPropertyValue(nameof(FirmNumber), ref _firmNumber, value); }

        [ModelDefault("AllowEdit", "False")]
        public IntegrationResult Result { get => _result; set => SetPropertyValue(nameof(Result), ref _result, value); }

    }
}