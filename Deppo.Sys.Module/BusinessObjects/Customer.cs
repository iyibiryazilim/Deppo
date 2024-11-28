using Deppo.Sys.Module.BaseBusinessObjects;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Deppo.Sys.Module.BusinessObjects
{
    [DefaultClassOptions]
    [MapInheritance(MapInheritanceType.OwnTable)]
    [NavigationItem("CustomerManagement")]
    [ImageName("Actions_User")]
    public class Customer : Current
    { 
        public Customer(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
        }

    }
}