using System.ComponentModel;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Deppo.Sys.Module.BusinessObjects;

[MapInheritance(MapInheritanceType.ParentTable)]
[DefaultProperty(nameof(UserName))]
[NavigationItem("SystemSettings")]
public class ApplicationUser : PermissionPolicyUser, IObjectSpaceLink, ISecurityUserWithLoginInfo, IAuthenticationStandardUser
{
    private string _code;
    private string _firstName;
    private string _lastName;
    private string _tckn;
    private string _address;
    private string _telephone;
    private string _otherTelephone;
    private string _webAddress;
    private string _eMail;
    private string _clientMachineName;
    private string _logoUserName;
    private MediaDataObject image;
    private Position _position;

    public ApplicationUser(Session session) : base(session)
    {
    }

    [RuleRequiredField, RuleUniqueValue]
    public string Code { get => _code; set => SetPropertyValue(nameof(Code), ref _code, value); }

    [RuleRequiredField]
    public string FirstName { get => _firstName; set => SetPropertyValue(nameof(FirstName), ref _firstName, value); }

    [RuleRequiredField]
    public string LastName { get => _lastName; set => SetPropertyValue(nameof(LastName), ref _lastName, value); }

    public string FullName => $"{FirstName} {LastName}";

    [Size(11)]
    public string TCKN { get => _tckn; set => SetPropertyValue(nameof(TCKN), ref _tckn, value); }

    [Size(250)]
    public string Address { get => _address; set => SetPropertyValue(nameof(Address), ref _address, value); }

    [RuleRequiredField]
    [ModelDefault("EditMaskType", "Simple")]
    [ModelDefault("EditMask", "(999) 000-0000")]
    public string Telephone { get => _telephone; set => SetPropertyValue(nameof(Telephone), ref _telephone, value); }

    [ModelDefault("EditMaskType", "Simple")]
    [ModelDefault("EditMask", "(999) 000-0000")]
    public string OtherTelephone { get => _otherTelephone; set => SetPropertyValue(nameof(OtherTelephone), ref _otherTelephone, value); }

    public string WebAddress { get => _webAddress; set => SetPropertyValue(nameof(WebAddress), ref _webAddress, value); }

    [RuleRequiredField, RuleRegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    public string EMail { get => _eMail; set => SetPropertyValue(nameof(EMail), ref _eMail, value); }

    public string ClientMachineName { get => _clientMachineName; set => SetPropertyValue(nameof(ClientMachineName), ref _clientMachineName, value); }

    public string LogoUserName { get => _logoUserName; set => SetPropertyValue(nameof(LogoUserName), ref _logoUserName, value); }

    [ImageEditor]
    public MediaDataObject Image { get => image; set => SetPropertyValue(nameof(Image), ref image, value); }

    [RuleRequiredField]
    public Position Position { get => _position; set => SetPropertyValue(nameof(Position), ref _position, value); }

    [Browsable(false)]
    [Aggregated, Association("User-LoginInfo")]
    public XPCollection<ApplicationUserLoginInfo> LoginInfo
    {
        get { return GetCollection<ApplicationUserLoginInfo>(nameof(LoginInfo)); }
    }

    [Association("Warehouses-Users")]
    public XPCollection<Warehouse> Warehouses => GetCollection<Warehouse>(nameof(Warehouses));

    IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => LoginInfo.OfType<ISecurityUserLoginInfo>();

    IObjectSpace IObjectSpaceLink.ObjectSpace { get; set; }

    ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey)
    {
        ApplicationUserLoginInfo result = new ApplicationUserLoginInfo(Session);
        result.LoginProviderName = loginProviderName;
        result.ProviderUserKey = providerUserKey;
        result.User = this;
        return result;
    }
}