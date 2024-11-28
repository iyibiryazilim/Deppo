using DevExpress.ExpressApp.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Module.CustomLogonSecurity;

public class CustomAuthenticationStandardProvider : AuthenticationStandardProviderV2
{
    public CustomAuthenticationStandardProvider(IOptions<AuthenticationStandardProviderOptions> options,
    IOptions<SecurityOptions> securityOptions) :
        base(options, securityOptions)
    { }
    protected override AuthenticationBase CreateAuthentication(Type userType, Type logonParametersType)
    {
        return new CustomAuthentication();
    }
}
