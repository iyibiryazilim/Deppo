﻿using Deppo.Sys.Module.BusinessObjects;
using Deppo.Sys.Module.CustomLogonObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;

namespace Deppo.Sys.Module.CustomLogonSecurity;

public class CustomAuthentication : AuthenticationBase, IAuthenticationStandard
{
    private CustomLogonParameters customLogonParameters;
    public CustomAuthentication()
    {
        customLogonParameters = new CustomLogonParameters();
    }
    public override void Logoff()
    {
        base.Logoff();
        customLogonParameters = new CustomLogonParameters();
    }
    public override void ClearSecuredLogonParameters()
    {
        customLogonParameters.Password = "";
        base.ClearSecuredLogonParameters();
    }
    public override object Authenticate(IObjectSpace objectSpace)
    {

        ApplicationUser applicationUser = objectSpace.FirstOrDefault<ApplicationUser>(e => e.UserName == customLogonParameters.UserName);

        if (applicationUser == null)
            throw new ArgumentNullException("ApplicationUser");

        if (!((IAuthenticationStandardUser)applicationUser).ComparePassword(customLogonParameters.Password))
            throw new AuthenticationException(
                applicationUser.UserName, "Password mismatch.");

        return applicationUser;
    }

    public override void SetLogonParameters(object logonParameters)
    {
        this.customLogonParameters = (CustomLogonParameters)logonParameters;
    }

    public override IList<Type> GetBusinessClasses()
    {
        return new Type[] { typeof(CustomLogonParameters) };
    }
    public override bool AskLogonParametersViaUI
    {
        get { return true; }
    }
    public override object LogonParameters
    {
        get { return customLogonParameters; }
    }
    public override bool IsLogoffEnabled
    {
        get { return true; }
    }
}
