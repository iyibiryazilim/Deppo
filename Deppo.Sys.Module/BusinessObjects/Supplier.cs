using Deppo.Sys.Module.BaseBusinessObjects;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [MapInheritance(MapInheritanceType.OwnTable)]
    [NavigationItem("CustomerManagement")]
    [ImageName("BO_Customer")]    
    public class Supplier : Current
    { 
        public Supplier(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
           
        }

    }
}